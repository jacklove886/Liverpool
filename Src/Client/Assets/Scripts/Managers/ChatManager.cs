using Models;
using SkillBridge.Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Managers
{
    class ChatManager : Singleton<ChatManager>
    {
        public Action Onchat { get; internal set; }

        public int PrivateID=0;//私聊对象的ID和名字
        public string PrivateName="";

        public List<ChatMessage> Messages = new List<ChatMessage>();

        public LocalChannel sendChannel= LocalChannel.ALL;

        public LocalChannel displayChannel;

        public enum LocalChannel//枚举
        {
            ALL=0,//所有
            Local=1,//本地
            World=2,//世界
            Team=3,//队伍
            Guild=4,//公会
            Private=5,//私聊
        }

        private ChatChannel[] ChannelFilter = new ChatChannel[6]
        {
            //All[0]显示所有
            ChatChannel.Local|ChatChannel.World|ChatChannel.Team|ChatChannel.Guild|ChatChannel.Private,
            ChatChannel.Local,
            ChatChannel.World,
            ChatChannel.Team,
            ChatChannel.Guild,
            ChatChannel.Private
        };

        public void StartPrivateChat(int targetId, string targetName)
        {
            this.PrivateID = targetId;
            this.PrivateName = targetName;

            this.sendChannel = LocalChannel.Private;//强制变成私聊频道
            if (Onchat != null)
            {
                Onchat();
            }
        }

        public void SendChat(string text)//发送信息到公屏
        {
            this.Messages.Add(new ChatMessage()
            {
                Channel = ChatChannel.Local,
                Message = text,
                FromId = User.Instance.CurrentCharacter.Id,
                FromName = User.Instance.CurrentCharacter.Name,
            });
        }

        public bool SetSendChannel(LocalChannel channel)
        {
            if (channel == LocalChannel.Team)
            {
                if (User.Instance.TeamInfo == null)
                {
                    this.AddSystemMessage("你没有加入任何队伍");
                    return false;
                }
            }

            if (channel == LocalChannel.Guild)
            {
                if (User.Instance.CurrentCharacter.Guild == null)
                {
                    this.AddSystemMessage("你没有加入任何公会");
                    return false;
                }
            }
            this.sendChannel = channel;
            Debug.LogFormat("Set Channel:{0}", this.sendChannel);
            return true;
        }

        private void AddSystemMessage(string message,string fromName="")
        {
            Messages.Add(new ChatMessage()
            {
                Channel = ChatChannel.System,
                Message = message,
                FromName = fromName
            });
            if (Onchat != null)
            {
                Onchat();
            }
        }

        public string GetCurrentMessages()
        {
            StringBuilder sb = new StringBuilder();
            foreach(var message in Messages)//遍历所有message
            {
                sb.AppendLine(FormatMessage(message));//格式化message
            }
            return sb.ToString();
        }

        private string FormatMessage(ChatMessage message)
        {
            switch (message.Channel)
            {
                case ChatChannel.Local:
                    return string.Format("<color=white>[本地]{0}{1}</color>", FormatFromPlayer(message), message.Message);
                case ChatChannel.World:
                    return string.Format("<color=cyan>[世界]{0}{1}</color>", FormatFromPlayer(message), message.Message);
                case ChatChannel.System:
                    return string.Format("<color=yellow>[系统]{0}</color>", message.Message);
                case ChatChannel.Private:
                    return string.Format("<color=magenta>[私聊]{0}{1}</color>", FormatFromPlayer(message), message.Message);
                case ChatChannel.Team:
                    return string.Format("<color=green>[队伍]{0}{1}</color>", FormatFromPlayer(message), message.Message);
                case ChatChannel.Guild:
                    return string.Format("<color=blue>[公会]{0}{1}</color>", FormatFromPlayer(message), message.Message);
            }
            return "";
        }

        //发送的玩家
        private string FormatFromPlayer(ChatMessage message)
        {
            if (message.FromId == User.Instance.CurrentCharacter.Id)//自己发的消息
            {
                return "<a name=\"\" class=\"player\">[我]</a>";
            }
            else
            {
                return string.Format("<a name=\"c:{0}:{1}\" class=\"player\">[{1}]</a>", message.FromId,message.FromName);
            }
        }
    }
}
