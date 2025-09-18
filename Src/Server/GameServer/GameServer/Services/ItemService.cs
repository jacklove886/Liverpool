using Common;
using GameServer.Entities;
using GameServer.Managers;
using Network;
using SkillBridge.Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameServer.Services
{
    class ItemService:Singleton<ItemService>
    {
        public ItemService()
        {
            //订阅请求信息 发送响应
            MessageDistributer<NetConnection<NetSession>>.Instance.Subscribe<ItemBuyRequest>(this.OnItemBuy);
            MessageDistributer<NetConnection<NetSession>>.Instance.Subscribe<ItemEquipRequest>(this.OnItemEquip);
        }

        public void Init()
        {

        }

        //购买装备请求
         void OnItemBuy(NetConnection<NetSession> sender, ItemBuyRequest request)
        {
            Character character = sender.Session.Character;
            Log.InfoFormat("购买请求:角色:{0}:商店:{1}:物品:{2}", character.Id, request.shopID, request.shopItemID);
            var result = ShopManager.Instance.BuyItem(sender, request.shopID, request.shopItemID);//返回方法的结果
            sender.Session.Response.itemBuy = new ItemBuyResponse();
            sender.Session.Response.itemBuy.Result = result;
            sender.Session.Response.itemBuy.Msg = request.shopItemID.ToString();
            sender.SendResponse();
        }

        //穿装备请求
         void OnItemEquip(NetConnection<NetSession> sender, ItemEquipRequest request)
        {
            Character character = sender.Session.Character;
            Log.InfoFormat("装备请求:角色:{0} 装备栏:{1} 物品:{2}", character.Id, request.Slot, request.itemId);
            var result = EquipManager.Instance.EquipItem(sender, request.Slot, request.itemId,true);//返回这个方法的结果 是true还是false
            sender.Session.Response.itemEquip = new ItemEquipResponse();
            sender.Session.Response.itemEquip.Result = result;
            sender.SendResponse();
        }
    }
}
