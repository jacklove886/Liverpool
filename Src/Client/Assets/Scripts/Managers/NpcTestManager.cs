using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Common.Data;
using UnityEngine.Events;

namespace Managers
{
    class NpcTestManager:Singleton<NpcTestManager>
    {
        public void Init()//游戏启动时被LoadingManager调用
        {
            NpcManager.Instance.RegisterNpcEvent(Common.Data.NpcFunction.InvokeShop, OnNpcInvokeShop);//商店功能
            NpcManager.Instance.RegisterNpcEvent(Common.Data.NpcFunction.InvokeInsrance, OnNpcInvokeInsrance);//副本功能
        }

        //方法匹配委托定义public delegate bool NpcActionHandler(NpcDefine npc)
        private bool OnNpcInvokeShop(NpcDefine npc)
        {
            UIDialog uIDialog = UIManager.Instance.Show<UIDialog>();
            uIDialog.title.text = npc.Name;
            uIDialog.Introduce.text = npc.Introduction;
            if(npc.ID==2)
            uIDialog.ButtonText.text = "购买商品";
            if(npc.ID==4)
            uIDialog.ButtonText.text = "升级装备";
            uIDialog.shopParam = npc.Param;
            return true;
        }

        private bool OnNpcInvokeInsrance(NpcDefine npc)
        {
            UIMessageBox msgBox = MessageBox.Show("你好旅行者", npc.Name, MessageBoxType.Confirm, "开始对话", "取消");
            msgBox.OnYes = () =>
            {
                UIDialog uIDialog = UIManager.Instance.Show<UIDialog>();
                uIDialog.title.text = npc.Name;
                uIDialog.Introduce.text = npc.Introduction;
            };
            msgBox.OnNo = () =>
            {
                //什么都不做;
            };    
            return true;
        }

    }
}
