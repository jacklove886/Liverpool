using Models;
using SkillBridge.Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Managers
{
    public class FriendManager : Singleton<FriendManager>
    {
        public List<NFriendInfo> allfriends;//定义的Info网络协议

        public void Init(List<NFriendInfo> friends)
        {
            this.allfriends = friends;
        }
     
    }
}
