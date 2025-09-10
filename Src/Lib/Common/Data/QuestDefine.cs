using SkillBridge.Message;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace Common.Data
{
    public enum QuestType
    {
        //在UI界面显示时会显示"主线"/"支线"而不是"Main"/"Branch"
        [Description("主线")]
        Main,
        [Description("支线")]
        Branch
    }

    public enum QuestTarget
    {
        None,
        Kill,
        Item
    }


    public class QuestDefine
    {
        public int ID { get; set; }

        public string Name { get; set; }
        public int LimitLevel { get; set; }
        public CharacterClass LimitClass { get; set; }

        public int PreQuest { get; set; }//前置任务

        public QuestType Type { get; set; }//任务类型

        public int AcceptNPC { get; set; }
        public int SubmitNPC { get; set; }

        public string OverView { get; set; }
        public string Dialog { get; set; }
        public string DialogAccept { get; set; }
        public string DialogDeny { get; set; }
        public string DialogIncomplete { get; set; }
        public string DialogFinish { get; set; }

        public QuestTarget Target1 { get; set; }
        public int Target1ID { get; set; }
        public int Target1Num { get; set; }

        public QuestTarget Target2 { get; set; }
        public int Target2ID { get; set; }
        public int Target2Num { get; set; }

        public QuestTarget Target3 { get; set; }
        public int Target3ID { get; set; }
        public int Target3Num { get; set; }

        public int RewardGold { get; set; }
        public int RewardExp { get; set; }
        public int RewardGItem1 { get; set; }
        public int RewardGItem1Count { get; set; }
        public int RewardGItem2 { get; set; }
        public int RewardGItem2Count { get; set; }
        public int RewardGItem3 { get; set; }
        public int RewardGItem3Count { get; set; }

    }
}
