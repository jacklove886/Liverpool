using Common;
using Common.Data;
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
        public Result BuyItem(NetConnection<NetSession>sender,int shopID,int shopItemID)//谁来买东西
        {
            if (!DataManager.Instance.Shops.ContainsKey(shopID))
            {
                return Result.Failed;
            }
            ShopItemDefine shopItem;
            if(DataManager.Instance.ShopItems[shopID].TryGetValue(shopItemID,out shopItem))
            {
                Log.InfoFormat("购买商品:角色:{0}:商品:{1}数量:{2}价格:{3}", sender.Session.Character, shopItem.ItemID, shopItem.Count, shopItem.Price);
                if (sender.Session.Character.Gold >= shopItem.Price)
                {
                    sender.Session.Character.ItemManager.AddItem(shopItem.ItemID, shopItem.Count);
                    sender.Session.Character.Gold -= shopItem.Price;
                    DBService.Instance.Save();
                    return Result.Success;
                }
                else//钱不够
                {
                    return Result.Failed;
                }
                
            }
            return Result.Failed;
        }

    }
}
