using Common;
using Common.Data;
using GameServer.Models;
using GameServer.Services;
using Network;
using SkillBridge.Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameServer.Managers
{
    class ShopManager:Singleton<ShopManager>
    {
        public Result BuyItem(NetConnection<NetSession>sender,int shopID,int shopItemID)//谁来买东西  买的商店ID 和商品ID是什么
        {
            if (!DataManager.Instance.Shops.ContainsKey(shopID))//如果商店ID不存在
            {
                return Result.Failed;
            }
            ShopItemDefine shopItem;
            if(DataManager.Instance.ShopItems[shopID].TryGetValue(shopItemID,out shopItem))//查找商品ID  返回商品配置信息
            {
                Log.InfoFormat("购买商品:角色:{0}:商品:{1}数量:{2}价格:{3}", sender.Session.Character, shopItem.ItemID, shopItem.Count, shopItem.Price);
                if (sender.Session.Character.Gold >= shopItem.Price)//如果角色钱够
                {
                    //把商品ID和数量加进去
                    sender.Session.Character.ItemManager.AddItem(shopItem.ItemID, shopItem.Count);
                    //扣钱
                    sender.Session.Character.Gold -= shopItem.Price;
                    DBService.Instance.Save();
                    return Result.Success;//返回成功消息
                }
                else//钱不够
                {
                    return Result.Failed;
                }
                
            }
            return Result.Failed;
        }

        public Result SellItem(NetConnection<NetSession> sender, int shopItemID, int count)
        {
            DataManager.Instance.Items.TryGetValue(shopItemID,out ItemDefine itemDefine);
            if (ItemManager.Items.TryGetValue(shopItemID,out Item item))
            {
                if (item.Count < count)
                {
                    return Result.Failed;
                }
            }
            sender.Session.Character.ItemManager.AddItem(shopItemID, -count);
            sender.Session.Character.Gold += itemDefine.SellPrice * count;
            DBService.Instance.Save();
            return Result.Success;
        }

    }
}
