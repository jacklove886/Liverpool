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
        }

        //方法匹配委托定义public delegate bool NpcActionHandler(NpcDefine npc)
        private bool OnNpcInvokeShop(NpcDefine npc)
        {
            UIDialog uIDialog = UIManager.Instance.Show<UIDialog>();
            uIDialog.title.text = npc.Name;
            uIDialog.Introduce.text = npc.Introduction;
            if(npc.ID==2)
            uIDialog.YesButtonText.text = "购买商品";
            if(npc.ID==4)
            uIDialog.YesButtonText.text = "升级装备";
            if (npc.ID == 5)
            uIDialog.YesButtonText.text = "购买坐骑";
            uIDialog.NoButtonText.text = "出售道具";
            uIDialog.shopParam = npc.Param;
            return true;
        }
    }
}
