using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameServer.Models
{
    public class Item
    {
        TCharacterItem dbItem;

        public int ItemID;
        public int Count;

        public Item(TCharacterItem item)//构造函数
        {
            this.dbItem = item;
            this.ItemID = item.ItemID;
            this.Count = item.ItemCount;
        }

        //添加道具方法
        public void Add(int count)
        {
            this.Count += count;
            dbItem.ItemCount = this.Count;
        }

        //删除道具方法
        public void Remove(int count)
        {
            this.Count -= count;
            dbItem.ItemCount = this.Count;
        }

        //使用方法
        public bool Use(int count = 1)
        {
            return false;//暂时没和其他系统关联  为空
        }

        public override string ToString()
        {
            return string.Format("ID:{0},Count:{1}", this.ItemID, this.Count);
        }
    }
}
