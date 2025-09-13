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
    class TeamService: Singleton<TeamService>
    {
        public TeamService()//构造函数
        {
            MessageDistributer<NetConnection<NetSession>>.Instance.Subscribe<TeamInviteRequest>(this.OnTeamInviteRequest);
            MessageDistributer<NetConnection<NetSession>>.Instance.Subscribe<TeamInviteResponse>(this.OnTeamInviteResponse);
            MessageDistributer<NetConnection<NetSession>>.Instance.Subscribe<TeamLeaveRequest>(this.OnTeamLeave);
        }

        public void Init()
        {
            TeamManager.Instance.Init();
        }

        private void OnTeamInviteRequest(NetConnection<NetSession> sender, TeamInviteRequest request)
        {
            //character代表A和sender代表A  
            Character character = sender.Session.Character;
            Log.InfoFormat("收到邀请好友请求:From角色ID:{0},Name:{1},To角色ID:{2},Name:{3}", request.FromId, request.FromName, request.ToId, request.ToName);

            //target代表B
            NetConnection<NetSession> target = SessionManager.Instance.GetSession(request.ToId);
            if (target == null)//B不在线
            {
                sender.Session.Response.teamInviteResponse = new TeamInviteResponse();
                sender.Session.Response.teamInviteResponse.Result = Result.Failed;
                sender.Session.Response.teamInviteResponse.Errormsg = "好友不在线";
                sender.SendResponse();
                return;
            }         
            if (target.Session.Character.Team!=null)//已经有队伍
            {
                sender.Session.Response.teamInviteResponse = new TeamInviteResponse();
                sender.Session.Response.teamInviteResponse.Result = Result.Failed;
                sender.Session.Response.teamInviteResponse.Errormsg = "对方已经有队伍";
                sender.SendResponse();
                return;
            }
            if (character.Team != null && character.Team.Leader!=character)
            {
                sender.Session.Response.teamInviteResponse = new TeamInviteResponse();
                sender.Session.Response.teamInviteResponse.Result = Result.Failed;
                sender.Session.Response.teamInviteResponse.Errormsg = "只有队长可以邀请队员";
                sender.SendResponse();
                return;
            }
            if (character.Team != null&&character.Team.members.Count >= 5)
            {
                sender.Session.Response.teamInviteResponse = new TeamInviteResponse();
                sender.Session.Response.teamInviteResponse.Result = Result.Failed;
                sender.Session.Response.teamInviteResponse.Errormsg = "您的队伍已满";
                sender.SendResponse();
                return;
            }
            //B转发请求         
            target.Session.Response.teamInviteRequest = request;
            target.SendResponse();
        }

        //接收到B玩家对A玩家邀请请求的结果响应(接收到B是接受了还是拒绝了)
        private void OnTeamInviteResponse(NetConnection<NetSession> sender, TeamInviteResponse response)
        {
            //character和sender代表B
            Character character = sender.Session.Character;
            Log.InfoFormat("OnFriendAddResponse:角色:{0},结果:{1},FromID:{2},FromID:{3}", character, response.Result, response.Request.FromId, response.Request.ToId);
            sender.Session.Response.teamInviteResponse = response;
            if (response.Result == Result.Success)//B接受了
            {
                //requester代表A
                var requster = SessionManager.Instance.GetSession(response.Request.FromId);
                if (requster == null)
                {
                    sender.Session.Response.teamInviteResponse.Result = Result.Failed;
                    sender.Session.Response.teamInviteResponse.Errormsg = "请求者已离线";
                }
                else
                {
                    //第一个参数是A 第二个参数是B
                    TeamManager.Instance.AddTeamMember(requster.Session.Character, character);
                    requster.Session.Response.teamInviteResponse = response;
                    requster.Session.Response.teamInviteResponse.Result = Result.Success;
                    sender.Session.Response.teamInviteResponse.Errormsg = "组队成功";
                    requster.SendResponse();
                }
            }
            sender.SendResponse();
        }

        private void OnTeamLeave(NetConnection<NetSession> sender, TeamLeaveRequest request)
        {
            Character character = sender.Session.Character;
            Log.InfoFormat("OnTeamLeave:角色ID:{0},TeamID:{1}:{2}", character.Id,request.TeamId,request.characterId);
            sender.Session.Response.teamLeave = new TeamLeaveResponse();
            sender.Session.Response.teamLeave.Result = Result.Success;
            sender.Session.Response.teamLeave.characterId = request.characterId;
            character.Team.Leave(character);
            sender.SendResponse();
        }
    }
}
