using Common;
using Common.Utils;
using GameServer.Entities;
using GameServer.Models;
using GameServer.Services;
using SkillBridge.Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameServer.Managers
{
    class ChatManager : Singleton<ChatManager>
    {
        //全局唯一
        public List<ChatMessage> System = new List<ChatMessage>();//综合
        public List<ChatMessage> World = new List<ChatMessage>();//世界

        //每张地图 每个队伍 每个公会维护自己的消息
        public Dictionary<int, List<ChatMessage>> Local = new Dictionary<int, List<ChatMessage>>();//本地
        public Dictionary<int, List<ChatMessage>> Team = new Dictionary<int, List<ChatMessage>>();//队伍
        public Dictionary<int, List<ChatMessage>> Guild = new Dictionary<int, List<ChatMessage>>();//公会

        public void Init()
        {

        }

        public void AddMessage(Character fromCharacter,ChatMessage message)
        {
            message.FromId = fromCharacter.Id;
            message.FromName = fromCharacter.Name;
            message.Time = TimeUtil.timestamp;
            switch (message.Channel)
            {
                case ChatChannel.System:
                    this.AddSystemMessage(message);
                    break;

                case ChatChannel.World:
                    this.AddWorldMessage(message);
                    break;

                case ChatChannel.Local:
                    this.AddLocalMessage(fromCharacter.Info.mapId, message);
                    break;

                case ChatChannel.Team:
                    this.AddTeamMessage(fromCharacter.Team.Id, message);
                    break;

                case ChatChannel.Guild:
                    this.AddGuildMessage(fromCharacter.Guild.Id, message);
                    break;
            }
        }

        private void AddSystemMessage(ChatMessage message)
        {
            this.System.Add(message);
        }

        private void AddWorldMessage(ChatMessage message)
        {
            this.World.Add(message);
        }

        private void AddLocalMessage(int mapId, ChatMessage message)
        {
            //只有第一次用到这个结构才创建
            if(!this.Local.TryGetValue(mapId,out List<ChatMessage> messages))
            {
                messages = new List<ChatMessage>();
                this.Local[mapId] = messages;//添加到字典
            }
            messages.Add(message);//添加到列表
        }

        private void AddTeamMessage(int teamId, ChatMessage message)
        {
            //只有第一次用到这个结构才创建
            if (!this.Team.TryGetValue(teamId, out List<ChatMessage> messages))
            {
                messages = new List<ChatMessage>();
                this.Team[teamId] = messages;//添加到字典
            }
            messages.Add(message);//添加到列表
        }

        private void AddGuildMessage(int guildId, ChatMessage message)
        {
            //只有第一次用到这个结构才创建
            if (!this.Guild.TryGetValue(guildId, out List<ChatMessage> messages))
            {
                messages = new List<ChatMessage>();
                this.Guild[guildId] = messages;//添加到字典
            }
            messages.Add(message);//添加到列表
        }

        public int GetSystemMessage(int index,List<ChatMessage> result)
        {
            return GetNewMessage(index, result, this.System);
        }

        public int GetWorldMessage(int index, List<ChatMessage> result)
        {
            return GetNewMessage(index, result, this.System);
        }

        public int GetLocalMessage(int mapId,int index,List<ChatMessage> result)
        {
            if(!this.Local.TryGetValue(mapId, out List<ChatMessage> messages))
            {
                return 0;
            }
            return GetNewMessage(index, result, messages);
        }

        public int GetTeamMessage(int teamId, int index, List<ChatMessage> result)
        {
            if (!this.Team.TryGetValue(teamId, out List<ChatMessage> messages))
            {
                return 0;
            }
            return GetNewMessage(index, result, messages);
        }

        public int GetGuildMessage(int guildId, int index, List<ChatMessage> result)
        {
            if (!this.Guild.TryGetValue(guildId, out List<ChatMessage> messages))
            {
                return 0;
            }
            return GetNewMessage(index, result, messages);
        }

        private int GetNewMessage(int index, List<ChatMessage> result, List<ChatMessage> messages)
        {
            //比如总共50条  减去20条  index是30  就是从30开始获取到50的记录
            if (index == 0)
            {
                if (messages.Count > GameDefine.MaxChatRecoredNums)//如果总数超过20条
                {
                    index = messages.Count - GameDefine.MaxChatRecoredNums;//获得减去20条的索引
                }
            }

            for (; index < messages.Count; index++)//从索引开始获取到总数的记录
            {
                result.Add(messages[index]);
            }
            return index;
        }
    }
}
