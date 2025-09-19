using Managers;
using Models;
using Network;
using SkillBridge.Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Services
{
    class TeamService:Singleton<TeamService>
    {
        public TeamService()//构造函数
        {
            MessageDistributer.Instance.Subscribe<TeamInviteRequest>(this.OnTeamInviteRequest);
            MessageDistributer.Instance.Subscribe<TeamInviteResponse>(this.OnTeamInviteResponse);
            MessageDistributer.Instance.Subscribe<TeamInfoResponse>(this.OnTeamInfo);
            MessageDistributer.Instance.Subscribe<TeamLeaveResponse>(this.OnTeamLeave);
        }

        public void Dispose()
        {
            MessageDistributer.Instance.Unsubscribe<TeamInviteRequest>(this.OnTeamInviteRequest);
            MessageDistributer.Instance.Unsubscribe<TeamInviteResponse>(this.OnTeamInviteResponse);
            MessageDistributer.Instance.Unsubscribe<TeamInfoResponse>(this.OnTeamInfo);
            MessageDistributer.Instance.Unsubscribe<TeamLeaveResponse>(this.OnTeamLeave);
        }

        public void Init()
        {

        }

        public void SendTeamInviteRequest(int friendId, string friendName)//A向B发送邀请好友请求
        {
            Debug.LogFormat("发送邀请好友请求");
            NetMessage message = new NetMessage();
            message.Request = new NetMessageRequest();
            message.Request.teamInviteRequest = new TeamInviteRequest();
            message.Request.teamInviteRequest.FromId = User.Instance.CurrentCharacter.Id;
            message.Request.teamInviteRequest.FromName = User.Instance.CurrentCharacter.Name;
            message.Request.teamInviteRequest.ToId = friendId;
            message.Request.teamInviteRequest.ToName = friendName;
            NetClient.Instance.SendMessage(message);
        }

        //接收到A玩家发送邀请B组队的请求   返回一个接受或拒绝的结果 并把请求原封不动返回服务器
        public void OnTeamInviteRequest(object sender, TeamInviteRequest request)
        {
            var confirm = MessageBox.Show(string.Format("{0}邀请你加入队伍", request.FromName), "组队邀请", MessageBoxType.Confirm, "同意", "拒绝");
            confirm.OnYes = () =>
            {
                SendTeamInviteResponse(true, request);
            };
            confirm.OnNo = () =>
            {
                SendTeamInviteResponse(false, request);
            };
        }

        public void SendTeamInviteResponse(bool accept, TeamInviteRequest request)//对方发来的请求
        {
            Debug.Log("SendTeamInviteResponse");
            NetMessage message = new NetMessage();
            message.Request = new NetMessageRequest();
            message.Request.teamInviteResponse = new TeamInviteResponse();
            message.Request.teamInviteResponse.Result = accept ? Result.Success : Result.Failed;
            message.Request.teamInviteResponse.Msg = accept ? "对方同意了你的邀请" : "对方拒绝了你的组队请求";//A收到的
            message.Request.teamInviteResponse.Request = request;
            NetClient.Instance.SendMessage(message);
        }

        public void OnTeamInviteResponse(object sender, TeamInviteResponse response)
        {
            if (response.Result == Result.Success)
            {
                MessageBox.Show(response.Msg, "组队成功");
            }
            else if (response.Result == Result.Failed)
            {
                MessageBox.Show(response.Msg, "组队失败");
            }
        }

        public void OnTeamInfo(object sender, TeamInfoResponse response)
        {
            Debug.LogFormat("收到组队更新响应");
            TeamManager.Instance.UpdateTeamInfo(response.Team);
        }


        public void SendTeamLeaveRequest(int id)//发送离开组队请求
        {
            Debug.LogFormat("发送离开组队请求");
            NetMessage message = new NetMessage();
            message.Request = new NetMessageRequest();
            message.Request.teamLeave = new TeamLeaveRequest();
            message.Request.teamLeave.TeamId = User.Instance.TeamInfo.Id;
            message.Request.teamLeave.characterId = User.Instance.CurrentCharacter.Id;
            NetClient.Instance.SendMessage(message);
        }

        public void OnTeamLeave(object sender, TeamLeaveResponse response)
        {
            if (response.Result == Result.Success)
            {
                MessageBox.Show("退出成功", "退出队伍");
                TeamManager.Instance.UpdateTeamInfo(null);
            }
            else if (response.Result == Result.Failed)
            {
                MessageBox.Show("退出失败", "退出队伍", MessageBoxType.Error);
            }
        }

    }
}
