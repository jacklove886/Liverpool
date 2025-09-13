using Common;
using GameServer.Entities;
using GameServer.Managers;
using Network;
using SkillBridge.Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameServer.Services
{
    class GuildService : Singleton<GuildService>
    {
        public GuildService()//构造函数
        {
            MessageDistributer<NetConnection<NetSession>>.Instance.Subscribe<GuildCreateRequest>(this.OnGuildCreateRequest);
            MessageDistributer<NetConnection<NetSession>>.Instance.Subscribe<GuildCreateResponse>(this.OnGuildCreateResponse);
            MessageDistributer<NetConnection<NetSession>>.Instance.Subscribe<GuildJoinRequest>(this.OnGuildJoin);
            MessageDistributer<NetConnection<NetSession>>.Instance.Subscribe<GuildRequest>(this.OnGuild);
            MessageDistributer<NetConnection<NetSession>>.Instance.Subscribe<GuildListRequest>(this.OnGuildGuildList);
            MessageDistributer<NetConnection<NetSession>>.Instance.Subscribe<GuildLeaveRequest>(this.OnGuildLeave);
        }

        public void Init()
        {
            TeamManager.Instance.Init();
        }

        private void OnGuildCreateRequest(NetConnection<NetSession> sender, GuildCreateRequest request)
        {
            
        }

        private void OnGuildCreateResponse(NetConnection<NetSession> sender, GuildCreateResponse response)
        {
            
        }     

        private void OnGuildJoin(NetConnection<NetSession> sender, GuildJoinRequest request)
        {
            
        }

        private void OnGuild(NetConnection<NetSession> sender, GuildRequest request)
        {
            
        }

        private void OnGuildGuildList(NetConnection<NetSession> sender, GuildListRequest request)
        {
            
        }

        private void OnGuildLeave(NetConnection<NetSession> sender, GuildLeaveRequest request)
        {
           
        }

        
    }
}
