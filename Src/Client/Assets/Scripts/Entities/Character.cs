using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SkillBridge.Message;
using UnityEngine;

namespace Entities
{
    public class Character : Entity
    {
        [Header("角色信息")]
        public NCharacterInfo Info;
        public Common.Data.CharacterDefine Define;

        public string Name
        {
            get
            {
                if (this.Info.Type == CharacterType.Player)
                    return this.Info.Name;
                else
                    return this.Define.Name;
            }
        }

        public bool IsPlayer
        {
            get { return this.Info.Id == Models.User.Instance.CurrentCharacter.Id; }
        }

        public Character(NCharacterInfo info) : base(info.Entity)
        {
            this.Info = info;
            this.Define = DataManager.Instance.Characters[info.Tid];
        }

        public void Move()
        {
            this.speed = this.Define.Speed;
        }


        public void Stop()
        {
            this.speed = 0;
        }

        public void SetDirection(Vector3Int direction)
        {
            this.direction = direction;
        }

        public void SetPosition(Vector3Int position)
        {
            this.position = position;
        }
    }
}
