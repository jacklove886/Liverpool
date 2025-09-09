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

            sender.Session.Response.userLogin = new UserLoginResponse();

            TUser user = DBService.Instance.Entities.Users.Where(u => u.Username == request.User).FirstOrDefault();
            if (user == null)
            {
                sender.Session.Response.userLogin.Result = Result.Failed;
                sender.Session.Response.userLogin.Errormsg = "用户不存在.";
            }
            else if (user.Password!=request.Passward)
            {
                sender.Session.Response.userLogin.Result = Result.Failed;
                sender.Session.Response.userLogin.Errormsg = "密码错误";
            }
            else
            {
                sender.Session.User = user;

                sender.Session.Response.userLogin.Result = Result.Success;
                sender.Session.Response.userLogin.Errormsg = "None";
                sender.Session.Response.userLogin.Userinfo = new NUserInfo();
                sender.Session.Response.userLogin.Userinfo.Id = (int)user.ID;
                sender.Session.Response.userLogin.Userinfo.Player = new NPlayerInfo();
                sender.Session.Response.userLogin.Userinfo.Player.Id = user.Player.ID;
                foreach (var c in user.Player.Characters)
                {
                    NCharacterInfo info = new NCharacterInfo();
                    info.Id = 0;  //Entity ID
                    info.Name = c.Name;
                    info.Class = (CharacterClass)c.Class;
                    info.Type = CharacterType.Player;
                    info.Level = c.Level;
                    info.Tid = c.ID;
                    sender.Session.Response.userLogin.Userinfo.Player.Characters.Add(info);
                }
            }
            sender.SendResponse();
        }

        //创建角色的请求
        void OnCharacterCreate(NetConnection<NetSession> sender, UserCreateCharacterRequest request)
        {
            Log.InfoFormat("创建角色的姓名: Name:{0} 职业: Class:{1}", request.Name,request.Class);

            var existingCharacter = DBService.Instance.Entities.Characters.FirstOrDefault(c => c.Name == request.Name);

            //角色名字已存在
            if (existingCharacter != null)
            {
                sender.Session.Response.createChar = new UserCreateCharacterResponse();
                sender.Session.Response.createChar.Result = Result.Failed;
                sender.Session.Response.createChar.Errormsg = "角色名已存在,请重新输入";
                sender.SendResponse();
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
                Gold=1000000,
                Equips=new byte[28]
            };

            //初始化背包  背包表和角色表是一对一的关系
            var bag = new TCharacterBag();//TCharacterBag是数据库表
            bag.Owner = character;
            bag.Items = new byte[0];
            bag.Unlocked = 20;
            character.Bag = DBService.Instance.Entities.CharacterBags.Add(bag);
            character=DBService.Instance.Entities.Characters.Add(character);

            character.Items.Add(new TCharacterItem()
            {
                Owner=character,
                ItemID=1,
                ItemCount=20,
            });

            character.Items.Add(new TCharacterItem()
            {
                Owner = character,
                ItemID = 2,
                ItemCount = 20,
            });


            sender.Session.User.Player.Characters.Add(character);
            DBService.Instance.Entities.SaveChanges();


            //处理回发的消息
            sender.Session.Response.createChar = new UserCreateCharacterResponse();
            sender.Session.Response.createChar.Result = Result.Success;
            sender.Session.Response.createChar.Errormsg = "None";


            //返回新创建的角色

            NCharacterInfo newCharacterInfo = new NCharacterInfo();
            newCharacterInfo.Id = 0;  //Entity ID
            newCharacterInfo.Name = character.Name;
            newCharacterInfo.Class = (CharacterClass)character.Class;
            newCharacterInfo.Type = CharacterType.Player;
            newCharacterInfo.Level = character.Level;
            newCharacterInfo.Tid = character.ID;

            sender.Session.Response.createChar.Characters.Add(newCharacterInfo);

            sender.SendResponse();
        }

        //删除角色的请求
        void OnCharacterDelete(NetConnection<NetSession> sender, UserDeleteCharacterRequest request)
        {
            if (sender.Session.User == null)
            {
                return;
            }
            Log.InfoFormat("删除角色的姓名:{0}", request.Name);

            sender.Session.Response.deleteChar = new UserDeleteCharacterResponse();

            try
            {
                var deleteCharacter = sender.Session.User.Player.Characters.FirstOrDefault(c => c.Name == request.Name);

                //角色名字不存在 不执行删除操作
                if (deleteCharacter == null)
                {
                    sender.Session.Response.deleteChar.Result = Result.Failed;
                    sender.Session.Response.deleteChar.Errormsg = "角色不存在";
                }
                else
                {
                    // 从数据库删除角色
                    DBService.Instance.Entities.Characters.Remove(deleteCharacter);
                    sender.Session.User.Player.Characters.Remove(deleteCharacter);
                    DBService.Instance.Entities.SaveChanges();

                    sender.Session.Response.deleteChar.Result = Result.Success;
                    sender.Session.Response.deleteChar.Errormsg = "None";
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
                    sender.Session.Response.deleteChar.Characters.Add(characterInfo);
                }
            }
            catch (Exception ex)
            {
                Log.ErrorFormat("删除角色异常：{0}", ex.Message);
                sender.Session.Response.deleteChar.Result = Result.Failed;
                sender.Session.Response.deleteChar.Errormsg = "删除失败";
            }

            //消息打包成数据流发给客户端
            sender.SendResponse();
        }

        private void OnGameEnter(NetConnection<NetSession> sender, UserGameEnterRequest request)
        {
            TCharacter databaseCharacter = sender.Session.User.Player.Characters.ElementAt(request.characterIdx);
            Log.InfoFormat("UserGameEnterRequest: characterID:{0}:{1} Map:{2}", databaseCharacter.ID, databaseCharacter.Name, databaseCharacter.MapID);
            Character character = CharacterManager.Instance.CharacterAdd(databaseCharacter);

            sender.Session.Response.gameEnter = new UserGameEnterResponse();
            sender.Session.Response.gameEnter.Result = Result.Success;
            sender.Session.Response.gameEnter.Errormsg = "None";

            //进入成功,发送初始角色信息
            sender.Session.Response.gameEnter.Character = character.Info;

            //发送响应客户端
            sender.SendResponse();
            sender.Session.Character = character;
            MapManager.Instance[databaseCharacter.MapID].CharacterEnter(sender, character);
        }

        private void OnGameLeave(NetConnection<NetSession> sender, UserGameLeaveRequest request)
        {
            Character character = sender.Session.Character;
            Log.InfoFormat("UserGameLeaveRequest: characterID:{0}:{1} Map:{2}", character.Id, character.Info.Name, character.Info.mapId);

            CharacterLeave(character);

            sender.Session.Response.gameLeave = new UserGameLeaveResponse();
            sender.Session.Response.gameLeave.Result = Result.Success;
            sender.Session.Response.gameLeave.Errormsg = "None";

            sender.SendResponse();
        }

        public  void CharacterLeave(Character character)
        {
            CharacterManager.Instance.CharacterRemove(character.Id);
            MapManager.Instance[character.Info.mapId].CharacterLeave(character);
        }
    }
}
