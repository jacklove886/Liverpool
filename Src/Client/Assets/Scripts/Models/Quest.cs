using Common.Data;
using Managers;
using SkillBridge.Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Models
{
    public class Quest
    {
        public QuestDefine Define;
        public NQuestInfo Info;//如果还没接任务 不存在网络信息

        public Quest()//构造函数
        {

        }

        public Quest(NQuestInfo info)//重载
        {
            this.Info = info;
            this.Define = DataManager.Instance.Quests[info.QuestId];
        }

        public Quest(QuestDefine define)//重载
        {
            this.Define = define;
            this.Info = null;
        }
    }
}
