using Common;
using GameServer.Entities;
using SkillBridge.Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameServer.Models
{
    class Guild
    {
        public TGuild Data;

        public int Id { get { return this.Data.Id; } }
        public Character Leader;

        public string Name { get { return this.Data.Name; } }

        public List<Character> Members = new List<Character>();

        public double timestamp;

        public Guild(TGuild Tguild)
        {
            this.Data = Tguild;
        }

    }
}
