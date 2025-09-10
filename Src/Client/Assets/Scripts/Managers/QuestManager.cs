using Models;
using SkillBridge.Message;
using System.Collections;
using System.Collections.Generic;
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

        public void Init(List<NQuestInfo> quests)
        {
            this.questInfos = quests;
            allQuests.Clear();
            this.npcQuests.Clear();
            InitQuests();
        }

        
        void InitQuests()
        {

            //初始化已有任务
            foreach(var info in this.questInfos)
            {
                Quest quest = new Quest();
                this.AddNpcQuest(quest.Define.AcceptNPC, quest);
                this.AddNpcQuest(quest.Define.SubmitNPC, quest);
                this.allQuests[quest.Info.QuestId] = quest;
            }

            //初始化可用任务
            foreach(var kv in DataManager.Instance.Quests)
            {
                //如果不是通用任务或者职业不符合
                if (kv.Value.LimitClass != CharacterClass.None && kv.Value.LimitClass != User.Instance.CurrentCharacter.Class)
                {
                    continue;
                }
                //等级不够
                if (kv.Value.LimitLevel >User.Instance.CurrentCharacter.Level)
                {
                    continue;
                }
                //任务已存在
                if (this.allQuests.ContainsKey(kv.Key))
                {
                    continue;
                }
                if (kv.Value.PreQuest > 0)
                {
                    Quest preQuest;
                    if (this.allQuests.TryGetValue(kv.Value.PreQuest,out preQuest))//获取前置任务
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
                    else if(kv.Value.PreQuest<=0)//前置任务还没接
                    {
                        continue;
                    }
                }
                Quest quest = new Quest(kv.Value);
                this.AddNpcQuest(quest.Define.AcceptNPC, quest);
                this.AddNpcQuest(quest.Define.SubmitNPC, quest);
                this.allQuests[quest.Define.ID] = quest;
            }
        }
    }
}
