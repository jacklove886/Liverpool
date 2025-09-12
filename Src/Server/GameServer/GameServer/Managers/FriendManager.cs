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
        private Character Owner;

        private List<NFriendInfo> friends = new List<NFriendInfo>();//好友列表

        bool friendChanged = false;
        

        public FriendManager(Character character)//构造函数
        {
            this.Owner = character;
            this.InitFriends();
        }

        public void GetFriendInfos(List<NFriendInfo> list)
        {
            foreach(var f in this.friends)
            {
                list.Add(f);
            }      
        }

        public void InitFriends()
        {
            this.friends.Clear();
            foreach(var friend in this.Owner.Data.Friends)//数据库里的转换为好友信息
            {
                this.friends.Add(GetFriendInfo(friend));
            }
        }

        public void AddFriend(Character friend)
        {
            TCharacterFriend tf = new TCharacterFriend()
            {
                FriendID = friend.Id,
                FriendName = friend.Data.Name,
                Class = friend.Data.Class,
                Level = friend.Data.Level
            };
            this.Owner.Data.Friends.Add(tf);//存进数据表里
            friendChanged = true;
        }

        public bool RemoveFriendByFriendID(int friendId)
        {
            var removeId = this.Owner.Data.Friends.FirstOrDefault(v => v.FriendID == friendId);
            if (removeId != null)
            {
                DBService.Instance.Entities.CharacterFriends.Remove(removeId);
            }
            friendChanged = true;
            return true;
        }

        public bool RemoveFriendByID(int id)
        {
            var removeId = this.Owner.Data.Friends.FirstOrDefault(v => v.FriendID == id);
            if (removeId != null)
            {
                DBService.Instance.Entities.CharacterFriends.Remove(removeId);
            }
            friendChanged = true;
            return true;
        }

        private NCharacterInfo GetBasicInfo(NCharacterInfo info)
        {
            return new NCharacterInfo()
            {
                Id = info.Id,
                Name = info.Name,
                Class = info.Class,
                Level = info.Level
            };
        }

        public NFriendInfo GetFriendInfo(int friendId)
        {
            foreach (var f in this.friends)
            {
                if (f.friendInfo.Id == friendId)
                {
                    return f;
                }
            }
            return null;
        }

        public NFriendInfo GetFriendInfo(TCharacterFriend friend)//重载
        {
            NFriendInfo friendInfo = new NFriendInfo();
            var character = CharacterManager.Instance.GetCharatcer(friend.FriendID);
            friendInfo.friendInfo = new NCharacterInfo();
            friendInfo.Id = friend.Id;
            if (character == null)
            {
                friendInfo.friendInfo.Id = friend.FriendID;
                friendInfo.friendInfo.Name = friend.FriendName;
                friendInfo.friendInfo.Class = (CharacterClass)friend.Class;
                friendInfo.friendInfo.Level = friend.Level;
                friendInfo.Status = 0;//下线
            }
            else
            {
                friendInfo.friendInfo = GetBasicInfo(character.Info);
                friendInfo.friendInfo.Name = character.Info.Name;
                friendInfo.friendInfo.Class = character.Info.Class;
                friendInfo.friendInfo.Level = character.Info.Level;
                character.FriendManager.UpdateFriendInfo(this.Owner.Info, 1);
                friendInfo.Status = 1;
            }
            return friendInfo;
        }
       
        //更新在线状态
        public void UpdateFriendInfo(NCharacterInfo friendInfo,int status)
        {
            foreach(var f in this.friends)
            {
                if (f.friendInfo.Id == friendInfo.Id)
                {
                    f.Status = status;
                    break;
                }
            }
            friendChanged = true;
        }

        public void PostProcess(NetMessageResponse message)
        {
            if (friendChanged)
            {
                this.InitFriends();
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
