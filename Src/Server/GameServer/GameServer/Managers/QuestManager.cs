using Common.Data;
using GameServer.Entities;
using GameServer.Services;
using Network;
using SkillBridge.Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameServer.Managers
{
    public class QuestManager
    {
        Character Owner;

        public QuestManager(Character owner)//构造函数
        {
            this.Owner = owner;
        }

        public void GetQuestInfos(List<NQuestInfo> list)
        {
            foreach(var quest in this.Owner.Data.Quests)
            {
                list.Add(GetQuestInfo(quest));
            }
        }

        //从T数据库转成N的网络信息
        public NQuestInfo GetQuestInfo(TCharacterQuest quest)
        {
            return new NQuestInfo()
            {
                QuestId = quest.QuestID,
                QuestDbid=quest.Id,
                Status=(QuestStatus)quest.Status,
                Targets=new int[3] {quest.Target1,quest.Target2,quest.Target3},
            };
        }

        public Result AcceptQuest(NetConnection<NetSession> sender, int questID)
        {
            Character character = sender.Session.Character;

            QuestDefine quest;

            //校验配置表有没有这个任务
            if(DataManager.Instance.Quests.TryGetValue(questID,out quest))
            {
                var dbquest = DBService.Instance.Entities.CharacterQuests.Create();//创建数据表
                dbquest.QuestID = quest.ID;
                if (quest.Target1 == QuestTarget.None)// 任务没有目标 直接完成
                {
                    
                    dbquest.Status = (int)QuestStatus.Complated;
                }
                else //任务有目标
                {
                    dbquest.Status = (int)QuestStatus.InProgress;//设置为进行中
                }
                //把操作完的任务状态发送给客户端
                sender.Session.Response.questAccept.Quest = this.GetQuestInfo(dbquest);
                character.Data.Quests.Add(dbquest);
                DBService.Instance.Save();//保存到数据库
                return Result.Success;
            }
            else
            {
                sender.Session.Response.questAccept.Errormsg = "任务不存在";
                return Result.Failed;
            }
        }

        public Result SubmitQuest(NetConnection<NetSession> sender, int questID)
        {
            Character character = sender.Session.Character;

            QuestDefine quest;

            //校验配置表有没有这个任务
            if (DataManager.Instance.Quests.TryGetValue(questID, out quest))
            {
                //在玩家数据表里查询任务ID是指定任务的任务 返回第一个元素
                var dbquest = character.Data.Quests.Where(q => q.QuestID == questID).FirstOrDefault();
                if (dbquest != null)
                {
                    if (dbquest.Status != (int)QuestStatus.Complated)//任务还未完成
                    {
                        sender.Session.Response.questSubmit.Errormsg = "任务未完成";
                        return Result.Failed;
                    }
                    dbquest.Status = (int)QuestStatus.Finished;//任务已完成
                    sender.Session.Response.questSubmit.Quest = this.GetQuestInfo(dbquest);
                    DBService.Instance.Save();//保存到数据库

                    //处理任务奖励
                    if (quest.RewardGold > 0)
                    {
                        character.Gold += quest.RewardGold;
                    }

                    if (quest.RewardExp > 0)
                    {
                        //character.Exp += quest.RewardExp; 暂时还没写经验
                    }

                    if (quest.RewardGItem1 > 0)
                    {
                        character.ItemManager.AddItem(quest.RewardGItem1, quest.RewardGItem1Count);
                    }

                    if (quest.RewardGItem2 > 0)
                    {
                        character.ItemManager.AddItem(quest.RewardGItem2, quest.RewardGItem2Count);
                    }

                    if (quest.RewardGItem3 > 0)
                    {
                        character.ItemManager.AddItem(quest.RewardGItem3, quest.RewardGItem3Count);
                    }
                    DBService.Instance.Save();
                    return Result.Success;                   
                }
                sender.Session.Response.questSubmit.Errormsg = "数据库任务不存在";
                return Result.Failed;
            }
            else
            {
                sender.Session.Response.questSubmit.Errormsg = "数据配置文件里任务不存在";
                return Result.Failed;
            }
        }
    }
}
