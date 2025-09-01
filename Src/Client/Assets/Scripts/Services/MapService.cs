using Common.Data;
using Models;
using Network;
using SkillBridge.Message;
using System;
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
        }


        public void Dispose()
        {
            MessageDistributer.Instance.Unsubscribe<MapCharacterEnterResponse>(this.OnMapCharacterEnter);
            MessageDistributer.Instance.Unsubscribe<MapCharacterLeaveResponse>(this.OnMapCharacterLeave);
        }

        public void Init()
        {
            
        }

        private void OnMapCharacterEnter(object sender, MapCharacterEnterResponse response)
        {
            Debug.LogFormat("角色进入了地图:{0},当前角色数量:{1}",response.mapId,response.Characters.Count);
            foreach(var cha in response.Characters)
            {
               if(User.Instance.CurrentCharacter.Id==cha.Id)
               {
                    User.Instance.CurrentCharacter=cha;
               }
               CharacterManager.Instance.AddCharacter(cha);
            }
            if(CurrentMapID!=response.mapId)
            {
                this.EnterMap(response.mapId);
                this.CurrentMapID=response.mapId;
            }
        }

        private void EnterMap(int mapId)
        {
         if(DataManager.Instance.Maps.ContainsKey(mapId))   
         {
            MapDefine map =DataManager.Instance.Maps[mapId];
            SceneManager.Instance.LoadScene(map.Resource);
         }
         else
         {
            Debug.LogErrorFormat("地图ID:{0}不存在",mapId);
         }
        }

        private void OnMapCharacterLeave(object sender, MapCharacterLeaveResponse response)
        {
            
        }
    }
}
