using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Common;
using Network;
using UnityEngine;

using SkillBridge.Message;
using Models;
using System.IO;

namespace Services
{
    class UserService : Singleton<UserService>, IDisposable
    {
        public UnityEngine.Events.UnityAction<Result, string> OnRegister;
        public UnityEngine.Events.UnityAction<Result, string> OnLogin;
        public UnityEngine.Events.UnityAction<Result, string> OnCharacterCreate;
        public UnityEngine.Events.UnityAction<Result, string> OnCharacterDelete;
        NetMessage pendingMessage = null;
        bool connected = false;
        private static bool logInitialized = false;  

        public UserService()
        {
            NetClient.Instance.OnConnect += OnGameServerConnect;
            NetClient.Instance.OnDisconnect += OnGameServerDisconnect;
            MessageDistributer.Instance.Subscribe<UserRegisterResponse>(this.OnUserRegister);
            MessageDistributer.Instance.Subscribe<UserLoginResponse>(this.OnUserLogin);
            MessageDistributer.Instance.Subscribe<UserCreateCharacterResponse>(this.OnUserCharacterCreate);
            MessageDistributer.Instance.Subscribe<UserDeleteCharacterResponse>(this.OnUserCharacterDelete);
            MessageDistributer.Instance.Subscribe<UserGameEnterResponse>(this.OnUserGameEnter);
            MessageDistributer.Instance.Subscribe<UserGameLeaveResponse>(this.OnUserGameLeave);
            MessageDistributer.Instance.Subscribe<MapCharacterEnterResponse>(this.OnUserMapCharacterEnter);
        }


        public void Dispose()
        {
            MessageDistributer.Instance.Unsubscribe<UserRegisterResponse>(this.OnUserRegister);
            MessageDistributer.Instance.Unsubscribe<UserLoginResponse>(this.OnUserLogin);
            MessageDistributer.Instance.Unsubscribe<UserCreateCharacterResponse>(this.OnUserCharacterCreate);
            MessageDistributer.Instance.Unsubscribe<UserDeleteCharacterResponse>(this.OnUserCharacterDelete);
            MessageDistributer.Instance.Unsubscribe<UserGameEnterResponse>(this.OnUserGameEnter);
            MessageDistributer.Instance.Unsubscribe<UserGameLeaveResponse>(this.OnUserGameLeave);
            MessageDistributer.Instance.Unsubscribe<MapCharacterEnterResponse>(this.OnUserMapCharacterEnter);
            NetClient.Instance.OnConnect -= OnGameServerConnect;
            NetClient.Instance.OnDisconnect -= OnGameServerDisconnect;
        }

        public void Init()
        {
        }

        public void ConnectToServer()
        {
            if (!logInitialized)
            {
                Common.Log.Init("UserService");
                logInitialized = true;
            }

            //NetClient.Instance.CryptKey = this.SessionId;
            NetClient.Instance.Init("127.0.0.1", 8000);
            NetClient.Instance.Connect();
        }


        void OnGameServerConnect(int result, string reason)
        {
            Log.InfoFormat("LoadingMesager::OnGameServerConnect :{0} reason:{1}", result, reason);
            if (NetClient.Instance.Connected)
            {
                this.connected = true;
                if(this.pendingMessage!=null)
                {
                    NetClient.Instance.SendMessage(this.pendingMessage);
                    this.pendingMessage = null;
                }
            }
            else
            {
                if (!this.DisconnectNotify(result, reason))
                {
                    MessageBox.Show(string.Format("网络错误，无法连接到服务器！\n RESULT:{0} ERROR:{1}", result, reason), "错误", MessageBoxType.Error);
                }
            }
        }

        public void OnGameServerDisconnect(int result, string reason)
        {
            this.DisconnectNotify(result, reason);
            return;
        }

        bool DisconnectNotify(int result,string reason)
        {
            if (this.pendingMessage != null)
            {
                if (this.pendingMessage.Request.userRegister!=null)
                {
                    if (this.OnRegister != null)
                    {
                        this.OnRegister(Result.Failed, string.Format("服务器断开！\n RESULT:{0} ERROR:{1}", result, reason));
                    }
                }
                return true;
            }
            return false;
        }

        public void SendRegister(string user, string psw)
        {
            Debug.LogFormat("UserRegisterRequest::user :{0} psw:{1}", user, psw);
            NetMessage message = new NetMessage();
            message.Request = new NetMessageRequest();
            message.Request.userRegister = new UserRegisterRequest();
            message.Request.userRegister.User = user;
            message.Request.userRegister.Passward = psw;

            if (this.connected && NetClient.Instance.Connected)
            {
                this.pendingMessage = null;
                NetClient.Instance.SendMessage(message);
            }
            else
            {
                this.pendingMessage = message;
                this.ConnectToServer();
            }
        }

        void OnUserRegister(object sender, UserRegisterResponse response)
        {
            Debug.LogFormat("OnUserRegister:{0} [{1}]", response.Result, response.Errormsg);

            if (this.OnRegister != null)
            {
                this.OnRegister(response.Result, response.Errormsg);
            }
        }
        public void SendLogin(string user, string psw)
        {
            Debug.LogFormat("用户名 :{0} 密码:{1}", user, psw);
            NetMessage message = new NetMessage();
            message.Request = new NetMessageRequest();
            message.Request.userLogin = new UserLoginRequest();
            message.Request.userLogin.User = user;
            message.Request.userLogin.Passward = psw;

            if (this.connected && NetClient.Instance.Connected)
            {
                this.pendingMessage = null;
                NetClient.Instance.SendMessage(message);
            }
            else
            {
                this.pendingMessage = message;
                this.ConnectToServer();
            }
        }

