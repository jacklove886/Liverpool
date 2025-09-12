using Common;
using Common.Data;
using GameServer.Entities;
using GameServer.Managers;
using Network;
using SkillBridge.Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameServer.Services
{
    class MapService : Singleton<MapService>
    {
        public MapService()
        {
            MessageDistributer<NetConnection<NetSession>>.Instance.Subscribe<MapEntitySyncRequest>(this.OnMapEntitySync);
            MessageDistributer<NetConnection<NetSession>>.Instance.Subscribe<MapTeleportRequest>(this.OnMapTeleport);
        }

        public void Init()
        {
            MapManager.Instance.Init();
        }

        private void OnMapEntitySync(NetConnection<NetSession> sender, MapEntitySyncRequest request)
        {
            Character character = sender.Session.Character;

            // 添加空值检查
            if (character == null)
            {
                Log.WarningFormat("OnMapEntitySync: character is null, ignoring request");
                return;
            }

            if (character.Info == null)
            {
                Log.WarningFormat("OnMapEntitySync: character.Info is null for character {0}", character.Id);
                return;
            }

            if (request?.entitySync == null)
            {
                Log.WarningFormat("OnMapEntitySync: entitySync is null for character {0}", character.Id);
                return;
            }

            // 检查地图是否存在
            if (!MapManager.Instance.Maps.ContainsKey(character.Info.mapId))
            {
                Log.WarningFormat("OnMapEntitySync: Map {0} not found for character {1}", character.Info.mapId, character.Id);
                return;
            }

            MapManager.Instance[character.Info.mapId].UpdateEntity(request.entitySync);
        }

        internal void SendEntityUpdate(NetConnection<NetSession> sender, NEntitySync entity)
        {
            sender.Session.Response.mapEntitySync = new MapEntitySyncResponse();
            sender.Session.Response.mapEntitySync.entitySyncs.Add(entity);

            //消息打包成数据流发给客户端
            sender.SendResponse();
        }

        private void OnMapTeleport(NetConnection<NetSession> sender, MapTeleportRequest request)
        {
            Character character = sender.Session.Character;
            Log.InfoFormat("OnMapTeleport接收到请求,characterID:{0},TeleporterID:{1}", character.entityId, request.teleporterId);
            if (!DataManager.Instance.Teleporters.ContainsKey(request.teleporterId))
            {
                Log.WarningFormat("传送点:{0}不存在", request.teleporterId);
                return;
            }

            //request.teleporterId是当前传送点ID
            TeleporterDefine source = DataManager.Instance.Teleporters[request.teleporterId];
            if (source.LinkTo==0|| !DataManager.Instance.Teleporters.ContainsKey(source.LinkTo))
            {
                Log.WarningFormat("连接传送点:{0}不存在", source.LinkTo);
            }

            TeleporterDefine target= DataManager.Instance.Teleporters[source.LinkTo];//source.LinkTo是目标传送点

            MapManager.Instance[source.MapID].CharacterLeave(character);//source.MapID是当前地图ID
            //传送点ID和地图ID不一样  传送点ID是在UI里配置的  地图ID是配置表里的
            character.Position = target.Position;
            character.Direction = target.Direction;
            MapManager.Instance[target.MapID].CharacterEnter(sender,character);//target.MapID是目标地图ID

        }
    }
}
