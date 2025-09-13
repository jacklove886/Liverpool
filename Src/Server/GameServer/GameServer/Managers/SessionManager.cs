using Common;
using Network;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameServer.Managers
{
    class SessionManager:Singleton<SessionManager>
    {
        //字典保存着characterID的Session
        public Dictionary<int, NetConnection<NetSession>> Sessions = new Dictionary<int, NetConnection<NetSession>>();

        public void AddSession(int characterId, NetConnection<NetSession> session)
        {
            this.Sessions[characterId] = session;
        }

        public void Remove(int characterId)
        {
            this.Sessions.Remove(characterId);
        }

        public NetConnection<NetSession> GetSession(int characterId)
        {
            NetConnection<NetSession> session = null;
            this.Sessions.TryGetValue(characterId, out session);
            return session;
        }

        public bool GetSessionByUserId(long userId)
        {
            foreach(var session in Sessions.Values)//遍历所有会话
            {
                if (session.Session.User != null && session.Session.User.ID == userId)//如果有和当前输入的用户ID相同的
                {
                    return true;
                }
            }
            return false;
        }
    }
}
