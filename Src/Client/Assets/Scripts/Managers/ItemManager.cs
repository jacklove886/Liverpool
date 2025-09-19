using Models;
using SkillBridge.Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Common.Data;
using Services;

namespace Managers
{
    public class ItemManager:Singleton<ItemManager>
    {
        public Dictionary<int, Item> Items = new Dictionary<int, Item>();

        internal void Init(List<NItemInfo> items)//初始化  在UserService的GameEnter里调用 进入游戏初始化道具系统
        {
            this.Items.Clear();//清空字典 确保没有旧数据
            foreach(var info in items)
            {
                //创建new Item 来调用构造函数  传入参数
                Item item = new Item(info);//这里的info是List<NItemInfo>  items遍历出来的  而不是字典里的Items
                this.Items.Add(item.Id, item);//遍历角色的道具 添加到字典中
                Debug.LogFormat("ItemManager初始化道具:[{0}]", item);
            }
            //注册物品状态变更事件
            StatusService.Instance.RegisterStatusNofity(StatusType.Item, OnItemNotify);
        }

        //获取到道具
        public ItemDefine GetItem(int itemID)
        {
            return null;
        }

        private bool OnItemNotify(Nstatus status)
        {
            //根据行为来调用不同的方法
            if (status.Action == StatusAction.Add)
            {
                this.AddItem(status.Id, status.Value);
            }
            if (status.Action == StatusAction.Delete)
            {
                this.RemoveItem(status.Id, status.Value);
            }
            return true;
        }

        void AddItem(int itemId,int count)
        {
            Item item = null;
            if(this.Items.TryGetValue(itemId,out item))
            {
                item.Count += count;//如果ID存在直接添加
            }
            else
            {
                item = new Item(itemId, count);//不存在新建再添加
                this.Items.Add(itemId, item);
            }
            BagManager.Instance.AddItem(itemId, count);//同步背包
        }

        void RemoveItem(int itemId, int count)
        {
            if (!this.Items.ContainsKey(itemId))//不存在返回
            {
                return;
            }
            Item item = this.Items[itemId];
            if (item.Count < count)//已有数量小于要删除的 返回
            {
                return;
            }
            if (item.Count == count)
            {
                BagManager.Instance.RemoveItem(itemId, count);
                Items.Remove(itemId);
            }
            else
            {
                item.Count -= count;
                BagManager.Instance.RemoveItem(itemId, count);
            }           
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

        public int GetRideId()
        {
            foreach (var item in ItemManager.Instance.Items)
            {
                if (item.Value.Define.Type == ItemType.Ride&& item.Value.Define.LimitClass==User.Instance.CurrentCharacter.Class)
                {
                    return item.Value.Id;
                }
            }
            return 0;
        }
    }
}
