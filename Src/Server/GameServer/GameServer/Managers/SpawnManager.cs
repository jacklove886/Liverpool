using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SkillBridge.Message;

using Common;
using Common.Data;

using Network;
using GameServer.Managers;
using GameServer.Entities;
using GameServer.Services;
using GameServer.Models;

namespace GameServer.Managers
{
    public class SpawnManager//只管理刷怪的规则
    {
        private List<Spawner> Rules = new List<Spawner>();

        private Map Map;

        public void Init(Map map)
        {
            this.Map = map;
            if (DataManager.Instance.SpawnRules.ContainsKey(map.Define.ID))
            {
                //读取当前地图的刷怪规则表
                foreach (var define in DataManager.Instance.SpawnRules[map.Define.ID].Values)
                {
                    //创建刷怪器  传入参数 规则以及地图
                    this.Rules.Add(new Spawner(define, this.Map));
                }
            }
        }

        public void Update()
        {
            if (Rules.Count == 0)//没有规则
            {
                return;
            }
            for(int i = 0; i < this.Rules.Count; i ++)
            {
                this.Rules[i].Update();
            }
        }
    }
}