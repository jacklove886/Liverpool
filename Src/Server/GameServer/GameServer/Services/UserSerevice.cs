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

            sender.Session.Response.userRegister = new UserRegisterResponse();

            TUser user = DBService.Instance.Entities.Users.Where(u => u.Username == request.User).FirstOrDefault();
            if (user != null)
            {
                sender.Session.Response.userRegister.Result = Result.Failed;
                sender.Session.Response.userRegister.Msg = "用户已存在.";
            }
            else
            {
                TPlayer player = DBService.Instance.Entities.Players.Add(new TPlayer());
                DBService.Instance.Entities.Users.Add(new TUser() { Username = request.User, Password = request.Passward, Player = player });
                DBService.Instance.Entities.SaveChanges();
                sender.Session.Response.userRegister.Result = Result.Success;
                sender.Session.Response.userRegister.Msg = "None";
            }
            sender.SendResponse();
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
                sender.Session.Response.userLogin.Msg = "用户不存在.";
            }
            else if (user.Password!=request.Passward)
            {
                sender.Session.Response.userLogin.Result = Result.Failed;
                sender.Session.Response.userLogin.Msg = "密码错误";
            }
            /*else if (SessionManager.Instance.GetSessionByUserId(user.ID))
            {
                sender.Session.Response.userLogin.Result = Result.Failed;
                sender.Session.Response.userLogin.Errormsg = "用户已登录.";
            }*/
            else
            {
                sender.Session.User = user;

                sender.Session.Response.userLogin.Result = Result.Success;
                sender.Session.Response.userLogin.Msg = "None";
                sender.Session.Response.userLogin.Userinfo = new NUserInfo();
                sender.Session.Response.userLogin.Userinfo.Id = (int)user.ID;
                sender.Session.Response.userLogin.Userinfo.Player = new NPlayerInfo();
                sender.Session.Response.userLogin.Userinfo.Player.Id = user.Player.ID;
                foreach (var c in user.Player.Characters)
                {
                    NCharacterInfo info = new NCharacterInfo();
                    info.Id = c.ID;
                    info.Name = c.Name;
                    info.Class = (CharacterClass)c.Class;
                    info.Type = CharacterType.Player;
                    info.Level = c.Level;
                    info.ConfigId = c.TID;
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
                sender.Session.Response.createChar.Msg = "角色名已存在,请重新输入";
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
                Gold = 5000,
                Equips = new byte[28],
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
            sender.Session.Response.createChar.Msg = "None";


            //返回新创建的角色

            NCharacterInfo newCharacterInfo = new NCharacterInfo();
            newCharacterInfo.Id = character.ID; 
            newCharacterInfo.Name = character.Name;
            newCharacterInfo.Class = (CharacterClass)character.Class;
            newCharacterInfo.Type = CharacterType.Player;
            newCharacterInfo.Level = character.Level;
            newCharacterInfo.ConfigId = character.TID;

            sender.Session.Response.createChar.Characters.Add(newCharacterInfo);

            sender.SendResponse();
        }

        private void OnGameEnter(NetConnection<NetSession> sender, UserGameEnterRequest request)
        {
            //进入游戏 只选择一个角色
            TCharacter databaseCharacter = sender.Session.User.Player.Characters.ElementAt(request.characterIdx);
            Log.InfoFormat("UserGameEnterRequest: characterID:{0}:{1} Map:{2}", databaseCharacter.ID, databaseCharacter.Name, databaseCharacter.MapID);
            Character character = CharacterManager.Instance.CharacterAdd(databaseCharacter);

            SessionManager.Instance.AddSession(character.Id, sender);//添加连接

            sender.Session.Response.gameEnter = new UserGameEnterResponse();
            sender.Session.Response.gameEnter.Result = Result.Success;
            sender.Session.Response.gameEnter.Msg = "None";

            //发送响应客户端  后处理赋值放在发送响应前
            sender.Session.Character = character;
            sender.Session.PostResponser = character;

            //进入成功,发送初始角色信息
            sender.Session.Response.gameEnter.Character = character.Info;
            sender.SendResponse();
         
            MapManager.Instance[databaseCharacter.MapID].CharacterEnter(sender, character);
        }

        private void OnGameLeave(NetConnection<NetSession> sender, UserGameLeaveRequest request)
        {
            Character character = sender.Session.Character;
            Log.InfoFormat("UserGameLeaveRequest: characterID:{0}:{1} Map:{2}", character.Id, character.Info.Name, character.Info.mapId);

            CharacterLeave(character);

            sender.Session.Response.gameLeave = new UserGameLeaveResponse();
            sender.Session.Response.gameLeave.Result = Result.Success;
            sender.Session.Response.gameLeave.Msg = "None";

            sender.SendResponse();
        }

        public void CharacterLeave(Character character)
        {
            Log.InfoFormat("角色离开 ID:{0},{1}", character.Id, character.Info.Name);
            SessionManager.Instance.Remove(character.Id);//移除连接
            CharacterManager.Instance.CharacterRemove(character.Id);
            character.Clear();
            MapManager.Instance[character.Info.mapId].CharacterLeave(character);
        }
    }
}
