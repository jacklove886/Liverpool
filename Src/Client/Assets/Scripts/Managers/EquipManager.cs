using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Models;
using Services;
using SkillBridge.Message;
using UnityEngine;

namespace Managers
{
    public class EquipManager : Singleton<EquipManager>
    {
        public event System.Action OnEquipChange;

        //因为EquipSlot枚举是从0开始 所以加一个字段SlotMax表示枚举最大长度
        public Item[] Equips = new Item[(int)EquipSlot.SlotMax];//数组的长度是7

        byte[] Data;

        unsafe public void Init(byte[] data)//在Uservice里调用 初始化装备管理器
        {
            this.Data = data;//保存装备数据
            this.AnalyzeEquipData(data);//解析数据
        }
   
        unsafe void AnalyzeEquipData(byte[] data)//解析装备
        {
            fixed (byte* pt = this.Data)//固定内存地址
            {
                for(int i = 0; i < this.Equips.Length; i++)
                {
                    int itemID = *(int*)(pt + i * sizeof(int));
                    if (itemID > 0)//说明该槽位有装备
                    {
                        Debug.Log("装备槽位" + i + " itemID=" + itemID);
                        Equips[i] = ItemManager.Instance.Items[itemID];
                    }
                    else//没装备
                    {
                        Equips[i] = null;//槽位设置为空
                    }
                }
            }
        }

        public bool Contains(int equipID)//看有没有穿这个装备
        {
            for (int i = 0; i < this.Equips.Length; i++)
            {
                if (Equips[i] != null && Equips[i].Id == equipID)
                {
                    return true;//该槽位穿了装备
                }
            }
            return false;
        }

        public Item GetEquip(EquipSlot slot)//EquipSlot是枚举
        {
            return Equips[(int)slot];//返回枚举的int值 判断是哪个槽位
        }

        unsafe public byte[] GetEquipData()
        {
            fixed (byte* pt = Data)
            {
                for(int i = 0; i < (int)EquipSlot.SlotMax; i++)
                {
                    int* itemID = (int*)(pt + i * sizeof(int));
                    if (Equips[i] == null)
                    {
                        *itemID = 0;
                    }
                    else
                    {
                        *itemID = Equips[i].Id;
                    }
                }
            }
            return this.Data;
        }

        public void EuqipItem(Item equip)//装备
        {
            //发送请求
            ItemService.Instance.SendEquip(equip, true);
        }

        public void UnEuqipItem(Item equip)//脱装备
        {
            ItemService.Instance.SendEquip(equip, false);
        }

        //接受服务器响应后 由ItemService调用
        public void OnEquipItem(Item equip)
        {
            //如果该槽位已经是这个装备
            if (this.Equips[(int)equip.EquipInfo.Slot] != null && this.Equips[(int)equip.EquipInfo.Slot].Id == equip.Id)
                {
                return;
            }
            this.Equips[(int)equip.EquipInfo.Slot] = ItemManager.Instance.Items[equip.Id];
            if (OnEquipChange != null)
            {
                OnEquipChange();
            }
        }

        public void OnUnEquipItem(EquipSlot slot)
        {
            if (this.Equips[(int)slot] != null)
            {
                this.Equips[(int)slot] = null;//清空槽位
                if (OnEquipChange != null)
                {
                    OnEquipChange();
                }
            }
        }
    }
}
