using Common;
using GameServer.Entities;
using GameServer.Services;
using Network;
using SkillBridge.Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameServer.Managers
{
    class EquipManager:Singleton<EquipManager>
    {
        public Result EquipItem(NetConnection<NetSession>sender,int slot,int itemID,bool isEquip)
        {
            Character character = sender.Session.Character;
            if (!character.ItemManager.Items.ContainsKey(itemID))//ItemID不存在
            {
                return Result.Failed;
            }
            if (isEquip)//只在穿装备时检查
            {
                var itemDefine = DataManager.Instance.Items[itemID];
                if (itemDefine.LimitClass != character.Info.Class)
                {
                    return Result.Failed; // 职业不符合
                }
            }
            UpdateEquip(character.Data.Equips, slot, itemID, isEquip);//更新装备
            DBService.Instance.Save();
            return Result.Success;
        }

        //更新装备
        unsafe void UpdateEquip(byte[]equipData,int slot,int itemID,bool isEquip)
        {
            fixed (byte* pt = equipData)//每个装备槽位占4个字节 fixed防止内存移动
            {
                //(int*)表示转换为int*类型  固定移动4个字节
                int* slotid = (int*)(pt + slot * sizeof(int));
                if (isEquip)//穿装备
                {
                    *slotid = itemID;//解引用操作 把值写入指针指向的内存地址
                }
                else//脱装备
                {
                    *slotid = 0;//该槽位为空 没有值
                }
            }
        }

    }
}
