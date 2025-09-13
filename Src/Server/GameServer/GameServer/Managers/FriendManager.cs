using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common;
using GameServer.Entities;
using GameServer.Services;
using SkillBridge.Message;

namespace GameServer.Managers
{
    public class FriendManager
    {
        private Character Character;//谁的好友管理器

        private List<NFriendInfo> friends = new List<NFriendInfo>();//好友列表

        bool friendChanged = false;//好友状态有没有变化
        

        public FriendManager(Character character)//构造函数
        {
            this.Character = character;
            this.InitFriendsList();
        }

        public void InitFriendsList()//初始化好友列表
        {
            this.friends.Clear();//清空好友列表(网络数据)
            foreach (var DBfriend in this.Character.TCharacter.Friends)//遍历数据库里的
            {
                this.friends.Add(GetFriendInfo(DBfriend));//转换并存储到网络数据中
            }
        }

        public void GetFriendInfos(List<NFriendInfo> list)
        {
            foreach(var friendInfo in this.friends)//遍历网络数据中的好友信息 存进Info.Friends 便于传输给客户端
            {
                list.Add(friendInfo);
            }      
        }

        public NFriendInfo GetFriendInfo(int friendId)//参数是characterID
        {
            foreach (var friendInfo in this.friends)//在网络数据中查找
            {
                if (friendInfo.friendInfo.Id == friendId)//f.friendInfo是NCharacterInfo
                {
                    return friendInfo;//如果找到ID相同的 就返回NFriendInfo 
                }
            }
            return null;
        }

        public NFriendInfo GetFriendInfo(TCharacterFriend DBfriend)//重载
        {
            NFriendInfo friendInfo = new NFriendInfo();
            var character = CharacterManager.Instance.GetCharatcer(DBfriend.FriendID);//dbfriend.FriendID就是好友的characterID
            friendInfo.friendInfo = new NCharacterInfo();
            friendInfo.Id = DBfriend.Id;//dbfriend.ID是那条记录的ID
            if (character == null)
            {
                friendInfo.friendInfo.Id = DBfriend.FriendID;//friendInfo.friendInfo是NCharacterInfo
                friendInfo.friendInfo.Name = DBfriend.FriendName;
                friendInfo.friendInfo.Class = (CharacterClass)DBfriend.Class;
                friendInfo.friendInfo.Level = DBfriend.Level;
                friendInfo.Status = 0;//下线
                //下线的时候由CharacterLeave调用TellFriendsLeaving更新状态为下线
            }
            else
            {
                friendInfo.friendInfo = character.GetBasicInfo();
                friendInfo.friendInfo.Name = character.Info.Name;
                friendInfo.friendInfo.Class = character.Info.Class;
                friendInfo.friendInfo.Level = character.Info.Level;
                if (DBfriend.Level != character.Info.Level)//好友等级和数据库的不一样就更新
                {
                    DBfriend.Level = character.Info.Level;
                }
                character.FriendManager.UpdateFriendInfo(this.Character.Info, 1);
                friendInfo.Status = 1;//上线
            }
            return friendInfo;
        }

        public void AddFriend(Character chafriend)
        {
            Log.InfoFormat("AddFriend: {0}({1}) 添加好友 {2}({3})",
            this.Character.Info.Name, this.Character.Id, chafriend.TCharacter.Name, chafriend.Id);
            TCharacterFriend tf = new TCharacterFriend()
            {
                FriendID = chafriend.Id,
                FriendName = chafriend.TCharacter.Name,
                Class = chafriend.TCharacter.Class,
                Level = chafriend.TCharacter.Level
            };
            //添加时EF能自动识别新对象
            this.Character.TCharacter.Friends.Add(tf);//存进数据库里 (缓存)需要搭配DBService
            friendChanged = true;
        }

        public bool RemoveFriendByFriendID(int friendId)
        {
            Log.InfoFormat("RemoveFriend: {0}({1}) 删除好友ID:{2}",
            this.Character.Info.Name, this.Character.Id, friendId);
            var removeId = this.Character.TCharacter.Friends.FirstOrDefault(v => v.FriendID == friendId);
            if (removeId != null)
            {
                //通过编号删除
                DBService.Instance.Entities.CharacterFriends.Remove(removeId);
            }
            friendChanged = true;
            return true;
        }

        public bool RemoveFriendByID(int id)
        {
            var removeId = this.Character.TCharacter.Friends.FirstOrDefault(v => v.Id == id);
            if (removeId != null)
            {
                //通过编号删除
                DBService.Instance.Entities.CharacterFriends.Remove(removeId);
            }
            friendChanged = true;
            return true;
        }
        
       
        //更新在线状态
        public void UpdateFriendInfo(NCharacterInfo friendInfo,int status)
        {
            foreach(var Nfriend in this.friends)
            {
                if (Nfriend.friendInfo.Id == friendInfo.Id)//f.friendInfo是NCharacterInfo
                {
                    Nfriend.Status = status;
                    break;
                }
            }
            friendChanged = true;
        }

        public void TellFriendsLeaving()//解决了下线不能即使通知好友的bug
        {
            foreach(var friendInfo in this.friends)//遍历所有好友
            {
                var chafriend = CharacterManager.Instance.GetCharatcer(friendInfo.friendInfo.Id);
                if(chafriend!=null)//如果好友在线 就通知他
                {
                    chafriend.FriendManager.UpdateFriendInfo(this.Character.Info, 0);
                }
            }
        }

        public void PostProcess(NetMessageResponse message)
        {
            if (friendChanged)//如果好友信息变化
            {
                Log.InfoFormat("PostProcess>FriendManager:characterID:{0}:{1}", this.Character.Id, this.Character.Info.Name);
                this.InitFriendsList();//初始化列表
                if (message.friendList == null)
                {
                    message.friendList = new FriendListResponse();
                    message.friendList.Friends.AddRange(this.friends);
                }
                friendChanged = false; 
            }
        }
    }
}
