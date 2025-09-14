using Common;
using Common.Utils;
using GameServer.Entities;
using GameServer.Models;
using GameServer.Services;
using SkillBridge.Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameServer.Managers
{
    class GuildManager : Singleton<GuildManager>
    {
        public Dictionary<int, Guild> Guilds = new Dictionary<int, Guild>();
        private HashSet<string> GuildNames = new HashSet<string>();


        public void Init()
        {
            this.Guilds.Clear();
            foreach(var guild in DBService.Instance.Entities.Guilds)//从数据库中加载到字典中
            {
                this.AddGuild(new Guild(guild));
            }
        }

        private void AddGuild(Guild guild)
        {
            this.Guilds.Add(guild.Id, guild);
            this.GuildNames.Add(guild.Name);
            guild.changeTime = TimeUtil.timestamp;
        }

        public bool CheckNameExisted(string guildName)
        {
            return GuildNames.Contains(guildName);//哈希表高效查询
        }

        public bool CreateGuild(string name, string notice, Character leader)
        {
            DateTime now = DateTime.Now;
            TGuild dbGuild = DBService.Instance.Entities.Guilds.Create();
            dbGuild.Name = name;
            dbGuild.Notice = notice;
            dbGuild.LeaderID = leader.Id;
            dbGuild.LeaderName = leader.Name;
            dbGuild.CreateTime = now;
            DBService.Instance.Entities.Guilds.Add(dbGuild);
            DBService.Instance.Save();

            Guild guild = new Guild(dbGuild);//new一次model
            //把会长身份填进去
            guild.AddMember(leader.Id, leader.Name, leader.TCharacter.Class, leader.TCharacter.Level, GuildTitle.President);
            leader.Guild = guild;
            DBService.Instance.Save();
            leader.TCharacter.GuildId = dbGuild.Id;
            DBService.Instance.Save();
            this.AddGuild(guild);
            return true;
        }

        internal Guild GetGuild(int guildId)//根据公会ID获取公会信息
        {
            if (guildId == 0)
            {
                return null;
            }
            Guild guild = null;
            this.Guilds.TryGetValue(guildId, out guild);
            return guild;
        }

        internal List<NGuildInfo> GetGuildsInfo()
        {
            List<NGuildInfo> result = new List<NGuildInfo>();
            foreach(var kv in this.Guilds)
            {
                result.Add(kv.Value.GuildInfo(null));//加入公会不需要传来源 只包含基础信息
            }
            return result;
        }
    }
}
