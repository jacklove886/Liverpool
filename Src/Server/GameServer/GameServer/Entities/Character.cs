using Common.Data;
using GameServer.Core;
using GameServer.Managers;
using SkillBridge.Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameServer.Entities
{
    public class Character : CharacterBase
    {
       
        public TCharacter Data;

        public ItemManager ItemManager;
        

        public Character(CharacterType type,TCharacter cha)://构造函数
            base(new Core.Vector3Int(cha.MapPosX, cha.MapPosY, cha.MapPosZ),new Core.Vector3Int(100,0,0))
        {

            Data = cha;
            Info = new NCharacterInfo();
            Info.Type = type;
            Info.Id = cha.ID;
            Info.Name = cha.Name;
            Info.Level = cha.Level;
            Info.Tid = cha.TID;
            Info.Class = (CharacterClass)cha.Class;
            Info.mapId = cha.MapID;
            Info.Entity = this.EntityData;
            Define = DataManager.Instance.Characters[this.Info.Tid];

            ItemManager = new ItemManager(this);
            ItemManager.GetItemInfos(Info.Items);
        }
    }
}
