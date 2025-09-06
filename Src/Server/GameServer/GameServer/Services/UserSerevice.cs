using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common;
using Network;
using SkillBridge.Message;
using GameServer.Entities;
using GameServer.Managers;
using System.Data.Entity;
using System.IO;

namespace GameServer.Services
{
    class UserService : Singleton<UserService>
    {


        public UserService()
        {
            MessageDistributer<NetConnection<NetSession>>.Instance.Subscribe<UserRegisterRequest>(this.OnRegister);
            MessageDistributer<NetConnection<NetSession>>.Instance.Subscribe<UserLoginRequest>(this.OnLogin);
            MessageDistributer<NetConnection<NetSession>>.Instance.Subscribe<UserCreateCharacterRequest>(this.OnCharacterCreate);
            MessageDistributer<NetConnection<NetSession>>.Instance.Subscribe<UserDeleteCharacterRequest>(this.OnCharacterDelete);
            MessageDistributer<NetConnection<NetSession>>.Instance.Subscribe<UserGameEnterRequest>(this.OnGameEnter);
            MessageDistributer<NetConnection<NetSession>>.Instance.Subscribe<UserGameLeaveRequest>(this.OnGameLeave);
        }

        public void Init()
        {

        }

        //注册请求
        void OnRegister(NetConnection<NetSession> sender, UserRegisterRequest request)
        {
            Log.InfoFormat("UserRegisterRequest: User:{0}  Pass:{1}", request.User, request.Passward);

            NetMessage message = new NetMessage();
            message.Response = new NetMessageResponse();
            message.Response.userRegister = new UserRegisterResponse();

            TUser user = DBService.Instance.Entities.Users.Where(u => u.Username == request.User).FirstOrDefault();
            if (user != null)
            {
                message.Response.userRegister.Result = Result.Failed;
                message.Response.userRegister.Errormsg = "用户已存在.";
            }
            else
            {
                TPlayer player = DBService.Instance.Entities.Players.Add(new TPlayer());
                DBService.Instance.Entities.Users.Add(new TUser() { Username = request.User, Password = request.Passward, Player = player });
                DBService.Instance.Entities.SaveChanges();
                message.Response.userRegister.Result = Result.Success;
                message.Response.userRegister.Errormsg = "None";
            }

            byte[] data = PackageHandler.PackMessage(message);
            sender.SendData(data, 0, data.Length);
        }
        //登录请求
        void OnLogin(NetConnection<NetSession> sender, UserLoginRequest request)
        {
            Log.InfoFormat("UserLoginRequest: User:{0}  Pass:{1}", request.User, request.Passward);

            NetMessage message = new NetMessage();
            message.Response = new NetMessageResponse();
            message.Response.userLogin = new UserLoginResponse();

            TUser user = DBService.Instance.Entities.Users.Where(u => u.Username == request.User).FirstOrDefault();
            if (user == null)
            {
                message.Response.userLogin.Result = Result.Failed;
                message.Response.userLogin.Errormsg = "用户不存在.";
            }
            else if (user.Password!=request.Passward)
            {
                message.Response.userLogin.Result = Result.Failed;
                message.Response.userLogin.Errormsg = "密码错误";
            }
            else
            {
                sender.Session.User = user;
                message.Response.userLogin.Result = Result.Success;
                message.Response.userLogin.Errormsg = "None";
                message.Response.userLogin.Userinfo = new NUserInfo();
                message.Response.userLogin.Userinfo.Id = (int)user.ID;
                message.Response.userLogin.Userinfo.Player = new NPlayerInfo();
                message.Response.userLogin.Userinfo.Player.Id = user.Player.ID;
                foreach (var c in user.Player.Characters)
                {
                    NCharacterInfo info = new NCharacterInfo();
                    info.Id = 0;  //Entity ID
                    info.Name = c.Name;
                    info.Class = (CharacterClass)c.Class;
                    info.Type = CharacterType.Player;
                    info.Level = c.Level;
                    info.Tid = c.ID;
                    message.Response.userLogin.Userinfo.Player.Characters.Add(info);
                }
            }

            byte[] data = PackageHandler.PackMessage(message);
            sender.SendData(data, 0, data.Length);
        }

        //创建角色的请求
        void OnCharacterCreate(NetConnection<NetSession> sender, UserCreateCharacterRequest request)
        {
            Log.InfoFormat("创建角色的姓名: Name:{0} 职业: Class:{1}", request.Name,request.Class);

            var existingCharacter = DBService.Instance.Entities.Characters.FirstOrDefault(c => c.Name == request.Name);

            //角色名字已存在
            if (existingCharacter != null)
            {
                NetMessage messageFail = new NetMessage();
                messageFail.Response = new NetMessageResponse();
                messageFail.Response.createChar = new UserCreateCharacterResponse();
                messageFail.Response.createChar.Result = Result.Failed;
                messageFail.Response.createChar.Errormsg = "角色名已存在,请重新输入";
                byte[] dataFail = PackageHandler.PackMessage(messageFail);
                //消息打包成数据流发给客户端
                sender.SendData(dataFail, 0, dataFail.Length);
                return;
            }

            TCharacter character = new TCharacter()
            {
                Name = request.Name,
                Class = (int)request.Class,
                TID = (int)request.Class,
                Level = 1,
                MapID = 1,//默认出身在地图1
                MapPosX = 4150,//出生点的三维坐标
                MapPosY = 3000,
                MapPosZ = 800,
            };
            DBService.Instance.Entities.Characters.Add(character);
            sender.Session.User.Player.Characters.Add(character);
            DBService.Instance.Entities.SaveChanges();


            //处理回发的消息
            NetMessage message = new NetMessage();
            message.Response = new NetMessageResponse();
            message.Response.createChar = new UserCreateCharacterResponse();
            message.Response.createChar.Result = Result.Success;
            message.Response.createChar.Errormsg = "None";


            //返回新创建的角色

            NCharacterInfo newCharacterInfo = new NCharacterInfo();
            newCharacterInfo.Id = 0;  //Entity ID
            newCharacterInfo.Name = character.Name;
            newCharacterInfo.Class = (CharacterClass)character.Class;
            newCharacterInfo.Type = CharacterType.Player;
            newCharacterInfo.Level = character.Level;
            newCharacterInfo.Tid = character.ID;

            message.Response.createChar.Characters.Add(newCharacterInfo);


            byte[] data = PackageHandler.PackMessage(message);
            //消息打包成数据流发给客户端
            sender.SendData(data, 0, data.Length);
        }

