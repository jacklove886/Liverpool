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
    public class Guild
    {
        public TGuild Data;

        public int Id { get { return this.Data.Id; } }
        public Character Leader;

        public string Name { get { return this.Data.Name; } }

        public List<Character> Members = new List<Character>();

        public int changeTime;

        public Guild(TGuild Tguild)//构造函数
        {
            this.Data = Tguild;
        }

        //加入公会申请
        public bool JoinApply()
        {
            return true;
        }

        internal NGuildInfo GuildInfo(Character character)
        {
            throw new NotImplementedException();
        }

        internal void PostProcess(Character character, NetMessageResponse message)
        {
            throw new NotImplementedException();
        }
    }
}
