using Common.Data;
using Managers;
using SkillBridge.Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Models
{
    public class Item
    {
        public int Id;
        public int Count;
        public ItemDefine Define;//道具信息
        public EquipDefine EquipInfo;//装备信息

        public Item(NItemInfo item) ://构造函数   接受网络NItemInfo  
            this(item.Id, item.Count)
        {
        }

        //重载构造函数
        public Item(int Id,int count)
        {
            this.Id = Id;
            this.Count = count;
            //获取道具和装备
            DataManager.Instance.Items.TryGetValue(this.Id, out this.Define);
            DataManager.Instance.Equips.TryGetValue(this.Id, out this.EquipInfo);
        }

        public override string ToString()
        {
            return string.Format("ID:{0},Count:{1}", this.Id, this.Count);
        }
    }
}
