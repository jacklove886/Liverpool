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

        internal void SendChat(ChatManager.LocalChannel sendChannel, string text, int toId, string toName)
        {
            
        }
    }
}
