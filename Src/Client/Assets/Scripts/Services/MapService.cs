using Common.Data;
using Managers;
using Models;
using Network;
using SkillBridge.Message;
using System;
using System.Collections;
using UnityEngine;

namespace Services
{
    class MapService : Singleton<MapService>, IDisposable
    {

        public int CurrentMapID=0;//当前地图的ID

        public MapService()
        {
            MessageDistributer.Instance.Subscribe<MapCharacterEnterResponse>(this.OnMapCharacterEnter);
            MessageDistributer.Instance.Subscribe<MapCharacterLeaveResponse>(this.OnMapCharacterLeave);
            MessageDistributer.Instance.Subscribe<MapEntitySyncResponse>(this.OnMapEntitySync);
        }


        public void Dispose()
        {
            MessageDistributer.Instance.Unsubscribe<MapCharacterEnterResponse>(this.OnMapCharacterEnter);
            MessageDistributer.Instance.Unsubscribe<MapCharacterLeaveResponse>(this.OnMapCharacterLeave);
            MessageDistributer.Instance.Unsubscribe<MapEntitySyncResponse>(this.OnMapEntitySync);
        }

        public void Init()
        {
            
        }

        private void OnMapCharacterEnter(object sender, MapCharacterEnterResponse response)
        {
            Debug.LogFormat("进入了地图:{0},当前角色数量:{1}",response.mapId,response.Characters.Count);

            foreach (var cha in response.Characters)
            {
               if(User.Instance.CurrentCharacter.Id==cha.Id||User.Instance.CurrentCharacter==null)
               {
                    User.Instance.CurrentCharacter=cha;
               }
               CharacterManager.Instance.AddCharacter(cha);
            }
                
            if (CurrentMapID != response.mapId)
            {
                Debug.LogFormat("场景不匹配：当前场景{0}, 目标场景{1}, 开始切换", CurrentMapID, response.mapId);
                EnterMap(response.mapId);
                CurrentMapID = response.mapId;  // 更新地图ID
            }
            
        }

        private void EnterMap(int mapId)
        {
             if(DataManager.Instance.Maps.ContainsKey(mapId))   
             {
                MapDefine map =DataManager.Instance.Maps[mapId];
                User.Instance.CurrentMapData = map;
                SceneManager.Instance.LoadScene(map.Resource);
             }
             else
             {
                Debug.LogErrorFormat("地图ID:{0}不存在",mapId);
             }
        }

        private void OnMapCharacterLeave(object sender, MapCharacterLeaveResponse response)
        {
            Debug.LogFormat("{0}离开了地图", response.characterId);

            if (response.characterId != User.Instance.CurrentCharacter.Id)
            {
                CharacterManager.Instance.RemoveCharacter(response.characterId);//离开的是其他人 移除离开的人
            }
            else
            {
                CharacterManager.Instance.Clear();//自己退出 销毁所有角色
            }
        }

        //实体同步  客户端要发送消息给服务端 要进行角色移动了
        public void SendMapEntitySync(EntityEvent entityEvent,NEntity entity)
        {
            Debug.LogFormat("发送SendMapEntitySync请求:ID{0}", entity.Id);
            NetMessage message = new NetMessage();
            message.Request = new NetMessageRequest();
            message.Request.mapEntitySync = new MapEntitySyncRequest();
            message.Request.mapEntitySync.entitySync = new NEntitySync()
            {
                Id = entity.Id,
                Event = entityEvent,
                Entity = entity
            };
            NetClient.Instance.SendMessage(message);
        }

        //客户端接收到服务器发送的同步消息  进行移动同步
        private void OnMapEntitySync(object sender, MapEntitySyncResponse response)
        {
            //System.Text.StringBuilder sb = new System.Text.StringBuilder();
            //sb.AppendFormat("MapEntityUpdateResponse:Entity:{0}", response.entitySyncs.Count);
            //sb.AppendLine();
            foreach(var entity in response.entitySyncs)
            {
                EntityManager.Instance.OnEntitySync(entity);//真正有用的只有这一行 其余都是日志输出
                //sb.AppendFormat("[{0}]evt:{1} entity:{2}", entity.Id, entity.Event, entity.Entity.String());
                //sb.AppendLine();
            }
            //Debug.Log(sb.ToString());
        }

        //发送角色传送的消息
        public void SendMapTeleport(int teleporterID)
        {
            Debug.LogFormat("发送SendMapTeleport请求:teleporterID{0}", teleporterID);
            NetMessage message = new NetMessage();
            message.Request = new NetMessageRequest();
            message.Request.mapTeleport = new MapTeleportRequest();
            message.Request.mapTeleport.teleporterId = teleporterID;
            NetClient.Instance.SendMessage(message);
        }
    }
}
