using Models;
using SkillBridge.Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Common.Data;

namespace Managers
{
    public class ItemManager:Singleton<ItemManager>
    {
        public Dictionary<int, Item> Items = new Dictionary<int, Item>();

        internal void Init(List<NItemInfo> items)//初始化  在UserService的GameEnter里调用 进入游戏初始化道具系统
        {
            this.Items.Clear();//清空字典
            foreach(var info in items)
            {
                Item item = new Item(info);//这里的info是List<NItemInfo>  items遍历出来的  而不是字典里的Items
                this.Items.Add(item.Id, item);//遍历角色的道具 添加到字典中
                Debug.LogFormat("ItemManager初始化道具:[{0}]", item);
            }
        }

        //获取到道具
        public ItemDefine GetItem(int itemID)
        {
            return null;
        }

        //方法重载  通过ID使用道具 下面是通过道具定义使用道具
        public bool UseItem(int itemID)
        {
            return false;
        }

        public bool UseItem(ItemDefine item)
        {
            return false;
        }
    }
}
