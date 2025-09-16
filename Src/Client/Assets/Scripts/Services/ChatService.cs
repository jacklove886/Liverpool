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
    class ChatService : Singleton<ChatService>
    {

        public void Init()
        {

        }

        public ChatService()//构造函数
        {
            MessageDistributer.Instance.Subscribe<ChatResponse>(this.OnChat);          
        }

        public void Dispose()
        {
            MessageDistributer.Instance.Unsubscribe<ChatResponse>(this.OnChat);
        }

        internal void SendChat(ChatChannel SendChannel, string text, int toId, string toName)
        {
            Debug.Log("发送聊天请求");
            NetMessage message = new NetMessage();
            message.Request = new NetMessageRequest();
            message.Request.Chat = new ChatRequest();
            message.Request.Chat.Message = new ChatMessage();
            message.Request.Chat.Message.Channel = SendChannel;
            message.Request.Chat.Message.Message = text;
            message.Request.Chat.Message.ToId = toId;
            message.Request.Chat.Message.ToName = toName;
            NetClient.Instance.SendMessage(message);
        }

        private void OnChat(object sender, ChatResponse response)
        {
            Debug.LogFormat("收到聊天响应,{0}", response.Result);
            if (response.Result == Result.Success)
            {
                if (response.systemMessages != null && response.systemMessages.Count > 0)
                {
                    ChatManager.Instance.AddMessages(ChatChannel.System, response.systemMessages);
                }
                if (response.localMessages != null && response.localMessages.Count > 0)
                {
                    ChatManager.Instance.AddMessages(ChatChannel.Local, response.localMessages);
                }
                if (response.worldMessages != null && response.worldMessages.Count > 0)
                {
                    ChatManager.Instance.AddMessages(ChatChannel.World, response.worldMessages);
                }
                if (response.teamMessages != null && response.teamMessages.Count > 0)
                {
                    ChatManager.Instance.AddMessages(ChatChannel.Team, response.teamMessages);
                }
                if (response.guildMessages != null && response.guildMessages.Count > 0)
                {
                    ChatManager.Instance.AddMessages(ChatChannel.Guild, response.guildMessages);
                }
                if (response.privateMessages != null && response.privateMessages.Count > 0)
                {
                    ChatManager.Instance.AddMessages(ChatChannel.Private, response.privateMessages);
                }
            }
            else
            {
                ChatManager.Instance.AddSystemMessage(response.Msg);
            }
        }
        
    }
}
