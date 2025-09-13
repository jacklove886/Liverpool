using Common;
using GameServer.Entities;
using GameServer.Managers;
using Network;
using SkillBridge.Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameServer.Services
{
    class FriendService : Singleton<FriendService>
    {
        public FriendService()//构造函数
        {
            MessageDistributer<NetConnection<NetSession>>.Instance.Subscribe<FriendAddRequest>(this.OnFriendAddRequest);
            MessageDistributer<NetConnection<NetSession>>.Instance.Subscribe<FriendAddResponse>(this.OnFriendAddResponse);
            MessageDistributer<NetConnection<NetSession>>.Instance.Subscribe<FriendRemoveRequest>(this.OnFriendRemove);
        }

        public void Init()
        {

        }

        //A玩家发送添加B为好友的请求  
        private void OnFriendAddRequest(NetConnection<NetSession> sender, FriendAddRequest request)
        {
            //character代表A和sender代表A
            Character character = sender.Session.Character;
            Log.InfoFormat("收到添加好友请求:From角色ID:{0},Name:{1},To角色ID:{2},Name:{3}", request.FromId, request.FromName,request.ToId,request.ToName);

            if (request.ToId == 0)//没有传入ID 利用名称查找
            {
                foreach(var cha in CharacterManager.Instance.Characters)
                {
                    if (cha.Value.TCharacter.Name == request.ToName)
                    {
                        request.ToId = cha.Key;
                        break;
                    }
                }
            }
            //friend代表B
            NetConnection<NetSession> friend = null;
            if (request.ToId > 0)//用ID查找
            {
                //在好友管理器能查到信息    
                if (character.FriendManager.GetFriendInfo(request.ToId)!= null)
                {
                    sender.Session.Response.friendAddResponse = new FriendAddResponse();
                    sender.Session.Response.friendAddResponse.Result = Result.Failed;
                    sender.Session.Response.friendAddResponse.Errormsg = "已经是好友了";
                    sender.SendResponse();
                    return;
                }
            }
            friend = SessionManager.Instance.GetSession(request.ToId);
            if (friend == null)//有可能该玩家突然离线
            {
                sender.Session.Response.friendAddResponse = new FriendAddResponse();
                sender.Session.Response.friendAddResponse.Result = Result.Failed;
                sender.Session.Response.friendAddResponse.Errormsg = "该玩家不存在或不在线";
                sender.Session.Response.friendAddResponse.Request = request;
                sender.SendResponse();
                return;
            }
            Log.InfoFormat("ForwardRequest:From角色ID:{0},Name:{1},To角色ID:{2},Name:{3}", request.FromId, request.FromName, request.ToId, request.ToName);
            friend.Session.Response.friendAddRequest = request;
            friend.SendResponse();
        }


        //接收到B玩家对A玩家好友请求的结果响应(接收到B是接受了还是拒绝了)
        private void OnFriendAddResponse(NetConnection<NetSession> sender, FriendAddResponse response)
        {
            //character和sender代表B
            Character character = sender.Session.Character;
            Log.InfoFormat("OnFriendAddResponse:角色:{0},结果:{1},FromID:{2},FromID:{3}", character, response.Result, response.Request.FromId, response.Request.ToId);
            sender.Session.Response.friendAddResponse = response;
            if (response.Result==Result.Success)//B接受了
            {
                //requester代表A
                var requster = SessionManager.Instance.GetSession(response.Request.FromId);
                if (requster == null)
                {
                    sender.Session.Response.friendAddResponse.Result = Result.Failed;
                    sender.Session.Response.friendAddResponse.Errormsg = "请求者已离线";
                }
                else//互相加好友 添加成功
                {
                    //B把A添加进好友管理器
                    character.FriendManager.AddFriend(requster.Session.Character);
                    //A把B添加进好友管理器
                    requster.Session.Character.FriendManager.AddFriend(character);
                    DBService.Instance.Save();
                    requster.Session.Response.friendAddResponse = response;
                    requster.Session.Response.friendAddResponse.Result = Result.Success;
                    sender.Session.Response.friendAddResponse.Errormsg = "添加好友成功";
                    requster.SendResponse();
                }
            }
            else//b拒绝了
            {
                var requster = SessionManager.Instance.GetSession(response.Request.FromId);
                if (requster == null)
                {
                    sender.Session.Response.friendAddResponse.Result = Result.Failed;
                    sender.Session.Response.friendAddResponse.Errormsg = "请求者已离线";
                }
                else
                {
                    requster.Session.Response.friendAddResponse = response;  // 把B的响应直接给A
                    requster.SendResponse();
                }
                //b界面显示
                sender.Session.Response.friendAddResponse.Errormsg = "添加好友失败";
            }
            //如果拒绝 原封不动发回去
            sender.SendResponse();
        }

        //收到A要删除B的请求
        private void OnFriendRemove(NetConnection<NetSession> sender, FriendRemoveRequest request)
        {
            //character是A
            Character character = sender.Session.Character;
            Log.InfoFormat("收到删除好友请求:角色:{0},FriendReletion:{1}", request.Id, request.frinedId);
            sender.Session.Response.friendRemove = new FriendRemoveResponse();
            sender.Session.Response.friendRemove.Id = request.Id;

            //删除好友记录   request.Id 是好友记录的ID
            if (character.FriendManager.RemoveFriendByID(request.Id))
            {
                sender.Session.Response.friendRemove.Result = Result.Success;

                //在A的好友列表中删除B  friend是B
                var friend = SessionManager.Instance.GetSession(request.frinedId);//获取到B
                if (friend != null)//B在线
                {
                    //从B的好友管理器中删除A
                    //通过friendID删除记录
                    friend.Session.Character.FriendManager.RemoveFriendByFriendID(character.Id);
                }
                else//B不在线  直接操作数据库删除
                {
                    RemoveFriend(character.Id,request.frinedId);
                }
            }
            else
            {
                sender.Session.Response.friendRemove.Result = Result.Failed;
            }
            DBService.Instance.Save();
            sender.SendResponse();
        }

        void RemoveFriend(int characterId,int friendId)
        {
            //characterId是A  friendId是B
            var removeItem = DBService.Instance.Entities.CharacterFriends.FirstOrDefault(v => v.FriendID == characterId && v.CharacterID == friendId);
            if (removeItem != null)
            {
                DBService.Instance.Entities.CharacterFriends.Remove(removeItem);
            }
        }
    }
}
