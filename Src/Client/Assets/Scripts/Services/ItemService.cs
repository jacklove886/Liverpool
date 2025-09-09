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
            //订阅ItemBuyResponse类型的响应消息  发送响应调用OnItemBuy
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
            message.Request.itemBuy.shopID = shopID;//传入商店ID
            message.Request.itemBuy.shopItemID = shopItemID;//传入商品ID
            NetClient.Instance.SendMessage(message);//发送消息
        }

        private void OnItemBuy(object sender,ItemBuyResponse response)
        {
            if(response.Result==Result.Success)
            MessageBox.Show("购买成功！" ,"购买完成");
            else
            {
                MessageBox.Show("购买失败！" + response.Errormsg, "购买失败");
            }
        }
    }
}
