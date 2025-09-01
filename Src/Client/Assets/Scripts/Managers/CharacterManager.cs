using System;
using System.Collections.Generic;
using UnityEngine;          
using Entities;
using SkillBridge.Message;

namespace Services
{
    class CharacterManager : Singleton<CharacterManager>, IDisposable
    {
        [Header("角色管理")]
        public Dictionary<int,Character>Characters=new Dictionary<int,Character>();
        public UnityEngine.Events.UnityAction<Character>OnCharacterEnter;

        public void Init()
        {

        }
        public void Clear()
        {
            this.Characters.Clear();
        }
        public void Dispose()
        {
            
        }

        public void AddCharacter(SkillBridge.Message.NCharacterInfo cha)
        {
            Debug.LogFormat("加入角色姓名:{0},地图:{1}，实体信息:{2}",cha.Name,cha.mapId,cha.Entity.String());
            Character character=new Character(cha);
            this.Characters[cha.Id]=character;

            if(OnCharacterEnter!= null)
            {
                OnCharacterEnter(character);
            }
        }
        public void RemoveCharacter(int characterId)
        {
            Debug.LogFormat("移除角色ID：{0}",characterId);
            this.Characters.Remove(characterId);
        }
    }
}
