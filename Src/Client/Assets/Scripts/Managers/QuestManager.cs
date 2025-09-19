using Models;
using Services;
using SkillBridge.Message;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Managers
{
    public enum NpcQuestStatus
    {
        None=0,//无任务
        Complete,//拥有已完成可提交任务
        Available,//拥有可接受任务
        Incomplete//拥有未完成任务
    }

    public class QuestManager : Singleton<QuestManager>
    {
        //所有有效任务  questInfos是服务端传来的数据
        public List<NQuestInfo> questInfos;
        //所有任务用字典保存 方便查询
        public Dictionary<int, Quest> allQuests = new Dictionary<int, Quest>();
        //key代表NPCID  Value的Key代表上面的枚举  value代表allQuests
        public Dictionary<int, Dictionary<NpcQuestStatus, List<Quest>>> npcQuests = new Dictionary<int, Dictionary<NpcQuestStatus, List<Quest>>>();

        public event System.Action<Quest> OnQuestStatusChanged;

        public void Init(List<NQuestInfo> quests)//初始化后执行的方法
        {
            this.questInfos = quests;
            allQuests.Clear();
            this.npcQuests.Clear();
            InitQuests();
        }


        void InitQuests()
        {
            //初始化已有任务
            foreach (var info in this.questInfos)//服务器传回来的信息 如果没有就是没有这个任务
            {
                Quest quest = new Quest(info);
                this.allQuests[quest.Info.QuestId] = quest;//将任务添加到可用任务字典中
            }

            this.CheakAvailableQuests();//可接任务

            //初始化可用任务
            foreach (var kv in this.allQuests)//遍历所有任务  一条一条加  试一试能不能加  条件符合就能加
            {
                this.AddNpcQuest(kv.Value.Define.AcceptNPC, kv.Value);
                this.AddNpcQuest(kv.Value.Define.SubmitNPC, kv.Value);
            }
        }

        void CheakAvailableQuests()//检查任务
        {
            foreach(var kv in DataManager.Instance.Quests)
            {
                //如果有职业要求且职业不匹配
                if (kv.Value.LimitClass != CharacterClass.None && kv.Value.LimitClass != User.Instance.CurrentCharacter.Class)
                {
                    continue;
                }
                //等级不够
                if (kv.Value.LimitLevel > User.Instance.CurrentCharacter.Level)
                {
                    continue;
                }
                //任务已存在
                if (this.allQuests.ContainsKey(kv.Key))
                {
                    continue;
                }
                if (kv.Value.PreQuest > 0)//如果有前置任务
                {
                    Quest preQuest;
                    if (this.allQuests.TryGetValue(kv.Value.PreQuest, out preQuest))//获取前置任务
                    {
                        if (preQuest.Info == null)
                        {
                            continue;//前置任务没接取
                        }
                        if (preQuest.Info.Status != QuestStatus.Finished)
                        {
                            continue;//接了前置任务但未完成
                        }
                    }
                    else//前置任务未解锁
                    {
                        continue;
                    }
                }
                //满足所有要求
                Quest quest = new Quest(kv.Value);
                //allQuests是字典
                this.allQuests[quest.Define.ID] = quest;//将任务添加进去
            }
        }

        void AddNpcQuest(int npcID,Quest quest)
        {
            
            if (!this.npcQuests.ContainsKey(npcID))
            {
                //如果是新npc 建立任务分类索引
                this.npcQuests[npcID] = new Dictionary<NpcQuestStatus, List<Quest>>();
            }

            //为每个NPC创立三个人物列表
            List<Quest> availables;
            List<Quest> complates;
            List<Quest> incomplates;

            //可接任务列表
            if(!this.npcQuests[npcID].TryGetValue(NpcQuestStatus.Available,out availables))
            {
                availables = new List<Quest>();
                this.npcQuests[npcID][NpcQuestStatus.Available] = availables;
            }
            //完成列表
            if (!this.npcQuests[npcID].TryGetValue(NpcQuestStatus.Complete, out complates))
            {
                complates = new List<Quest>();
                this.npcQuests[npcID][NpcQuestStatus.Complete] = complates;
            }
            //进行中列表
            if (!this.npcQuests[npcID].TryGetValue(NpcQuestStatus.Incomplete, out incomplates))
            {
                incomplates = new List<Quest>();
                this.npcQuests[npcID][NpcQuestStatus.Incomplete] = incomplates;
            }

            //根据任务状态添加到相应的列表中
            if (quest.Info == null)//如果还没接受
            {
                if (npcID == quest.Define.AcceptNPC && !this.npcQuests[npcID][NpcQuestStatus.Available].Contains(quest))
                {
                    this.npcQuests[npcID][NpcQuestStatus.Available].Add(quest);
                }
            }
            else//已经接受
            {
                //已经完成
                if (npcID == quest.Define.SubmitNPC&&quest.Info.Status==QuestStatus.Complated)
                {
                    if (!this.npcQuests[npcID][NpcQuestStatus.Complete].Contains(quest))
                    {
                        this.npcQuests[npcID][NpcQuestStatus.Complete].Add(quest);
                    }
                }
                //进行中
                if (npcID == quest.Define.SubmitNPC && quest.Info.Status == QuestStatus.InProgress)
                {
                    if (!this.npcQuests[npcID][NpcQuestStatus.Incomplete].Contains(quest))
                    {
                        this.npcQuests[npcID][NpcQuestStatus.Incomplete].Add(quest);
                    }
                }

            }
        }

        //获取NPC任务状态
        public NpcQuestStatus GetQuestStatusByNpc(int npcID)
        {
            Dictionary<NpcQuestStatus, List<Quest>> status = new Dictionary<NpcQuestStatus, List<Quest>>();
            if(this.npcQuests.TryGetValue(npcID,out status))//获取NPC任务
            {
                if (status[NpcQuestStatus.Complete].Count > 0)//如果有已完成的任务
                {
                    return NpcQuestStatus.Complete;
                }
                if (status[NpcQuestStatus.Available].Count > 0)//如果有可接任务
                {
                    return NpcQuestStatus.Available;
                }
                if (status[NpcQuestStatus.Incomplete].Count > 0)//如果有未完成任务
                {
                    return NpcQuestStatus.Incomplete;
                }
            }
            return NpcQuestStatus.None;
        }

        public bool OpenNpcQuest(int npcID)
        {
            Dictionary<NpcQuestStatus, List<Quest>> status = new Dictionary<NpcQuestStatus, List<Quest>>();
            if (this.npcQuests.TryGetValue(npcID, out status))//获取NPC任务
            {
                if (status[NpcQuestStatus.Complete].Count > 0)
                {
                    return ShowQuestDialog(status[NpcQuestStatus.Complete].First());
                }
                if (status[NpcQuestStatus.Available].Count > 0)
                {
                    return ShowQuestDialog(status[NpcQuestStatus.Available].First());
                }
                if (status[NpcQuestStatus.Incomplete].Count > 0)
                {
                    return ShowQuestDialog(status[NpcQuestStatus.Incomplete].First());
                }
            }
            return false;
        }

        private bool ShowQuestDialog(Quest quest)
        {
            //任务为空或者任务已完成
            if (quest.Info == null || quest.Info.Status == QuestStatus.Complated)
            {
                UIQuestDialog dialog = UIManager.Instance.Show<UIQuestDialog>();
                dialog.SetQuest(quest);
                dialog.OnClose += OnQuestDialogClose;
                return true;
            }
            //任务不为空且任务未完成
            if (quest.Info != null && quest.Info.Status != QuestStatus.Complated)
            {
                if (!string.IsNullOrEmpty(quest.Define.DialogIncomplete))
                {
                    MessageBox.Show(quest.Define.DialogIncomplete);//展示未完成任务对话
                }
            }
            return true;
        }

        void OnQuestDialogClose(UIWindow sender, UIWindow.WindowResult result)
        {
            UIQuestDialog dialog = (UIQuestDialog)sender;
            if (result == UIWindow.WindowResult.Yes)
            {
                if (dialog.quest.Info == null)
                {
                    QuestService.Instance.SendQuestAccept(dialog.quest);
                }
                else if (dialog.quest.Info.Status == QuestStatus.Complated)
                {
                    QuestService.Instance.SendQuestSubmit(dialog.quest);
                }
            }
            else if(result == UIWindow.WindowResult.No)
            {
                UIDialog uidialog = UIManager.Instance.Show<UIDialog>();
                uidialog.Introduce.text = dialog.quest.Define.DialogDeny;
                uidialog.title.text = "那很坏了";
                uidialog.YesButtonText.text = "确认";
            }
        }

        Quest RefreshQuestStatus(NQuestInfo quest)
        {
            this.npcQuests.Clear();
            Quest result;
            if (this.allQuests.ContainsKey(quest.QuestId))//曾经接过的任务
            {
                //更新新的任务状态
                this.allQuests[quest.QuestId].Info = quest;
                result = this.allQuests[quest.QuestId];
            }
            else//新接的任务
            {
                result = new Quest(quest);
                this.allQuests[quest.QuestId] = result;
            }

            CheakAvailableQuests();

            foreach (var kv in this.allQuests)//遍历所有任务  一条一条加  试一试能不能加  条件符合就能加
            {
                this.AddNpcQuest(kv.Value.Define.AcceptNPC, kv.Value);
                this.AddNpcQuest(kv.Value.Define.SubmitNPC, kv.Value);
            }
            if (OnQuestStatusChanged != null)
            {
                OnQuestStatusChanged(result);
            }
            return result;
        }

        public void OnQuestAccepted(NQuestInfo info)
        {
            var quest = this.RefreshQuestStatus(info);
            UIDialog uidialog = UIManager.Instance.Show<UIDialog>();
            uidialog.Introduce.text = quest.Define.DialogAccept;
        }

        public void OnQuestSubmited(NQuestInfo info)
        {
            var quest = this.RefreshQuestStatus(info);
            UIDialog uidialog = UIManager.Instance.Show<UIDialog>();
            uidialog.Introduce.text = quest.Define.DialogFinish;
        }
    }
}
