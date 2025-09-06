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
            if (!eventMap.ContainsKey(function))//如果字典里还没有这个功能
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
            if (npc.Type == NpcType.Task)//如果是Task类型
            {
                return DoTaskInteractive(npc);
            }
            else if (npc.Type == NpcType.Functional)//如果是Fcuntional类型
            {
                return DoFunctionInteractive(npc);
            }
            return false;
        }

        private bool DoTaskInteractive(NpcDefine npc)
        {
            UIMessageBox msgBox = MessageBox.Show("你好旅行者", npc.Name, MessageBoxType.Confirm, "开始对话", "取消");
            msgBox.OnYes = () =>
            {
                UIDialog uIDialog = UIManager.Instance.Show<UIDialog>();
                uIDialog.title.text = npc.Name;
                uIDialog.Introduce.text = npc.Introduction;
                uIDialog.ButtonText.text = "接受任务";
            };
            return true;
        }

        private bool DoFunctionInteractive(NpcDefine npc)
        {
            if (npc.Type != NpcType.Functional)
            {
                return false;
            }
            if (!eventMap.ContainsKey(npc.Function))//如果没用注册功能事件
            {
                return false;
            }
            return eventMap[npc.Function](npc);//调用功能事件
        }
    }
}
