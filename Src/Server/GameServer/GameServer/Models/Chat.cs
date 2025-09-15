using Common;
using Common.Utils;
using GameServer.Entities;
using GameServer.Managers;
using GameServer.Services;
using SkillBridge.Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameServer.Models
{
    public class Chat
    {
        public int systemIndex;//表示当前的聊天记录拉到第几条
        public int worldIndex;
        public int localIndex;
        public int teamIndex;
        public int guildIndex;

        private Character owner;

        public Chat(Character character)
        {
            this.owner = character;
        }

        public void PostProcess(NetMessageResponse message)
        {
            if (message.Chat == null)
            {
                message.Chat = new ChatResponse();
                message.Chat.Result = Result.Success;
            }
            this.systemIndex = ChatManager.Instance.GetSystemMessage(this.systemIndex, message.Chat.systemMessages);
            this.worldIndex = ChatManager.Instance.GetWorldMessage(this.worldIndex, message.Chat.worldMessages);
            this.localIndex = ChatManager.Instance.GetLocalMessage(this.owner.Info.mapId, this.localIndex, message.Chat.localMessages);
            if (this.owner.Team != null)
            {
                this.teamIndex = ChatManager.Instance.GetLocalMessage(this.owner.Team.Id, this.teamIndex, message.Chat.teamMessages);
            }
            if (this.owner.Guild != null)
            {
                this.guildIndex = ChatManager.Instance.GetGuildMessage(this.owner.Guild.Id, this.guildIndex, message.Chat.guildMessages);
            }
        }
    }
}
