using Common.Data;
using Managers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Managers
{
    class NpcManager:Singleton<NpcManager>
    {
        public delegate bool NpcActionHandler(NpcDefine npc);//定义NPC行为委托  返回值是bool

        Dictionary<NpcFunction, NpcActionHandler> eventMap = new Dictionary<NpcFunction, NpcActionHandler>();

        public void RegisterNpcEvent(NpcFunction function,NpcActionHandler action)
        {
            if (!eventMap.ContainsKey(function))//如果字典里还没有这个功能 目前只有:InvokeShop  InvokeInsrance
            {
                eventMap[function] = action;//添加功能到字典中
            }
            else
            {
                eventMap[function] += action;
            }
        }

        public NpcDefine GetNpcDefine(int npcId)
        {
            NpcDefine npc = null;
            DataManager.Instance.Npcs.TryGetValue(npcId,out npc);//获得到NpcDefine的变量
            return npc;//可以利用npc.来获取里面的值 (比如Name 或者 Description)
        }

        //最主要的NPC交互方法
        public bool Interactive(NpcDefine npc) 
        {
            if (DoTaskInteractive(npc))
            {
                return true;
            }
            else if (npc.Type == NpcType.Functional)//如果是Fcuntional类型
            {
                return DoFunctionInteractive(npc);
            }
            return false;
        }

        private bool DoTaskInteractive(NpcDefine npc)
        {
            var status = QuestManager.Instance.GetQuestStatusByNpc(npc.ID);
            //NPC状态为空的话 返回false
            if (status == NpcQuestStatus.None)
                return false;
            return QuestManager.Instance.OpenNpcQuest(npc.ID);
        }

        //功能类型
        private bool DoFunctionInteractive(NpcDefine npc)
        {
            if (npc.Type != NpcType.Functional)
            {
                return false;
            }
            if (!eventMap.ContainsKey(npc.Function))//如果没有注册功能事件
            {
                return false;
            }
            return eventMap[npc.Function](npc);//调用功能事件  传入npc参数
        }
    }
}
