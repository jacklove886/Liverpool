using Common.Data;
using Services;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Managers
{
    public class ShopManager : Singleton<ShopManager>
    {
        public void Init()
        {

        }
        public void ShowShop(int shopID)
        {
            ShopDefine shop;
            //传入的参数是商店ID:shopID
            if(DataManager.Instance.Shops.TryGetValue(shopID,out shop))
            {
                UIShop uiShop = UIManager.Instance.Show<UIShop>();
                if (uiShop != null)
                {
                    uiShop.SetShop(shop);
                }
            }
        }

        public void ShowSellShop()
        {
            if (ItemManager.Instance != null&&ItemManager.Instance.Items!=null)
            {
                foreach(var item in ItemManager.Instance.Items)
                {
                    UIShop uISellShop = UIManager.Instance.Show<UIShop>();
                    if (uISellShop != null)
                    {
                        uISellShop.SetSellShop(item.Value);
                    }
                }
            }
        }

        public bool BuyItem(int shopID,int shopItemID)
        {
            ItemService.Instance.SendBuyItem(shopID, shopItemID);
            return true;//表示请求发送成功 不代表购买成功
        }

        public bool SellItem(int shopItemID, int count)
        {
            ItemService.Instance.SendBuyItem(shopItemID, count);
            return true;//表示请求发送成功 不代表购买成功
        }
    }
}

