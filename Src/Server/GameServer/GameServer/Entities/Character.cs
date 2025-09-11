using Common.Data;
using GameServer.Core;
using GameServer.Managers;
using SkillBridge.Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameServer.Entities
{
    public class Character : CharacterBase
    {
       
        public TCharacter Data;
        public StatusManager StatusManager;
        public ItemManager ItemManager;
        public QuestManager QuestManager;
        
        
         //T开头是数据库的
        public Character(CharacterType type,TCharacter cha)://构造函数
            base(new Core.Vector3Int(cha.MapPosX, cha.MapPosY, cha.MapPosZ),new Core.Vector3Int(100,0,0))
        {

            Data = cha;
            Info = new NCharacterInfo();//NCharacterInfo是自定义的协议
            Info.Type = type;
            Info.Id = cha.ID;
            Info.Name = cha.Name;
            Info.Level = 10; //cha.Level
            Info.Tid = cha.TID;
            Info.Class = (CharacterClass)cha.Class;
            Info.mapId = cha.MapID;
            Info.Gold = cha.Gold;
            Info.Entity = this.EntityData;
            Define = DataManager.Instance.Characters[this.Info.Tid];

            //道具系统
            ItemManager = new ItemManager(this);
            ItemManager.GetItemInfos(Info.Items);

            //背包系统
            Info.Bag = new NBagInfo();
            Info.Bag.Items = this.Data.Bag.Items;
            Info.Bag.Unlocked = this.Data.Bag.Unlocked;
            //装备系统数据初始化
            Info.Equips = this.Data.Equips;

            //状态管理器
            this.StatusManager = new StatusManager(this);

            //任务系统
            this.QuestManager = new QuestManager(this);
            this.QuestManager.GetQuestInfos(Info.Quests);
        }

        public long Gold
        {
            get { return this.Data.Gold;}
            set
            {
                if (this.Data.Gold == value)
                {
                    return;
                }
                this.StatusManager.AddGoldChange((int)(value - this.Data.Gold));//新金币减去老金币
                this.Data.Gold = value;//新金币赋值
            }
        }
    }
}
