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

        public System.Action OnGuildClose;//关闭公会页面

        public System.Action OnGuildListClose;//关闭公会页面

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
            MessageDistributer.Instance.Subscribe<GuildAdminResponse>(this.OnGuildAdmin);
        }

        public void Dispose()
        {
            MessageDistributer.Instance.Unsubscribe<GuildCreateResponse>(this.OnGuildCreate);
            MessageDistributer.Instance.Unsubscribe<GuildListResponse>(this.OnGuildList);
            MessageDistributer.Instance.Unsubscribe<GuildJoinRequest>(this.OnGuildJoinRequest);
            MessageDistributer.Instance.Unsubscribe<GuildJoinResponse>(this.OnGuildJoinResponse);
            MessageDistributer.Instance.Unsubscribe<GuildResponse>(this.OnGuild);
            MessageDistributer.Instance.Unsubscribe<GuildLeaveResponse>(this.OnGuildLeave);
            MessageDistributer.Instance.Unsubscribe<GuildAdminResponse>(this.OnGuildAdmin);
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
                MessageBox.Show(string.Format("创建{0}公会成功", response.Guild.GuildName), "公会");
            }
            else
            {
                MessageBox.Show(response.Msg, "公会");
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
            if (response.Result == Result.Success&&response.Apply.Result==ApplyResult.Accept)
            {
                MessageBox.Show(response.Msg, "申请通过");
                if (OnGuildListClose != null)
                {
                    OnGuildListClose();
                }
            }
            else if (response.Result == Result.Failed && response.Apply.Result == ApplyResult.Reject)
            {
                MessageBox.Show(response.Msg, "申请失败", MessageBoxType.Error);
            }
            else
            {
                MessageBox.Show(response.Msg, "等待结果");
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
        public void SendGuildLeaveRequest(int guildId,int characterId)
        {
            Debug.LogFormat("发送离开公会的请求");
            NetMessage message = new NetMessage();
            message.Request = new NetMessageRequest();
            message.Request.guildLeave = new GuildLeaveRequest();
            message.Request.guildLeave.GuildId = guildId;
            message.Request.guildLeave.characterId = characterId;
            NetClient.Instance.SendMessage(message);
        }

        private void OnGuildLeave(object sender, GuildLeaveResponse response)
        {
            if (response.Result == Result.Success)
            {
                GuildManager.Instance.Init(null);
                if(response.Msg!=null)
                MessageBox.Show(response.Msg, "公会");
                if (OnGuildClose != null)
                {
                    OnGuildClose();
                }
            }
            else if (response.Result == Result.Failed)
            {
                MessageBox.Show(response.Msg, "公会",MessageBoxType.Error);
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

        //处理离线申请
        public void SendGuildJoinApply(bool accept,NGuildApplyInfo apply)
        {
            Debug.LogFormat("SendGuildJoinApply处理离线申请");
            NetMessage message = new NetMessage();
            message.Request = new NetMessageRequest();
            message.Request.guildJoinResponse = new GuildJoinResponse();
            message.Request.guildJoinResponse.Result = Result.Success;
            message.Request.guildJoinResponse.Apply = apply;
            message.Request.guildJoinResponse.Apply.Result =accept?ApplyResult.Accept: ApplyResult.Reject;
            NetClient.Instance.SendMessage(message);
        }


        public void SendGuildAdmin(GuildAdminCommand command, int characterId)
        {
            Debug.Log("OnGuildAdmin请求");
            NetMessage message = new NetMessage();
            message.Request = new NetMessageRequest();
            message.Request.guildAdmin = new GuildAdminRequest();
            message.Request.guildAdmin.Command = command;
            message.Request.guildAdmin.Target = characterId;
            NetClient.Instance.SendMessage(message);
        }

        private void OnGuildAdmin(object sender, GuildAdminResponse response)
        {
            Debug.Log("OnGuildAdmin响应");
            switch (response.commandRequest.Command)
            {
                case GuildAdminCommand.Kickout:
                    if (response.Result == Result.Success)
                    {
                        MessageBox.Show(response.Msg, "踢出成功");
                        if (OnGuildClose != null && GuildManager.Instance.myMemberInfo == null)
                        {
                            OnGuildClose();
                        }
                    }
                    else
                    {
                        MessageBox.Show(response.Msg, "踢出失败", MessageBoxType.Error);
                    }
                    break;
                case GuildAdminCommand.Promote:
                    if (response.Result == Result.Success)
                    {
                        MessageBox.Show(response.Msg, "晋升成功");
                    }
                    else
                    {
                        MessageBox.Show(response.Msg, "晋升失败", MessageBoxType.Error);
                    }
                    break;
                case GuildAdminCommand.Depose:
                    if (response.Result == Result.Success)
                    {
                        MessageBox.Show(response.Msg, "罢免成功");
                    }
                    else
                    {
                        MessageBox.Show(response.Msg, "罢免失败", MessageBoxType.Error);
                    }
                    break;
                case GuildAdminCommand.Transfer:
                    if (response.Result == Result.Success)
                    {
                        MessageBox.Show(response.Msg, "转让成功");
                    }
                    else
                    {
                        MessageBox.Show(response.Msg, "转让失败", MessageBoxType.Error);
                    }
                    break;
            }          
        }
    }
}
