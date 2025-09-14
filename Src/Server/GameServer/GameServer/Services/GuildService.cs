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
            MessageDistributer<NetConnection<NetSession>>.Instance.Subscribe<GuildJoinRequest>(this.OnGuildJoinRequest);
            MessageDistributer<NetConnection<NetSession>>.Instance.Subscribe<GuildJoinResponse>(this.OnGuildJoinResponse);
            MessageDistributer<NetConnection<NetSession>>.Instance.Subscribe<GuildListRequest>(this.OnGuildGuildList);
            MessageDistributer<NetConnection<NetSession>>.Instance.Subscribe<GuildLeaveRequest>(this.OnGuildLeave);
            MessageDistributer<NetConnection<NetSession>>.Instance.Subscribe<GuildAdminRequest>(this.OnGuildAdmin);
        }

        public void Init()
        {
            GuildManager.Instance.Init();
        }

        private void OnGuildCreateRequest(NetConnection<NetSession> sender, GuildCreateRequest request)
        {
            Character character = sender.Session.Character;
            Log.InfoFormat("收到创建公会请求:公会名称:{0},角色:[{1},{2}]", request.GuildName,character.Id,character.Info.Name);
            sender.Session.Response.guildCreate = new GuildCreateResponse();
            if (character.Guild != null)//已经有公会
            {
                sender.Session.Response.guildCreate = new GuildCreateResponse();
                sender.Session.Response.guildCreate.Result = Result.Failed;
                sender.Session.Response.guildCreate.Msg = "您已经有公会了";
                sender.SendResponse();
                return;
            }
            if (GuildManager.Instance.CheckNameExisted(request.GuildName))//已经有队伍
            {
                sender.Session.Response.guildCreate.Result = Result.Failed;
                sender.Session.Response.guildCreate.Msg = "公会名称已经存在";
                sender.SendResponse();
                return;
            }     
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

        //加入请求
        private void OnGuildJoinRequest(NetConnection<NetSession> sender, GuildJoinRequest request)
        {
            Character character = sender.Session.Character;
            Log.InfoFormat("加入公会请求:公会:{0}角色:[{1},{2}]",request.Apply.GuildId,character.Id, character.Info.Name);
            var guild = GuildManager.Instance.GetGuild(request.Apply.GuildId);
            if (guild == null)
            {
                sender.Session.Response.guildJoinResponse = new GuildJoinResponse();
                sender.Session.Response.guildJoinResponse.Result = Result.Failed;
                sender.Session.Response.guildJoinResponse.Msg = "公会不存在";
                sender.SendResponse();
                return;
            }
            request.Apply.characterId = character.TCharacter.ID;
            request.Apply.Name = character.TCharacter.Name;
            request.Apply.Class = character.TCharacter.Class;
            request.Apply.Level = character.TCharacter.Level;


            if (guild.JoinApply(request.Apply))
            {
                var leader = SessionManager.Instance.GetSession(guild.Data.LeaderID);
                if (leader != null)//会长在线
                {
                    //给会长发申请加入请求
                    leader.Session.Response.guildJoinRequest = request;
                    leader.SendResponse();
                }
            }
            else
            {
                sender.Session.Response.guildJoinResponse = new GuildJoinResponse();
                sender.Session.Response.guildJoinResponse.Result = Result.Failed;
                sender.Session.Response.guildJoinResponse.Msg = "请勿重复申请";
                sender.SendResponse();
                return;
            }
        }

        //审批
        private void OnGuildJoinResponse(NetConnection<NetSession> sender, GuildJoinResponse response)
        {
            Character character = sender.Session.Character;
            Log.InfoFormat("加入公会响应:公会:{0}角色:[{1},{2}]", response.Apply.GuildId, character.Id, character.Info.Name);
            var guild = GuildManager.Instance.GetGuild(response.Apply.GuildId);
            if (response.Result==Result.Success)//收到网络请求
            {
                guild.JoinAppove(response.Apply);
            }
            var requester = SessionManager.Instance.GetSession(response.Apply.characterId);
            if (requester != null&&response.Apply.Result==ApplyResult.Accept)
            {
                requester.Session.Character.Guild = guild;
                requester.Session.Response.guildJoinResponse = new GuildJoinResponse();
                requester.Session.Response.guildJoinResponse.Result = Result.Success;
                requester.Session.Response.guildJoinResponse.Msg = "加入公会成功";
                requester.SendResponse();

                sender.Session.Response.guildJoinResponse = new GuildJoinResponse();
                sender.Session.Response.guildJoinResponse.Result = Result.Success;
                sender.Session.Response.guildJoinResponse.Msg = character.Name+"审核通过";
            }
            else if(response.Apply.Result == ApplyResult.Reject)
            {
                requester.Session.Response.guildJoinResponse = new GuildJoinResponse();
                requester.Session.Response.guildJoinResponse.Result = Result.Failed;
                requester.Session.Response.guildJoinResponse.Msg = "申请被拒绝";
                requester.SendResponse();

                sender.Session.Response.guildJoinResponse = new GuildJoinResponse();
                sender.Session.Response.guildJoinResponse.Result = Result.Failed;
                sender.Session.Response.guildJoinResponse.Msg = character.Name + "批准不通过";
            }
            sender.SendResponse();
        }

        private void OnGuildLeave(NetConnection<NetSession> sender, GuildLeaveRequest request)
        {
            Character character = sender.Session.Character;
            Log.InfoFormat("离开公会请求:角色:[{0},{1}]", character.Id, character.Info.Name);
            sender.Session.Response.guildLeave = new GuildLeaveResponse ();
            character.Guild.Leave(character);
            sender.Session.Response.guildJoinResponse.Result = Result.Success;
            DBService.Instance.Save();
            sender.SendResponse();
        }

        private void OnGuildAdmin(NetConnection<NetSession> sender, GuildAdminRequest request)
        {
            Character character = sender.Session.Character;
            Log.InfoFormat("OnGuildAdmin:角色:[{0},{1}],操作:{2}", character.Id, character.Info.Name,request.Command);
            sender.Session.Response.guildAdmin = new GuildAdminResponse();
            if (character.Guild == null)
            {
                sender.Session.Response.guildAdmin = new GuildAdminResponse();
                sender.Session.Response.guildAdmin.Result = Result.Failed;
                sender.Session.Response.guildAdmin.Msg = "你没公会不要乱来";
                return;
            }
            TGuildMember memberData = character.Guild.GetDBMember(character.Id);
            if (memberData.Position == 0)
            {
                sender.Session.Response.guildAdmin = new GuildAdminResponse();
                sender.Session.Response.guildAdmin.Result = Result.Failed;
                sender.Session.Response.guildAdmin.Msg = "你没有资格进行操作";
                return;
            }
            character.Guild.ExcuteAdmin(request.Command, request.Target, character.Id);
            var target = SessionManager.Instance.GetSession(request.Target);//查找删除的角色
            if (target != null)
            {
                target.Session.Response.guildAdmin = new GuildAdminResponse();
                target.Session.Response.guildAdmin.Result = Result.Success;
                target.Session.Response.guildAdmin.Msg = "你的公会申请被审核了";
                target.Session.Response.guildAdmin.commandRequest = request;
                target.SendResponse();
            }
            sender.Session.Response.guildAdmin.Result = Result.Success;
            sender.Session.Response.guildAdmin.Msg = "操作完成";
            sender.Session.Response.guildAdmin.commandRequest = request;
            sender.SendResponse();
        }
    }
}
