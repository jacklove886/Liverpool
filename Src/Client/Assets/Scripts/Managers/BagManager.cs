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

        public BagItem[] Items;//每个元素代表一个背包槽位 

        public NBagInfo info;//定义的NBagInfo网络协议

        //unsafe关键字允许使用指针操作
        unsafe public void Init(NBagInfo info)//初始化方法  角色进入游戏时Uservice调用
        {
            this.info = info;
            this.Unlocked = info.Unlocked;
            //Items是结构体数组    new一个新的结构体  数组的长度是Unlocked
            Items = new BagItem[this.Unlocked];
            if (info.Items != null && info.Items.Length >= this.Unlocked * sizeof(BagItem))//保证数据存在且发送的数据完整(字节长度正确)
            {
                Analyze(info.Items);
            }
            else
            {
                info.Items = new byte[sizeof(BagItem) * this.Unlocked];//创建新的字节数组  大小是槽位数量×BagItem的字节大小(4字节)
                Reset();
            }
        }

        public void Reset()//整理道具
        {
            for (int j = 0; j < this.Items.Length; j++)//必须要清空原有数据
            {
                this.Items[j].ItemID = 0;
                this.Items[j].Count = 0;
            }
            int i = 0;//当前背包槽位的位置  最终i的值表示实际使用的槽位数量
            foreach(var kv in ItemManager.Instance.Items)//遍历道具 Items是Dictionary<int, Item>类型
            {
                if (kv.Value.Count <= kv.Value.Define.StackLimit)//拥有数量小于堆叠数量
                {
                    //左边的是字节数组
                    this.Items[i].ItemID = (ushort)kv.Key;//将道具ID赋给槽位的ItemID
                    this.Items[i].Count = (ushort)kv.Value.Count;
                }
                else//拆分道具 数量大于堆叠数
                {
                    //假设两百个道具 堆叠数量99 第一次放99个 count-=99 分成99+99+2 
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
            //fixed语句固定字节数组在内存中的位置
            fixed (byte* pt = data)//指针必须写在fixed方法内 获取data的起始位置
            {
                for(int i = 0; i < this.Unlocked; i++)
                {
                    //(BagItem*)强制转换为BagItem指针类型  指针每次移动4个字节
                    BagItem* item = (BagItem*)(pt + i * sizeof(BagItem));//sizeof指获取结构所占的指针长度
                    Items[i] = *item;//*解引用操作 获取该地址的实际数据
                }
            }
        }

        unsafe public NBagInfo GetBagInfo()//背包变化时 同步数据到服务器
        {
            fixed (byte* pt = info.Items)
            {
                for(int i = 0; i < this.Unlocked; i++)//遍历所有槽位
                {
                    //item是指针变量
                    BagItem* item = (BagItem*)(pt + i * sizeof(BagItem));
                    *item = Items[i];//把实际数据写入到字节数组中  对应的引用地址发生变化
                }
            }
            return this.info;
        }

        public void AddItem(int itemId,int count)
        {
            ushort addCount = (ushort)count;
            for(int i = 0; i < Items.Length; i++)
            {
                if (this.Items[i].ItemID == itemId)
                {
                    ushort canAdd = (ushort)(DataManager.Instance.Items[itemId].StackLimit - this.Items[i].Count);
                    if (canAdd >= addCount)
                    {
                        this.Items[i].Count += addCount;
                        addCount = 0;
                        break;
                    }
                    else
                    {
                        this.Items[i].Count += canAdd;
                        addCount -= canAdd;
                    }
                }
            }
            if (addCount > 0)
            {
                for(int i = 0; i < Items.Length; i++)
                {
                    if (this.Items[i].ItemID == 0)
                    {
                        this.Items[i].ItemID = (ushort)itemId;
                        this.Items[i].Count = addCount;
                        break;
                    }
                }
            }
        }

        public void RemoveItem(int itemId, int count)
        {
            ushort deleteCount = (ushort)count;
            for (int i = 0; i < Items.Length; i++)
            {
                if (this.Items[i].ItemID == itemId)
                {
                    if(Items[i].Count>= deleteCount)//槽位数量大于要减去的数量
                    {
                        Items[i].Count -= deleteCount;
                        deleteCount = 0;
                        if (Items[i].Count == 0)
                        {
                            Items[i].ItemID = 0;
                        }
                        break;
                    }
                    else//当前槽位数量不足
                    {
                        deleteCount -= Items[i].Count;
                        Items[i].Count = 0;
                        Items[i].ItemID = 0;
                    }
                }
            }
        }

    }
}
