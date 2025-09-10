using Common.Data;
using Managers;
using SkillBridge.Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Models
{
    public class Quest//实体类 具体的任务Quest
    {
        public QuestDefine Define;
        public NQuestInfo Info;

        public Quest()//构造函数
        {

        }

        public Quest(NQuestInfo info)//重载   从服务器创建任务
        {
            this.Info = info;//从服务器同步任务信息
            this.Define = DataManager.Instance.Quests[info.QuestId];//获得具体QuestId的那个任务的数据
        }

        public Quest(QuestDefine define)//重载  从配置数据创建任务
        {
            this.Define = define;
            this.Info = null;//表示任务还没被接受
        }
    }
}
