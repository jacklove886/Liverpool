using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using GameServer;
using GameServer.Entities;
using GameServer.Services;
using SkillBridge.Message;

namespace Network
{
    public class NetSession:INetSession
    {
        public TUser User { get; set; }
        public Character Character { get; set; }
        public NEntity Entity { get; set; }
        public IPostResponser PostResponser { get; set; }

        public void Disconnected()
        {
            this.PostResponser = null;
            if (Character != null)
            {
                UserService.Instance.CharacterLeave(Character);
            }
        }

        private NetMessage response;

        public NetMessageResponse Response
        {
            get
            {
                if (response == null)
                    response = new NetMessage();
                if (response.Response == null)
                {
                    response.Response = new NetMessageResponse();
                }
                return response.Response;
            }
        }

        public byte[] GetResponse()
        {
            if (response != null)
            {
                if (PostResponser != null)
                {
                    this.PostResponser.PostProcess(Response);
                }
                byte[] data = PackageHandler.PackMessage(response);//打包发送response
                response = null;//response设为空  保证了消息不会重复
                return data;
            }
            return null;
        }

    }
}
