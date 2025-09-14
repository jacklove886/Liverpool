using Common;
using GameServer.Entities;
using GameServer.Models;
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

        public void Init()
        {

        }

        internal Guild GetGuild(int id)
        {
            throw new NotImplementedException();
        }

        internal bool CheckNameExisted(string guildName)
        {
            throw new NotImplementedException();
        }

        internal void CreateGuild(string guildName, string guildNotice, Character character)
        {
            throw new NotImplementedException();
        }

        internal IEnumerable<NGuildInfo> GetGuildsInfo()
        {
            throw new NotImplementedException();
        }
    }
}
