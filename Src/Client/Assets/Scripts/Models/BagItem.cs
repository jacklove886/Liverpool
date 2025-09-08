using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace Models
{
    //StructLayout  控制结构体在内存中的布局方式
    [StructLayout(LayoutKind.Sequential,Pack=1)]//结构布局代表结构体在内存中的存储格式
    public struct BagItem//结构体是值类型(方便两个道具之间做交换 可以实现a=b)  类是引用类型
    {
        public ushort ItemID;//ushort是一个类型占2个字节
        public ushort Count;

        public static BagItem zero = new BagItem { ItemID = 0, Count = 0 };

        public BagItem(int itemID,int count)//构造函数
        {
            this.ItemID = (ushort)itemID;
            this.Count = (ushort)count;
        }

        //两个BagItem默认不支持用==进行比较
        //必须用operator关键字来重载运算符  重载预算符后必须写下面三个方法

        public static bool operator ==(BagItem lhs,BagItem rhs)
        {
            return lhs.ItemID == rhs.ItemID && lhs.Count == rhs.Count;//需要比较类型下的关键变量(哈希表也要同步)
        }

        public static bool operator!=(BagItem lhs, BagItem rhs)
        {
            return !(lhs == rhs);//如果相等 返回false

        }
          
        public override bool Equals(object other)
        {
            if(other is BagItem) //检查传入的对象是不是BagItem类型
            {
                return Equals((BagItem)other);//转换other到BagItem类型
            }
            return false;
        }

        public bool Equals(BagItem other)
        {
            return this == other;//进行比较 相等为true
        }

        public override int GetHashCode()
        {
            //左移两位 避免生成重复的哈希码
            return ItemID.GetHashCode() ^ (Count.GetHashCode() << 2);//获取ItemID和Count的哈希码合二为一
        }



    }
}
