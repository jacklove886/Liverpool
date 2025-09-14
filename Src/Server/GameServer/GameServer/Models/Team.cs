using Common;
using Common.Utils;
using GameServer.Entities;
using SkillBridge.Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameServer.Models
{
    public class Team
    {
        public int Id;
        public Character Leader;

        public List<Character> members = new List<Character>();

        public double changeTime;//队伍发生变化时 记录当前时间

        public Team(Character leader)//构造函数
        {
            this.AddMember(leader);
        }

        public void AddMember(Character member)
        {
            if (this.members.Count == 0)
            {
                this.Leader = member;
            }
            this.members.Add(member);//添加到列表里
            member.Team = this;//把成员的队伍指定为当前队伍
            changeTime = Time.changeTime;
        }

        public void Leave(Character member)
        {
            Log.InfoFormat("{0},{1}离开队伍", member.Id, member.Info.Name);
            this.members.Remove(member);
            if (member == this.Leader)
            {
                if (this.members.Count > 0)
                {
                    this.Leader = this.members[0];
                }
                else//队伍空了
                {
                    this.Leader = null;//清空队长
                }
            }
            member.Team = null;
            changeTime = TimeUtil.timestamp;
        }

        public void PostProcess(NetMessageResponse message)
        {
            if (message.teamInfo == null)
            {
                message.teamInfo = new TeamInfoResponse();
                message.teamInfo.Result = Result.Success;
                message.teamInfo.Team = new NTeamInfo();
                message.teamInfo.Team.Id = this.Id;
                message.teamInfo.Team.Leader = this.Leader.Id;
                foreach(var member in this.members)
                {
                    //Members是NCharacterInfo类型列表
                    message.teamInfo.Team.Members.Add(member.GetBasicInfo());
                }
            }
        }
    }
}
