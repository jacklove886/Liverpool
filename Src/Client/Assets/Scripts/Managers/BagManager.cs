using Models;
using SkillBridge.Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Managers
{
    public class BagManager:Singleton<BagManager>
    {
        public int Unlocked;//解锁的格子数

        public BagItem[] Items;

        public NBagInfo info;

        unsafe public void Init(NBagInfo info)
        {
            this.info = info;
            this.Unlocked = info.Unlocked;
            Items = new BagItem[this.Unlocked];
            if (info.Items != null && info.Items.Length >= this.Unlocked)
            {
                Analyze(info.Items);
            }
            else
            {
                info.Items = new byte[sizeof(BagItem) * this.Unlocked];
                Reset();
            }
        }

        public void Reset()//整理道具
        {
            int i = 0;
            foreach(var kv in ItemManager.Instance.Items)//遍历道具
            {
                if (kv.Value.Count <= kv.Value.Define.StackLimit)//拥有数量小于堆叠数量
                {
                    this.Items[i].ItemID = (ushort)kv.Key;
                    this.Items[i].Count = (ushort)kv.Value.Count;
                }
                else//拆分道具 数量大于可堆叠数
                {
                    //假设两百个道具 堆叠数量99 第一次放99个 count-=99 还剩103个 继续循环
                    int count = kv.Value.Count;
                    while (count > kv.Value.Define.StackLimit)
                    {
                        this.Items[i].ItemID = (ushort)kv.Key;
                        this.Items[i].Count = (ushort)kv.Value.Define.StackLimit;
                        i++;
                        count -= kv.Value.Define.StackLimit;
                    }
                    this.Items[i].ItemID = (ushort)kv.Key;
                    this.Items[i].Count = (ushort)count;
                }
                i++;
            }
        }

        unsafe void Analyze(byte[] data)//通过地址来访问数据  必须用结构体
        {
            fixed (byte* pt = data)//*是指针 指向data的指针pt  指针必须写在fixed方法内  
            {
                for(int i = 0; i < this.Unlocked; i++)
                {
                    BagItem* item = (BagItem*)(pt + i * sizeof(BagItem));//sizeof指这个结构占几个字节
                    Items[i] = *item;
                }
            }
        }

        unsafe public NBagInfo GetBagInfo()
        {
            fixed (byte* pt = info.Items)
            {
                for(int i = 0; i < this.Unlocked; i++)
                {
                    BagItem* item = (BagItem*)(pt + i * sizeof(BagItem));
                    *item = Items[i];
                }
            }
            return this.info;
        }
    }
}
