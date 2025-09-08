using Network;
using SkillBridge.Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Services
{
    class ItemService:Singleton<ItemService>
    {
        public ItemService()//构造函数
        {
            MessageDistributer.Instance.Subscribe<ItemBuyResponse>(this.OnItemBuy);
        }

        public void Dispose()
        {
            MessageDistributer.Instance.Unsubscribe<ItemBuyResponse>(this.OnItemBuy);
        }

        public void SendBuyItem(int shopID,int shopItemID)
        {
            Debug.Log("发送购买商品请求");

            NetMessage message = new NetMessage();
            message.Request = new NetMessageRequest();
            message.Request.itemBuy = new ItemBuyRequest();
            message.Request.itemBuy.shopID = shopID;
            message.Request.itemBuy.shopItemID = shopItemID;
            NetClient.Instance.SendMessage(message);
        }

        private void OnItemBuy(object sender,ItemBuyResponse response)
        {
            MessageBox.Show("购买结果" + response.Result, "购买完成");
        }
    }
}
