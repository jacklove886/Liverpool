using Managers;
using Models;
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
            //发送请求 订阅响应消息并调用方法  
            MessageDistributer.Instance.Subscribe<ItemBuyResponse>(this.OnItemBuy);
            MessageDistributer.Instance.Subscribe<ItemEquipResponse>(this.OnItemEquip);
        }

        public void Dispose()
        {
            MessageDistributer.Instance.Unsubscribe<ItemBuyResponse>(this.OnItemBuy);
            MessageDistributer.Instance.Unsubscribe<ItemEquipResponse>(this.OnItemEquip);
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
            if (response.Result == Result.Success)
            {
                MessageBox.Show("购买成功", "购买完成");
                ChatManager.Instance.AddSystemMessage(string.Format("恭喜购买【{0}】成功",response.Msg), "系统");
                SoundManager.Instance.PlayUI(SoundDefine.Gold);
            }
           
            else
            {
                MessageBox.Show(response.Msg, "购买失败",MessageBoxType.Error);
            }
        }

        //pendingEquip里有装备信息  发送请求的时候记录下来
        Item pendingEquip = null;
        bool isEquip = false;//是否为装备操作 false为脱装备

        public bool SendEquip(Item equip, bool isEquip)
        {
            if (pendingEquip!= null)//如果当前有正在处理的请求 比如连续点击
            {
                return false;
            }
            Debug.Log("发送装备穿戴请求");
            pendingEquip = equip;
            this.isEquip = isEquip;

            NetMessage message = new NetMessage();
            message.Request = new NetMessageRequest();
            message.Request.itemEquip = new ItemEquipRequest();
            message.Request.itemEquip.Slot = (int)equip.EquipInfo.Slot;//传入装备槽
            message.Request.itemEquip.itemId = equip.Id;//传入商品ID
            message.Request.itemEquip.isEquip = isEquip;//传入是否穿戴
            NetClient.Instance.SendMessage(message);//发送消息
            return true;
        }

        //利用pendingEquip  返回的时候就知道穿戴的装备信息
        private void OnItemEquip(object sender, ItemEquipResponse response)
        {
            if (response.Result == Result.Success)
            {
                if (pendingEquip != null)
                {
                    if (this.isEquip)
                    {
                        EquipManager.Instance.OnEquipItem(pendingEquip);//穿装备
                    }
                    else
                    {
                        EquipManager.Instance.OnUnEquipItem(pendingEquip.EquipInfo.Slot);//脱装备
                    }
                    pendingEquip = null;
                }
            }
            
        }

    }
}
