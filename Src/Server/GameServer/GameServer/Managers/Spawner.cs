using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SkillBridge.Message;

using Network;
using GameServer.Managers;
using GameServer.Entities;
using GameServer.Services;
using GameServer.Models;
using Common.Data;
using Common;

namespace GameServer.Managers
{
    class Spawner
    {
        public SpawnRuleDefine Define { get; set; }

        private Map Map;

        private float spawnTime = 0;//刷怪时间

        private float unspawnTime = 0;//上次死亡时间

        private bool spawned = false;//已经刷出怪物
        private SpawnPointDefine spawnPoint = null;//刷怪点配置信息

        public Spawner(SpawnRuleDefine define, Map map)//构造函数
        {
            this.Define = define;
            this.Map = map;

            //当前地图存在
            if (DataManager.Instance.SpawnPoints.ContainsKey(this.Map.ID))
            {
                //当前刷怪点存在
                if (DataManager.Instance.SpawnPoints[this.Map.ID].ContainsKey(this.Define.SpawnPoint))
                {
                    spawnPoint = DataManager.Instance.SpawnPoints[this.Map.ID][this.Define.SpawnPoint];
                }
                else
                {
                    Log.ErrorFormat("刷怪规则[{0}],刷怪点[{1}]不存在", this.Define.ID, this.Define.SpawnPoint);
                }
            }
        }

        public void Update()
        {
            if (CanSpawn())
            {
                Spawn();
            }
        }

        bool CanSpawn()
        {
            if (spawned)//已经刷出怪物
            {
                return false;
            }
            //上次死亡时间+间隔时间大于当前游戏时间  即刷怪冷却还没到
            if (unspawnTime + Define.SpawnPeriod > Time.time)
            {
                return false;
            }
            return true;
        }

        public void Spawn()//刷怪逻辑
        {
            spawned = true;
            Log.InfoFormat("地图:[{0}]刷怪规则:[{1}]怪物:[{2}]等级:[{3}]刷怪点:[{4}]",this.Define.MapID,this.Define.ID,this.Define.SpawnMonID,this.Define.SpawnLevel,this.Define.SpawnPoint);
            //调用MonsterManager创建方法
            Map.MonsterManager.Create(this.Define.SpawnMonID, this.Define.SpawnLevel, this.spawnPoint.Position, this.spawnPoint.Direction);
        }
    }
}