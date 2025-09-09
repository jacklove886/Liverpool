using SkillBridge.Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Common.Data
{
    public enum ItemFunction
    {
        RecoverHp,
        RecoverMP,
        AddBuff,
        AddExp,
        AddMoney,
        AddItem,
        AddSkillPoint,
    }

    public class ItemDefine
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }//描述
        public ItemType Type { get; set; }
        public string Category { get; set; }//类别
        public int Level { get; set; }//使用等级
        public CharacterClass LimitClass { get; set; }//使用角色类型
        public bool CanUse { get; set; }//能否使用
        public float UseCD { get; set; }
        public int Price { get; set; }
        public int SellPrice { get; set; }//售卖价格
        public int StackLimit { get; set; } //堆叠的限制
        public string Icon { get; set; }//道具的图标
        public ItemFunction Function { get; set; }//道具的功能
        public int Param { get; set; }
        public List<int> Params { get; set; }
    }
}
