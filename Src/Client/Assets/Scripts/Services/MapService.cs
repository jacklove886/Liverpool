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
            Debug.LogFormat("进入了地图:{0},当前角色数量:{1}",response.mapId,response.Characters.Count);

            foreach (var cha in response.Characters)
            {
               if(User.Instance.CurrentCharacter.Id==cha.Id)
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
    }
}
