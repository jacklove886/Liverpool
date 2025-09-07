using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace Models
{
    [StructLayout(LayoutKind.Sequential,Pack=1)]//结构布局代表结构体在内存中的存储格式
    public struct BagItem//结构体是值类型(方便两个道具之间做交换 可以实现a=b)  类是引用类型
    {
        public ushort ItemID;//ushort是一个类型占2个字节
        public ushort Count;

        public static BagItem zero = new BagItem { ItemID = 0, Count = 0 };

        public BagItem(int itemID,int count)
        {
            this.ItemID = (ushort)itemID;
            this.Count = (ushort)count;
        }

        public static bool operator ==(BagItem lhs,BagItem rhs)
        {
            return lhs.ItemID == rhs.ItemID && lhs.Count == rhs.Count;
        }

        public static bool operator!=(BagItem lhs, BagItem rhs)
        {
            return !(lhs == rhs);
        }

        public override bool Equals(object other)
        {
            if(other is BagItem)
            {
                return Equals((BagItem)other);
            }
            return false;
        }

        public bool Equals(BagItem other)
        {
            return this == other;
        }

        public override int GetHashCode()
        {
            return ItemID.GetHashCode() ^ (Count.GetHashCode() << 2);
        }



    }
}
