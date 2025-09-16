using Models;
using Services;
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

        public int toId=0;//私聊对象的ID和名字
        public string toName="";

        public List<ChatMessage>[] Messages = new List<ChatMessage>[6]
        {
            new List<ChatMessage>(),
            new List<ChatMessage>(),
            new List<ChatMessage>(),
            new List<ChatMessage>(),
            new List<ChatMessage>(),
            new List<ChatMessage>(),
        };

        public LocalChannel sendChannel;
        public ChatChannel SendChannel//下拉框的频道
        {
            get
            {
                switch (sendChannel)
                {
                    case LocalChannel.Local:return ChatChannel.Local;
                    case LocalChannel.World: return ChatChannel.World;
                    case LocalChannel.Team: return ChatChannel.Team;
                    case LocalChannel.Guild: return ChatChannel.Guild;
                    case LocalChannel.Private: return ChatChannel.Private;
                    default: return ChatChannel.Local;
                }
            }
        }

        public LocalChannel displayChannel;//展示内容的频道

        public void Init()
        {
            foreach(var messages in Messages)
            {
                messages.Clear();
            }
        }

        public enum LocalChannel
        {
            ALL=0,//综合
            Local=1,//本地
            World=2,//世界
            Team=3,//队伍
            Guild=4,//公会
            Private=5,//私聊
        }

        //建立了一个长度为6 类型为ChatChannel的数组的过滤方法
        private ChatChannel[] ChannelFilter = new ChatChannel[6]
        {
            //All[0]显示所有  利用|操作符 操作Flags枚举
            ChatChannel.Local|ChatChannel.World|ChatChannel.Team|ChatChannel.Guild|ChatChannel.Private,
            ChatChannel.Local,
            ChatChannel.World,
            ChatChannel.Team,
            ChatChannel.Guild,
            ChatChannel.Private
        };
    

        //私聊
        public void StartPrivateChat(int targetId, string targetName)
        {
            this.toId = targetId;
            this.toName = targetName;

            this.sendChannel = LocalChannel.Private;//强制变成私聊频道
            if (Onchat != null)
            {
                Onchat();
            }
        }

        public void SendChat(string text)
        {
            ChatService.Instance.SendChat(this.SendChannel, text, toId, toName);
        }

        public void AddMessages(ChatChannel channel, List<ChatMessage> messages)
        {
            for (int ch = 0; ch < 6; ch++)
            {
                if ((this.ChannelFilter[ch] & channel) == channel)//flag枚举判断是不是包含该频道
                {
                    this.Messages[ch].AddRange(messages);
                }
            }
            if (Onchat != null)
                Onchat();
        }

        //把传入的参数  新建消息汇总进去
        public void AddSystemMessage(string message,string fromName = "")
        {
            this.Messages[(int)LocalChannel.ALL].Add(new ChatMessage()
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

        public bool SetSendChannel(LocalChannel channel)
        {
            if (channel == LocalChannel.Team)//传入的参数是频道
            {
                if (User.Instance.TeamInfo == null)
                {
                    this.AddSystemMessage("你没有加入任何队伍");
                    return false;
                }
            }
            if (channel == LocalChannel.Guild)//传入的参数是公会
            {
                if (User.Instance.CurrentCharacter.Guild == null)
                {
                    this.AddSystemMessage("你没有加入任何公会");
                    return false;
                }
            }
            this.sendChannel = channel;//把当前频道设置为传入的参数
            Debug.LogFormat("Set Channel:{0}", this.sendChannel);
            return true;
        }

        //获取当前所有消息
        public string GetCurrentMessages()
        {
            //StringBuilder便于拼接消息
            StringBuilder sb = new StringBuilder();
            foreach(var message in this.Messages[(int)displayChannel])//遍历所有message
            {
                sb.AppendLine(FormatMessage(message));//格式化message
            }
            return sb.ToString();
        }

        //根据不同频道  格式化消息
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
