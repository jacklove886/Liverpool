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
            UIShopDialog uiShopDialog = UIManager.Instance.Show<UIShopDialog>();
            uiShopDialog.title.text = npc.Name;
            uiShopDialog.Introduce.text = npc.Introduction;
            if(npc.ID==2)
            uiShopDialog.YesButtonText.text = "购买商品";
            if(npc.ID==4)
            uiShopDialog.YesButtonText.text = "升级装备";
            if (npc.ID == 5)
            uiShopDialog.YesButtonText.text = "购买坐骑";
            uiShopDialog.NoButtonText.text = "出售道具";
            uiShopDialog.shopParam = npc.Param;
            return true;
        }
    }
}
