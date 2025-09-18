using Common;
using Common.Data;
using GameServer.Entities;
using GameServer.Managers;
using GameServer.Services;
using Network;
using SkillBridge.Message;
using System.Collections.Generic;

namespace GameServer.Models
{
    public class Map
    {
        internal class MapCharacter
        {
            public NetConnection<NetSession> sender;
            public Character character;

            public MapCharacter(NetConnection<NetSession> conn, Character cha)
            {
                this.sender = conn;
                this.character = cha;
            }
        }

        public int ID
        {
            get { return this.Define.ID; }
        }
        internal MapDefine Define;

        //字典以角色的CharacterID为Key
        Dictionary<int, MapCharacter> MapCharacters = new Dictionary<int, MapCharacter>();

        //刷怪管理器
        private SpawnManager SpawnManager = new SpawnManager();
        public MonsterManager MonsterManager = new MonsterManager();

        internal Map(MapDefine define)//构造函数 初始化
        {
            this.Define = define;
            this.SpawnManager.Init(this);
            this.MonsterManager.Init(this);
        }

        internal void Update()//每秒运行10帧
        {
            SpawnManager.Update();
        }

        /// <summary>
        /// 角色进入地图
        /// </summary>
        /// <param name="character"></param>
        internal void CharacterEnter(NetConnection<NetSession> sender, Character character)
        {
            Log.InfoFormat("CharacterEnter: Map:{0} characterId:{1}", this.Define.ID, character.Id);

            character.Info.mapId = this.ID;
            this.MapCharacters[character.Id] = new MapCharacter(sender, character);

            sender.Session.Response.mapCharacterEnter = new MapCharacterEnterResponse();
            sender.Session.Response.mapCharacterEnter.mapId = this.Define.ID;

            foreach (var kv in this.MapCharacters)
            {
                sender.Session.Response.mapCharacterEnter.Characters.Add(kv.Value.character.Info);
                if (kv.Value.character != character)
                {
                    this.AddCharacterEnterMap(kv.Value.sender, character.Info);
                }
            }
            foreach(var kv in this.MonsterManager.Monsters)
            {
                sender.Session.Response.mapCharacterEnter.Characters.Add(kv.Value.Info);
            }
            sender.SendResponse();
        }

        internal void CharacterLeave(Character cha)
        {
            Log.InfoFormat("CharacterLeave: Map:{0} characterId:{1}", this.Define.ID, cha.Id);
            foreach(var kv in MapCharacters)
            {
                SendCharacterLeaveMap(kv.Value.sender, cha);
            }
            MapCharacters.Remove(cha.Id);
        }

        void AddCharacterEnterMap(NetConnection<NetSession> connection, NCharacterInfo character)
        {
            if (connection.Session.Response.mapCharacterEnter == null)//角色如果为空
            {
                //创建
                connection.Session.Response.mapCharacterEnter = new MapCharacterEnterResponse();
                connection.Session.Response.mapCharacterEnter.mapId = this.Define.ID;
            } 
            connection.Session.Response.mapCharacterEnter.Characters.Add(character);
            connection.SendResponse();//为了性能 以后不加这句话
        }

        private void SendCharacterLeaveMap(NetConnection<NetSession> connection, Character character)
        {
            connection.Session.Response.mapCharacterLeave = new MapCharacterLeaveResponse();
            connection.Session.Response.mapCharacterLeave.entityId = character.entityId;
            connection.SendResponse();
        }

        internal void UpdateEntity(NEntitySync entity)
        {
            foreach(var kv in MapCharacters)//遍历当前地图上所有玩家
            {
                if (kv.Value.character.entityId == entity.Id)//如果发送同步的是自己
                {
                    //更新客户端数据到服务端
                    kv.Value.character.Position = entity.Entity.Position;
                    kv.Value.character.Direction= entity.Entity.Direction;
                    kv.Value.character.Speed = entity.Entity.Speed;
                    if (entity.Event == EntityEvent.EventRide)
                    {
                        kv.Value.character.Ride = entity.Param;//如果上马 把ID传进来
                    }
                }
                else//通知其他人需要移动同步
                {
                    MapService.Instance.SendEntityUpdate(kv.Value.sender, entity);
                }
            }
        }

        internal void MonsterEnter(Monster monster)
        {
            Log.InfoFormat("怪物进入地图:{0},怪物ID:{1}", this.Define.ID, monster.Id);
            //遍历地图上所有玩家
            foreach(var kv in this.MapCharacters)
            {
                //向玩家发送进入地图的消息
                this.AddCharacterEnterMap(kv.Value.sender, monster.Info);
            }
        }

    }
}
