using Models;
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
                Quest quest = new Quest(info);
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
                if (kv.Value.PreQuest > 0)//如果有前置任务
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
                    else//前置任务未解锁
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

        void AddNpcQuest(int npcID,Quest quest)
        {
            
            if (!this.npcQuests.ContainsKey(npcID))
            {
                //如果是新npc 建立任务分类索引
                this.npcQuests[npcID] = new Dictionary<NpcQuestStatus, List<Quest>>();
            }
            List<Quest> availables;
            List<Quest> complates;
            List<Quest> incomplates;

            if(!this.npcQuests[npcID].TryGetValue(NpcQuestStatus.Available,out availables))
            {
                availables = new List<Quest>();
                this.npcQuests[npcID][NpcQuestStatus.Available] = availables;
            }
            if (!this.npcQuests[npcID].TryGetValue(NpcQuestStatus.Complete, out complates))
            {
                complates = new List<Quest>();
                this.npcQuests[npcID][NpcQuestStatus.Complete] = complates;
            }
            if (!this.npcQuests[npcID].TryGetValue(NpcQuestStatus.Incomplete, out incomplates))
            {
                incomplates = new List<Quest>();
                this.npcQuests[npcID][NpcQuestStatus.Incomplete] = incomplates;
            }
            if (quest.Info == null)
            {
                if (npcID == quest.Define.AcceptNPC && !this.npcQuests[npcID][NpcQuestStatus.Available].Contains(quest))
                {
                    this.npcQuests[npcID][NpcQuestStatus.Available].Add(quest);
                }
            }
            else
            {
                if (npcID == quest.Define.SubmitNPC&&quest.Info.Status==QuestStatus.Complated)
                {
                    if (!this.npcQuests[npcID][NpcQuestStatus.Complete].Contains(quest))
                    {
                        this.npcQuests[npcID][NpcQuestStatus.Complete].Add(quest);
                    }
                }
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
                if (status[NpcQuestStatus.Complete].Count > 0)
                {
                    return NpcQuestStatus.Complete;
                }
                if (status[NpcQuestStatus.Available].Count > 0)
                {
                    return NpcQuestStatus.Available;
                }
                if (status[NpcQuestStatus.Incomplete].Count > 0)
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
                MessageBox.Show(dialog.quest.Define.DialogAccept);
            }
            else if(result == UIWindow.WindowResult.No)
            {
                UIDialog uidialog = UIManager.Instance.Show<UIDialog>();
                uidialog.Introduce.text = dialog.quest.Define.DialogDeny;
                uidialog.title.text = "那很坏了";
                uidialog.ButtonText.text = "确认";
            }
        }

        public void OnQuestAccepted(NQuestInfo quest)
        {

        }

        public void OnQuestSubmited(NQuestInfo quest)
        {

        }
    }
}
