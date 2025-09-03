using System;
using System.Collections.Generic;
using UnityEngine;          
using Entities;
using SkillBridge.Message;
using System.Linq;

namespace Managers
{
    class CharacterManager : Singleton<CharacterManager>, IDisposable
    {
        [Header("角色管理")]
        public Dictionary<int,Character>Characters=new Dictionary<int,Character>();
        public UnityEngine.Events.UnityAction<Character>OnCharacterEnter;
        public UnityEngine.Events.UnityAction<Character>OnCharacterLeave;

        public void Init()
        {

        }
        public void Clear()
        {
            int[] keys = Characters.Keys.ToArray();

            foreach(var key in keys)
            {
                RemoveCharacter(key);
            }

            Characters.Clear();
        }

        public void Dispose()
        {
            
        }

        public void AddCharacter(SkillBridge.Message.NCharacterInfo cha)
        {
            Debug.LogFormat("加入角色姓名:{0},地图:{1}",cha.Name,cha.mapId);
            Character character=new Character(cha);
            this.Characters[cha.Id]=character;
            EntityManager.Instance.AddEntity(character);

            //这句话永远不会执行
            if (OnCharacterEnter!= null)
            {
                OnCharacterEnter(character);
            }
        }

        public void RemoveCharacter(int characterId)
        {
            Debug.LogFormat("移除角色ID：{0}",characterId);
            if (this.Characters.ContainsKey(characterId))
            {
                EntityManager.Instance.RemoveEntity(Characters[characterId].Info.Entity);
                if (OnCharacterLeave != null)
                {
                    OnCharacterLeave(Characters[characterId]);
                }
            }
            this.Characters.Remove(characterId);
        }
    }
}
