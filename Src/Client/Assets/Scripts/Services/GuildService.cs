using Managers;
using Network;
using SkillBridge.Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Services
{
    class GuildService : Singleton<GuildService>
    {
        public System.Action OnGuildUpdate;//公会更新事件

        public System.Action<bool> OnGuildCreateResult;//公会创建事件

        public System.Action<List<NGuildInfo>> OnGuildListResult;//公会列表更新事件

        public void Init()
        {

        }

        public GuildService()//构造函数
        {
            MessageDistributer.Instance.Subscribe<GuildCreateResponse>(this.OnGuildCreate);
            MessageDistributer.Instance.Subscribe<GuildListResponse>(this.OnGuildList);
            MessageDistributer.Instance.Subscribe<GuildJoinRequest>(this.OnGuildJoinRequest);
            MessageDistributer.Instance.Subscribe<GuildJoinResponse>(this.OnGuildJoinResponse);
            MessageDistributer.Instance.Subscribe<GuildResponse>(this.OnGuild);
            MessageDistributer.Instance.Subscribe<GuildLeaveResponse>(this.OnGuildLeave);
        }

        public void Dispose()
        {
            MessageDistributer.Instance.Unsubscribe<GuildCreateResponse>(this.OnGuildCreate);
            MessageDistributer.Instance.Unsubscribe<GuildListResponse>(this.OnGuildList);
            MessageDistributer.Instance.Unsubscribe<GuildJoinRequest>(this.OnGuildJoinRequest);
            MessageDistributer.Instance.Unsubscribe<GuildJoinResponse>(this.OnGuildJoinResponse);
            MessageDistributer.Instance.Unsubscribe<GuildResponse>(this.OnGuild);
            MessageDistributer.Instance.Unsubscribe<GuildLeaveResponse>(this.OnGuildLeave);
        }

        //创建公会请求
        public void SendGuildCreate(string guildName, string notice)
        {
            Debug.Log("发送创建公会请求");
            NetMessage message = new NetMessage();
            message.Request = new NetMessageRequest();
            message.Request.guildCreate = new GuildCreateRequest();
            message.Request.guildCreate.GuildName = guildName;
            message.Request.guildCreate.GuildNotice = notice;
            NetClient.Instance.SendMessage(message);
        }

        private void OnGuildCreate(object sender, GuildCreateResponse response)
        {
            Debug.LogFormat("收到创建公会响应:{0}", response.Result);
            if (OnGuildCreateResult != null)
            {
                OnGuildCreateResult(response.Result == Result.Success);
            }
            if(response.Result == Result.Success)
            {
                GuildManager.Instance.Init(response.Guild);
                MessageBox.Show(string.Format("{0}公会成功", response.Guild.GuildName), "公会");
            }
            else
            {
                MessageBox.Show(string.Format("{0}公会失败", response.Guild.GuildName), "公会");
            }
        }


        //发送加入公会请求
        public void SendGuildJoinRequest(int guildId)
        {
            Debug.LogFormat("发送加入公会请求");
            NetMessage message = new NetMessage();
            message.Request = new NetMessageRequest();
            message.Request.guildJoinRequest = new GuildJoinRequest();
            message.Request.guildJoinRequest.Apply = new NGuildApplyInfo();
            message.Request.guildJoinRequest.Apply.GuildId = guildId;
            NetClient.Instance.SendMessage(message);
        }

        internal void SendAdminCommand(GuildAdminCommand promote, int id)
        {
            throw new NotImplementedException();
        }

        private void OnGuildJoinRequest(object sender, GuildJoinRequest request)
        {
            var confirm = MessageBox.Show(string.Format("{0}申请加入公会", request.Apply.Name), "公会申请", MessageBoxType.Confirm, "同意", "拒绝");
            confirm.OnYes = () =>
            {
                SendGuildJoinResponse(true, request);
            };
            confirm.OnNo = () =>
            {
                SendGuildJoinResponse(false, request);
            };
        }

        //发送审批的结果
        public void SendGuildJoinResponse(bool accept,GuildJoinRequest request)
        {
            Debug.LogFormat("发送审批的结果");
            NetMessage message = new NetMessage();
            message.Request = new NetMessageRequest();
            message.Request.guildJoinResponse = new GuildJoinResponse();
            message.Request.guildJoinResponse.Result = Result.Success;
            message.Request.guildJoinResponse.Apply= request.Apply;
            message.Request.guildJoinResponse.Apply.Result = accept?ApplyResult.Accept: ApplyResult.Reject;
            NetClient.Instance.SendMessage(message);
        }

        //收到审批的结果
        private void OnGuildJoinResponse(object sender, GuildJoinResponse response)
        {
            Debug.LogFormat("收到审批的结果,{0}",response.Result);
            if (response.Result == Result.Success)
            {
                MessageBox.Show(response.Msg, "申请通过");
            }
            else if (response.Result == Result.Failed)
            {
                MessageBox.Show(response.Msg, "申请失败", MessageBoxType.Error);
            }
        }

        private void OnGuild(object sender, GuildResponse response)
        {
            Debug.LogFormat("OnGuild,[{0}][{1}][{2}]", response.Result,response.Guild.Id,response.Guild.GuildName);
            GuildManager.Instance.Init(response.Guild);
            if (OnGuildUpdate != null)//通知更新事件
            {
                OnGuildUpdate();
            }
        }

        //发送离开公会的请求
        public void SendGuildLeaveRequest()
        {
            Debug.LogFormat("发送离开公会的请求");
            NetMessage message = new NetMessage();
            message.Request = new NetMessageRequest();
            message.Request.guildLeave = new GuildLeaveRequest();
            NetClient.Instance.SendMessage(message);
        }

        private void OnGuildLeave(object sender, GuildLeaveResponse response)
        {
            if (response.Result == Result.Success)
            {
                GuildManager.Instance.Init(null);
                MessageBox.Show("离开公会成功", "公会");
            }
            else if (response.Result == Result.Failed)
            {
                MessageBox.Show("离开公会失败", "公会",MessageBoxType.Error);
            }
        }

        //发送公会列表的请求
        public void SendGuildListRequest()
        {
            Debug.LogFormat("发送公会列表的请求");
            NetMessage message = new NetMessage();
            message.Request = new NetMessageRequest();
            message.Request.guildList = new GuildListRequest();
            NetClient.Instance.SendMessage(message);
        }

        //收到列表刷新的请求
        private void OnGuildList(object sender, GuildListResponse response)
        {
            if (OnGuildListResult != null)
            {
                OnGuildListResult(response.Guilds);
            }
        }



       

       
    }
}
