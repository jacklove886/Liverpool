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
            //Log.InfoFormat("接收到请求OnMapEntitySync:{0}:{1} Entity.Id{2} Entity:{3}", character.Id, character.Info.Name, request.entitySync.Id,request.entitySync.Entity.String());
            MapManager.Instance[character.Info.mapId].UpdateEntity(request.entitySync);  
        }

        internal void SendEntityUpdate(NetConnection<NetSession> sender, NEntitySync entity)
        {
            NetMessage message = new NetMessage();
            message.Response = new NetMessageResponse();
            message.Response.mapEntitySync = new MapEntitySyncResponse();
            message.Response.mapEntitySync.entitySyncs.Add(entity);

            //消息打包成数据流发给客户端
            byte[] data = PackageHandler.PackMessage(message);
            sender.SendData(data, 0, data.Length);
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
