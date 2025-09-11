using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SkillBridge.Message;

using Network;
using GameServer.Managers;
using GameServer.Entities;
using GameServer.Services;
using GameServer.Models;

namespace GameServer.Managers
{
    public class MonsterManager
    {
        private Map Map;
        public Dictionary<int, Monster> Monsters = new Dictionary<int, Monster>();

        public void Init(Map map)
        {
            this.Map = map;
        }

        //创建怪物实例
        internal Monster Create(int spawnMonID,int spawnLevel,NVector3 position,NVector3 direction)
        {
            Monster monster = new Monster(spawnMonID, spawnLevel, position, direction);
            EntityManager.Instance.AddEntity(this.Map.ID, monster);
            monster.Info.EntityId = monster.entityId;
            monster.Info.mapId = this.Map.ID;
            Monsters[monster.Id] = monster;

            this.Map.MonsterEnter(monster);
            return monster;
        }
    }
}