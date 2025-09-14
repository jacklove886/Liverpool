using Common;
using Common.Data;
using GameServer.Core;
using GameServer.Managers;
using GameServer.Models;
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
       
        public TCharacter TCharacter;
        public StatusManager StatusManager;
        public ItemManager ItemManager;
        public QuestManager QuestManager;
        public FriendManager FriendManager;

        public Team Team;//没有DB数据 所以做了个Team类
        public int TeamUpdateTS;//队伍更新时间的时间戳   每个角色的都不一样

        public Guild Guild;
        public int GuildUpdateTS;


         //T开头是数据库的
        public Character(CharacterType type,TCharacter cha)://构造函数
            base(new Core.Vector3Int(cha.MapPosX, cha.MapPosY, cha.MapPosZ),new Core.Vector3Int(100,0,0))
        {

            TCharacter = cha;
            this.Id=cha.ID;
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
            Info.Bag.Items = this.TCharacter.Bag.Items;
            Info.Bag.Unlocked = this.TCharacter.Bag.Unlocked;
            //装备系统数据初始化
            Info.Equips = this.TCharacter.Equips;

            //状态管理器
            this.StatusManager = new StatusManager(this);

            //任务系统
            this.QuestManager = new QuestManager(this);
            this.QuestManager.GetQuestInfos(Info.Quests);

            //好友系统
            this.FriendManager = new FriendManager(this);
            this.FriendManager.GetFriendInfos(Info.Friends);

            this.Guild = GuildManager.Instance.GetGuild(this.TCharacter.GuildId);
            
        }

        public long Gold
        {
            get { return this.TCharacter.Gold;}
            set
            {
                if (this.TCharacter.Gold == value)
                {
                    return;
                }
                this.StatusManager.AddGoldChange((int)(value - this.TCharacter.Gold));//新金币减去老金币
                this.TCharacter.Gold = value;//新金币赋值
            }
        }

        public NCharacterInfo GetBasicInfo()
        {
            return new NCharacterInfo()//创建一个安全的、只包含基本信息的副本 被好友系统所调用
            {
                Id = Info.Id,//Info是NCharacterInfo
                Name = Info.Name,
                Class = Info.Class,
                Level = Info.Level
            };
        }

        public void PostProcess(NetMessageResponse message)
        {
            this.FriendManager.PostProcess(message);//好友管理器后处理
            if (this.Team != null)
            {
                //Log.InfoFormat("PostProcess>Team:characterID:{0}:{1} {2}<{3}", this.Id, this.Info.Name, TeamUpdateTS, this, Team.changeTime);
                if (TeamUpdateTS < this.Team.changeTime)//时间戳小于队伍更新的时间
                {
                    TeamUpdateTS = (int)Team.changeTime;
                    this.Team.PostProcess(message);
                }
            }
            if (this.Guild != null)
            {
                if (this.Info.Guild == null)//本地没有公会信息
                {
                    this.Info.Guild = this.Guild.GuildInfo(this);
                    if (message.mapCharacterEnter != null)//角色第一次登陆 获取公会信息
                    {
                        GuildUpdateTS = (int)Guild.changeTime;
                    }
                }
                if (GuildUpdateTS < this.Guild.changeTime&&message.mapCharacterEnter==null)//时间戳小于队伍更新的时间
                {
                    GuildUpdateTS = (int)Guild.changeTime;
                    this.Guild.PostProcess(this,message);
                }
            }


            if (this.StatusManager.HasStatus)
            {
                this.StatusManager.PostProcess(message);//状态管理器后处理
            }
        }

        public void Clear()
        {
            this.FriendManager.TellFriendsLeaving();//离线通知
        }
    }
}