        void OnUserLogin(object sender, UserLoginResponse response)
        {
            // 登录成功时设置用户数据
            if (response.Result == Result.Success && response.Userinfo != null)
            {
                Models.User.Instance.SetupUserInfo(response.Userinfo);
                Debug.Log("登录成功");
            }
            if (this.OnLogin != null)
            {
                this.OnLogin(response.Result, response.Errormsg);
            }
        }

        public void SendCharacterCreate(string nameInputField, CharacterClass charClass)
        {
            Debug.LogFormat("创建角色 :{0}",nameInputField);
            NetMessage message = new NetMessage();
            message.Request = new NetMessageRequest();
            message.Request.createChar = new UserCreateCharacterRequest();
            message.Request.createChar.Name = nameInputField;
            message.Request.createChar.Class= charClass;

            if (this.connected && NetClient.Instance.Connected)
            {
                this.pendingMessage = null;
                NetClient.Instance.SendMessage(message);
            }
            else
            {
                this.pendingMessage = message;
                this.ConnectToServer();
            }
        }

        void OnUserCharacterCreate(object sender, UserCreateCharacterResponse response)
        {
            Debug.LogFormat("创建角色接收到响应");
            if (response.Result == Result.Success)
            {
                if (response.Characters != null)
                {
                    //Models.User.Instance.Info.Player.Characters.Clear();  不清空原有的 只添加新角色
                    Models.User.Instance.Info.Player.Characters.AddRange(response.Characters);
                }
            }
            if (this.OnCharacterCreate != null)
            {
                this.OnCharacterCreate(response.Result, response.Errormsg);
            }
        }

        public void SendCharacterDelete(string characterName)
        {
            Debug.LogFormat("删除角色: {0}", characterName);
            NetMessage message = new NetMessage();
            message.Request = new NetMessageRequest();
            message.Request.deleteChar = new UserDeleteCharacterRequest();
            message.Request.deleteChar.Name = characterName;

            if (this.connected && NetClient.Instance.Connected)
            {
                this.pendingMessage = null;
                NetClient.Instance.SendMessage(message);
                // 服务器会正常处理删除，但客户端不等待响应
                // 从本地角色列表中移除角色
                var characterToRemove = User.Instance.Info.Player.Characters.FirstOrDefault(c => c.Name == characterName);
                if (characterToRemove != null)
                {
                    User.Instance.Info.Player.Characters.Remove(characterToRemove);
                }
                
                // 立即触发成功回调，让UI更新角色列表
                if (this.OnCharacterDelete != null)
                {
                    this.OnCharacterDelete(Result.Success, "");
                }
            }
            else
            {
                this.pendingMessage = message;
                this.ConnectToServer();
            }
        }

        void OnUserCharacterDelete(object sender, UserDeleteCharacterResponse response)
        {
            Debug.LogFormat("删除角色接收到响应");
            if (response.Result == Result.Success)
            {
                // 用服务器返回的完整列表
                User.Instance.Info.Player.Characters.Clear();
                User.Instance.Info.Player.Characters.AddRange(response.Characters);
            }
            if (this.OnCharacterDelete != null)
            {
                this.OnCharacterDelete(response.Result, response.Errormsg);
            }
        }

        public void SendGameEnter(int characterIndex)
        {
            NetMessage message = new NetMessage();
            message.Request = new NetMessageRequest();
            message.Request.gameEnter = new UserGameEnterRequest();
            message.Request.gameEnter.characterIdx = characterIndex;

            if (this.connected && NetClient.Instance.Connected)
            {
                this.pendingMessage = null;
                NetClient.Instance.SendMessage(message);
            }
            else
            {
                this.pendingMessage = message;
                this.ConnectToServer();
            }
        }

        void OnUserGameEnter(object sender, UserGameEnterResponse response)
        {
            Debug.LogFormat("角色进入游戏:{0}", response.Result);
            if (response.Result == Result.Success)
            {
                //进入游戏
            }
        }


        public void SendGameLeave()
        {
            NetMessage message = new NetMessage();
            message.Request = new NetMessageRequest();
            message.Request.gameLeave = new UserGameLeaveRequest();
            NetClient.Instance.SendMessage(message);
        }

        void OnUserGameLeave(object sender, UserGameLeaveResponse response)
        {
            //MapService.Instance.CurrentMapId = 0;
            Debug.LogFormat("角色离开游戏:{0}", response.Result);
        }


        void OnUserMapCharacterEnter(object sender, MapCharacterEnterResponse response)
        {
            Debug.LogFormat("角色进入第:{0}张地图", response.mapId);
            if (SceneManager.Instance == null)
            {
                Debug.LogError("SceneManager.Instance is null");
                return;
            }
            NCharacterInfo info = response.Characters[0];
            User.Instance.CurrentCharacter = info;
            SceneManager.Instance.LoadScene(DataManager.Instance.Maps[response.mapId].Resource);
        }
    }
}
