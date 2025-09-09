using SkillBridge.Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Common.Data
{
    public class EquipDefine
    {
        public int ID { get; set; }
        public EquipSlot Slot { get; set; }//槽位
        public string Category { get; set; }//类别 战士法师游侠
        public float Strength { get; set; }//力量
        public float Intelligence { get; set; }//智力
        public float Dexterity { get; set; }//敏捷
        public float Hp { get; set; }//血量
        public float Mp { get; set; }//蓝量
        public float Ad { get; set; }//物理攻击
        public float Ap { get; set; }//法强
        public float Defense { get; set; }//物理防御
        public float MagicDefense { get; set; }//法术防御
        public float Speed { get; set; }//速度
        public float Critical { get; set; }//暴击
    }
}
