using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Models;
using Services;
using SkillBridge.Message;

namespace Managers
{
    public class EquipManager : Singleton<EquipManager>
    {
        public event System.Action OnEquipChange;

        public Item[] Equips = new Item[(int)EquipSlot.SlotMax];//数组的长度是7

        byte[] Data;

        unsafe public void Init(byte[] data)
        {
            this.Data = data;
            this.ParseEquipData(data);//解析数据
        }

        public bool Contains(int equipID)//看有没有穿这个装备
        {
            for(int i = 0; i < this.Equips.Length; i++)
            {
                if (Equips[i] != null && Equips[i].Id == equipID)
                {
                    return true;
                }       
            }
            return false;
        }

        public Item GetEquip(EquipSlot slot)//EquipSlot是枚举
        {
            return Equips[(int)slot];//返回枚举的int值 判断是哪个槽位
        }

        unsafe void ParseEquipData(byte[] data)
        {
            fixed (byte* pt = this.Data)
            {
                for(int i = 0; i < this.Equips.Length; i++)
                {
                    int itemID = *(int*)(pt + i * sizeof(int));
                    if (itemID > 0)
                    {
                        Equips[i] = ItemManager.Instance.Items[itemID];
                    }
                    else
                    {
                        Equips[i] = null;
                    }
                }
            }
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
                this.Equips[(int)slot] = null;
                if (OnEquipChange != null)
                {
                    OnEquipChange();
                }
            }
        }
    }
}
