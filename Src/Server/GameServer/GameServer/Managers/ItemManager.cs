using Common;
using GameServer.Entities;
using GameServer.Models;
using GameServer.Services;
using SkillBridge.Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameServer.Managers
{
    public class ItemManager
    {
        Character Owner;//Character类实例  角色对象

        public Dictionary<int, Item> Items = new Dictionary<int, Item>();//Key是ID，Value是道具对象

        public ItemManager(Character owner)//构造函数
        {
            this.Owner = owner;

            foreach (var item in owner.Data.Items)//遍历角色身上的所有道具
            {
                this.Items.Add(item.ItemID, new Item(item));//添加道具
            }
        }

        public bool UseItem(int itemID,int count = 1)//默认使用一个道具
        {
            //中括号是装饰性符号(普通文本字符)  大括号是占位符
            Log.InfoFormat("[角色:{0}]使用了:[{1}数量:{2}]", Owner.Data.ID, itemID, count);
            Item item = null;//用TrgGetValue前必须声明变量
            if(Items.TryGetValue(itemID,out item))//存在返回item 不存在返回null
            {
                if (item.Count < count)
                {
                    return false;
                }
                //TODO:使用道具的逻辑

                item.Remove(count);
                return true;
            }
            return false;
        }

        public bool HasItem(int itemID)//检查道具是否存在 比如交任务前是否有任务道具
        {
            Item item = null;
            if(this.Items.TryGetValue(itemID,out item))
            {
                return item.Count > 0;//数量大于0就return true
            }
            return false;
        }

        public Item SearchItem(int itemID)//获取指定道具
        {
            Item item = null;
            this.Items.TryGetValue(itemID, out item);//访问ID来尝试获取道具
            Log.InfoFormat("[角色:{0}]查询了:[{1}]", Owner.Data.ID, item);
            return item;//如果获取到了返回item  否则TryGetValue返回null
        }

        public bool AddItem(int itemID,int count)
        {
            Item item = null;
            if (Items.TryGetValue(itemID, out item))//如果字典已经存在 直接添加
            {
                    item.Add(count);
            }

            else//从数据库里创建实体
            {
                TCharacterItem dbItem = new TCharacterItem();
                dbItem.CharacterID = Owner.Data.ID;
                dbItem.Owner = Owner.Data;
                dbItem.ItemID = itemID;
                dbItem.ItemCount = count;
                Owner.Data.Items.Add(dbItem);//添加到角色数据中
                item = new Item(dbItem);
                this.Items.Add(itemID, item);//添加到字典中
            }
            Log.InfoFormat("[角色:{0}]添加了:[{1}]数量为:{2}", Owner.Data.ID, item,count);
            DBService.Instance.Save();
            return true;
        }

        public bool RemoveItem(int itemID,int count)
        {
            if (!this.Items.ContainsKey(itemID))
            {
                return false;
            }
            Item item = this.Items[itemID];
            if (item.Count < count)
            {
                return false;
            }
            item.Remove(count);
            Log.InfoFormat("[角色:{0}]移除了:[{1}]数量为:{2}", Owner.Data.ID, item, count);
            DBService.Instance.Save();
            return true;
        }

        public void GetItemInfos(List<NItemInfo> list)//获取所有道具  方便内存数据转换为网络数据
        {
            foreach(var item in this.Items)
            {
                //item 是<int, Item> 类型
                list.Add(new NItemInfo() { Id = item.Value.ItemID, Count = item.Value.Count });//数据添加到NItemInfo列表中
            }
        }

    }
}
