using System;
using System.Collections.Generic;
using UnityEngine;          
using Entities;
using SkillBridge.Message;

namespace Managers
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
            Debug.LogFormat("加入角色姓名:{0},地图:{1}",cha.Name,cha.mapId);
            Character character=new Character(cha);
            this.Characters[cha.Id]=character;

            //这句话永远不会执行
            if (OnCharacterEnter!= null)
            {
                Debug.Log("操你妈6");
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
