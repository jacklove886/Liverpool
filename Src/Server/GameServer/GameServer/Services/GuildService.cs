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
            GuildManager.Instance.Init();
        }

        private void OnGuildCreateRequest(NetConnection<NetSession> sender, GuildCreateRequest request)
        {
            //character代表A和sender代表A  
            Character character = sender.Session.Character;
            Log.InfoFormat("收到创建公会请求:公会名称:{0},角色:[{1},{2}]", request.GuildName,character.Id,character.Info.Name);
            sender.Session.Response.guildCreate = new GuildCreateResponse();
            if (character.Guild != null)//已经有公会
            {
                sender.Session.Response.guildCreate = new GuildCreateResponse();
                sender.Session.Response.guildCreate.Result = Result.Failed;
                sender.Session.Response.guildCreate.Errormsg = "您已经有公会了";
                sender.SendResponse();
                return;
            }
            if (GuildManager.Instance.CheckNameExisted(request.GuildName))//已经有队伍
            {
                sender.Session.Response.guildCreate.Result = Result.Failed;
                sender.Session.Response.guildCreate.Errormsg = "公会名称已经存在";
                sender.SendResponse();
                return;
            }
            //B转发请求         
            GuildManager.Instance.CreateGuild(request.GuildName, request.GuildNotice, character);
            sender.Session.Response.guildCreate.Guild = character.Guild.GuildInfo(character);
            sender.Session.Response.guildCreate.Result = Result.Success;
            sender.SendResponse();
        }

        private void OnGuildGuildList(NetConnection<NetSession> sender, GuildListRequest request)
        {
            Character character = sender.Session.Character;
            Log.InfoFormat("公会列表请求:角色:[{0},{1}]",character.Id, character.Info.Name);

            sender.Session.Response.guildList = new GuildListResponse();
            sender.Session.Response.guildList.Guilds.AddRange(GuildManager.Instance.GetGuildsInfo());
            sender.Session.Response.guildList.Result = Result.Success;
            sender.SendResponse();
        }

        private void OnGuildJoin(NetConnection<NetSession> sender, GuildJoinRequest request)
        {
            Character character = sender.Session.Character;
            Log.InfoFormat("加入公会请求:公会:{0}角色:[{1},{2}]",request.Apply.GuildId,character.Id, character.Info.Name);
            var guild = GuildManager.Instance.GetGuild(request.Apply.GuildId);
            if (guild == null)
            {
                sender.Session.Response.guildJoinResponse = new GuildJoinResponse();
                sender.Session.Response.guildJoinResponse.Result = Result.Failed;
                sender.Session.Response.guildJoinResponse.Errormsg = "公会不存在";
                sender.SendResponse();
                return;
            }
            request.Apply.characterId = character.TCharacter.ID;
            request.Apply.Name = character.TCharacter.Name;
            request.Apply.Class = character.TCharacter.Class;
            request.Apply.Level = character.TCharacter.Level;

        }

        private void OnGuildCreateResponse(NetConnection<NetSession> sender, GuildCreateResponse response)
        {

        }

        

          

        

        private void OnGuild(NetConnection<NetSession> sender, GuildRequest request)
        {
            
        }

        

        private void OnGuildLeave(NetConnection<NetSession> sender, GuildLeaveRequest request)
        {
           
        }

        
    }
}
