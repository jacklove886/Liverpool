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
            this.Characters[cha.EntityId]=character;
            EntityManager.Instance.AddEntity(character);

            //这句话永远不会执行
            if (OnCharacterEnter!= null)
            {
                OnCharacterEnter(character);
            }
        }

        public void RemoveCharacter(int entityId)
        {
            Debug.LogFormat("移除角色ID：{0}", entityId);
            if (this.Characters.ContainsKey(entityId))
            {
                EntityManager.Instance.RemoveEntity(Characters[entityId].Info.Entity);
                if (OnCharacterLeave != null)
                {
                    OnCharacterLeave(Characters[entityId]);
                }
            }
            this.Characters.Remove(entityId);
        }

        public Character GetCharacter(int id)
        {
            Character character = null;
            this.Characters.TryGetValue(id, out character);
            return character;
        }
    }
}
