using Common.Data;
using GameServer.Core;
using GameServer.Managers;
using Network;
using SkillBridge.Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameServer.Entities
{
    public class Character : CharacterBase, IPostResponser//基类是Entity
    {
       
        public TCharacter Data;
        public StatusManager StatusManager;
        public ItemManager ItemManager;
        public QuestManager QuestManager;
        public FriendManager FriendManager;


         //T开头是数据库的
        public Character(CharacterType type,TCharacter cha)://构造函数
            base(new Core.Vector3Int(cha.MapPosX, cha.MapPosY, cha.MapPosZ),new Core.Vector3Int(100,0,0))
        {

            Data = cha;
            Info = new NCharacterInfo();//NCharacterInfo是自定义的协议
            Info.Type = type;
            Info.Id = cha.ID;
            Info.EntityId = this.entityId;//继承自Entity
            Info.Name = cha.Name;
            Info.Level = 10; //cha.Level
            Info.ConfigId = cha.TID;
            Info.Class = (CharacterClass)cha.Class;
            Info.mapId = cha.MapID;
            Info.Gold = cha.Gold;
            Info.Entity = this.EntityData;
            Define = DataManager.Instance.Characters[this.Info.ConfigId];

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

            //好友系统
            this.FriendManager = new FriendManager(this);
            this.FriendManager.GetFriendInfos(Info.Friends);
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

        public void PostProcess(NetMessageResponse message)
        {
            this.FriendManager.PostProcess(message);//好友管理器后处理
            if (this.StatusManager.HasStatus)
            {
                this.StatusManager.PostProcess(message);//状态管理器后处理
            }
        }

        public void Clear()
        {
            this.FriendManager.UpdateFriendInfo(this.Info, 0);
        }
    }
}
