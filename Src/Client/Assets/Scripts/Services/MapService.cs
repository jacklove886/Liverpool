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
            Debug.LogFormat("进入了地图:{0},当前角色数量:{1}",response.mapId,response.Characters.Count);

            foreach (var cha in response.Characters)
            {
               if(User.Instance.CurrentCharacter.Id==cha.Id)
               {
                    User.Instance.CurrentCharacter=cha;
               }
               CharacterManager.Instance.AddCharacter(cha);
            }

            string currentScene = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
            if (DataManager.Instance.Maps.ContainsKey(response.mapId))
            {
                string targetScene = DataManager.Instance.Maps[response.mapId].Resource;

                if (currentScene != targetScene)
                {
                    Debug.LogFormat("场景不匹配：当前场景{0}, 目标场景{1}, 开始切换", currentScene, targetScene);
                    this.EnterMap(response.mapId);
                }
            }

            this.CurrentMapID = response.mapId;  // 更新地图ID
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
            
        }
    }
}
