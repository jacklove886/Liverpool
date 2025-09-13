using Managers;
using Models;
using Network;
using SkillBridge.Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Services
{
    class FriendService : Singleton<FriendService>, IDisposable
    {
        public System.Action OnFriendUpdate; // 好友列表更新事件

        public FriendService()//构造函数
        {
            MessageDistributer.Instance.Subscribe<FriendAddRequest>(this.OnFriendAddRequest);
            MessageDistributer.Instance.Subscribe<FriendAddResponse>(this.OnFriendAddResponse);
            MessageDistributer.Instance.Subscribe<FriendListResponse>(this.OnFriendList);
            MessageDistributer.Instance.Subscribe<FriendRemoveResponse>(this.OnFriendRemove);
        }



        public void Dispose()
        {
            MessageDistributer.Instance.Unsubscribe<FriendAddRequest>(this.OnFriendAddRequest);
            MessageDistributer.Instance.Unsubscribe<FriendAddResponse>(this.OnFriendAddResponse);
            MessageDistributer.Instance.Unsubscribe<FriendListResponse>(this.OnFriendList);
            MessageDistributer.Instance.Unsubscribe<FriendRemoveResponse>(this.OnFriendRemove);
        }

        public void Init()
        {

        }

        public void SendFriendAddRequest(int friendId, string friendName)//发送添加好友请求
        {
            Debug.LogFormat("发送添加好友请求");
            NetMessage message = new NetMessage();
            message.Request = new NetMessageRequest();
            message.Request.friendAddRequest = new FriendAddRequest();
            message.Request.friendAddRequest.FromId = User.Instance.CurrentCharacter.Id;
            message.Request.friendAddRequest.FromName = User.Instance.CurrentCharacter.Name;
            message.Request.friendAddRequest.ToId = friendId;
            message.Request.friendAddRequest.ToName = friendName;
            NetClient.Instance.SendMessage(message);
        }

        //接收到A玩家发送添加B为好友的请求   返回一个接受或拒绝的结果 并把请求原封不动返回服务器
        public void OnFriendAddRequest(object sender, FriendAddRequest request)
        {
            var confirm = MessageBox.Show(string.Format("{0}请求加你为好友", request.FromName), "好友请求", MessageBoxType.Confirm, "同意", "拒绝");
            confirm.OnYes = () =>
                {
                    SendFriendAddResponse(true, request);
                };
            confirm.OnNo = () =>
                {
                    SendFriendAddResponse(false, request);
                };
        }

        public void SendFriendAddResponse(bool accept, FriendAddRequest request)//对方发来的请求
        {
            NetMessage message = new NetMessage();
            message.Request = new NetMessageRequest();
            message.Request.friendAddResponse = new FriendAddResponse();
            message.Request.friendAddResponse.Result = accept ? Result.Success : Result.Failed;
            message.Request.friendAddResponse.Errormsg = accept ? "对方同意了你的好友" : "对方拒绝了你的好友请求";
            message.Request.friendAddResponse.Request = request;
            NetClient.Instance.SendMessage(message);          
        }

        public void OnFriendAddResponse(object sender, FriendAddResponse response)
        {
            if (response.Result == Result.Success)
            {
                MessageBox.Show(response.Errormsg,"添加成功");
            }
            else if (response.Result == Result.Failed)
            {
                MessageBox.Show(response.Errormsg, "添加失败");
            }
        }

        public void OnFriendList(object sender, FriendListResponse response)
        {
            FriendManager.Instance.Init(response.Friends);
            //通知好友管理器刷新列表
            if (OnFriendUpdate != null)
            {
                OnFriendUpdate();
            }
        }

        //A要删除B 发送请求
        public void SendFriendRemoveRequest(int id, int friendId)//发送删除好友请求
        {
            Debug.LogFormat("发送删除好友请求");
            NetMessage message = new NetMessage();
            message.Request = new NetMessageRequest();
            message.Request.friendRemove = new FriendRemoveRequest();
            message.Request.friendRemove.Id = id;
            message.Request.friendRemove.frinedId = friendId;
            NetClient.Instance.SendMessage(message);
        }

        public void OnFriendRemove(object sender, FriendRemoveResponse response)
        {
            if (response.Result == Result.Success)
            {
                MessageBox.Show("删除成功", "删除好友");
            }
            else if (response.Result == Result.Failed)
            {
                MessageBox.Show("删除失败", "删除好友",MessageBoxType.Error);
            }
        }
    }
}