        //删除角色的请求
        void OnCharacterDelete(NetConnection<NetSession> sender, UserDeleteCharacterRequest request)
        {
            if (sender.Session.User == null)
            {
                return;
            }
            Log.InfoFormat("删除角色的姓名:{0}", request.Name);

            NetMessage message = new NetMessage();
            message.Response = new NetMessageResponse();
            message.Response.deleteChar = new UserDeleteCharacterResponse();

            try
            {
                var deleteCharacter = sender.Session.User.Player.Characters.FirstOrDefault(c => c.Name == request.Name);

                //角色名字不存在 不执行删除操作
                if (deleteCharacter == null)
                {
                    message.Response.deleteChar.Result = Result.Failed;
                    message.Response.deleteChar.Errormsg = "角色不存在";
                }
                else
                {
                    // 从数据库删除角色
                    DBService.Instance.Entities.Characters.Remove(deleteCharacter);
                    sender.Session.User.Player.Characters.Remove(deleteCharacter);
                    DBService.Instance.Entities.SaveChanges();

                    message.Response.deleteChar.Result = Result.Success;
                    message.Response.deleteChar.Errormsg = "None";
                }
                // 返回删除后的完整角色列表
                foreach (var character in sender.Session.User.Player.Characters)
                {
                    NCharacterInfo characterInfo = new NCharacterInfo();
                    characterInfo.Id = character.ID;
                    characterInfo.Name = character.Name;
                    characterInfo.Class = (CharacterClass)character.Class;
                    characterInfo.Level = character.Level;
                    characterInfo.Tid = character.TID;
                    characterInfo.mapId = character.MapID;
                    message.Response.deleteChar.Characters.Add(characterInfo);
                }
            }
            catch (Exception ex)
            {
                Log.ErrorFormat("删除角色异常：{0}", ex.Message);
                message.Response.deleteChar.Result = Result.Failed;
                message.Response.deleteChar.Errormsg = "删除失败";
            }

            //消息打包成数据流发给客户端
            byte[] data = PackageHandler.PackMessage(message);
            sender.SendData(data, 0, data.Length);
        }

        private void OnGameEnter(NetConnection<NetSession> sender, UserGameEnterRequest request)
        {
            TCharacter databaseCharacter = sender.Session.User.Player.Characters.ElementAt(request.characterIdx);
            Log.InfoFormat("UserGameEnterRequest: characterID:{0}:{1} Map:{2}", databaseCharacter.ID, databaseCharacter.Name, databaseCharacter.MapID);
            Character character = CharacterManager.Instance.CharacterAdd(databaseCharacter);

            NetMessage message = new NetMessage();
            message.Response = new NetMessageResponse();
            message.Response.gameEnter = new UserGameEnterResponse();
            message.Response.gameEnter.Result = Result.Success;
            message.Response.gameEnter.Errormsg = "None";

            //进入成功,发送初始角色信息
            message.Response.gameEnter.Character = character.Info;

            //道具系统测试
            int itemID = 7;//道具ID
            bool hasItem = character.ItemManager.HasItem(itemID);//是否拥有
            Log.InfoFormat("拥有道具:[{0}]{1}", itemID, hasItem);
            if (hasItem)
            {
                character.ItemManager.RemoveItem(itemID, 1);//如果有就删除
            }
            else
            {
                character.ItemManager.AddItem(itemID, 5);//没有就添加
            }
            Models.Item item = character.ItemManager.SearchItem(itemID);//查询道具
            Log.InfoFormat("查找到道具:[{0}]{1}", itemID, item);

            byte[] data = PackageHandler.PackMessage(message);
            sender.SendData(data, 0, data.Length);
            sender.Session.Character = character;
            MapManager.Instance[databaseCharacter.MapID].CharacterEnter(sender, character);
        }

        private void OnGameLeave(NetConnection<NetSession> sender, UserGameLeaveRequest request)
        {
            Character character = sender.Session.Character;
            Log.InfoFormat("UserGameLeaveRequest: characterID:{0}:{1} Map:{2}", character.Id, character.Info.Name, character.Info.mapId);

            CharacterLeave(character);
            NetMessage message = new NetMessage();
            message.Response = new NetMessageResponse();
            message.Response.gameLeave = new UserGameLeaveResponse();
            message.Response.gameLeave.Result = Result.Success;
            message.Response.gameLeave.Errormsg = "None";

            byte[] data = PackageHandler.PackMessage(message);
            sender.SendData(data, 0, data.Length);
        }

        public  void CharacterLeave(Character character)
        {
            CharacterManager.Instance.CharacterRemove(character.Id);
            MapManager.Instance[character.Info.mapId].CharacterLeave(character);
        }
    }
}
