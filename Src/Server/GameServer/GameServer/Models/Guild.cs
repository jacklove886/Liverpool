using Common;
using Common.Utils;
using GameServer.Entities;
using GameServer.Managers;
using GameServer.Services;
using SkillBridge.Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameServer.Models
{
    public class Guild//限于当前公会的事情
    {
        public TGuild Data;

        public int Id { get { return this.Data.Id; } }
        public Character Leader;

        public string Name { get { return this.Data.Name; } }

        public List<Character> Members = new List<Character>();

        public double changeTime;

        public Guild(TGuild Tguild)//构造函数
        {
            this.Data = Tguild;
        }

        //加入公会申请
        public bool JoinApply(NGuildApplyInfo apply)
        {
            //从数据库里查找
            var oldApply = this.Data.Applies.FirstOrDefault(v => v.CharacterId == apply.characterId);
            if (oldApply != null)//已经申请过了
            {
                return false;
            }

            var dbApply = DBService.Instance.Entities.TGuildApplies.Create();
            dbApply.GuildId = apply.GuildId;
            dbApply.CharacterId = apply.characterId;
            dbApply.Name = apply.Name;
            dbApply.Class = apply.Class ;
            dbApply.Level = apply.Level;
            dbApply.ApplyTime = DateTime.Now;

            //公会申请表里将这个信息加进去
            //DBService.Instance.Entities.TGuildApplies.Add(dbApply);这句话可能是多余的  因为virtual属性会被EF自动跟踪
            this.Data.Applies.Add(dbApply);
            DBService.Instance.Save();

            this.changeTime = TimeUtil.timestamp;
            return true;
        }
        

        //审批
        public bool JoinAppove(NGuildApplyInfo apply)
        {
            //Result==0表示没审批过
            var oldApply = this.Data.Applies.FirstOrDefault(v => v.CharacterId == apply.characterId&&v.Result==0);
            if (oldApply == null)//没有这条申请请求
            {
                return false;
            }
            oldApply.Result = (int)apply.Result;
            if (apply.Result == ApplyResult.Accept)
            {
                this.AddMember(apply.characterId, apply.Name, apply.Class, apply.Level, GuildTitle.None);
            }

            DBService.Instance.Save();
            this.changeTime = TimeUtil.timestamp;
            return true;
        }

        public void AddMember(int characterId, string name, int @class, int level, GuildTitle position)
        {
            DateTime now = DateTime.Now;
            TGuildMember dbMember = new TGuildMember()
            {
                CharacterId = characterId,
                Name = name,
                Class=@class,
                Level=level,
                Position= (int)position,
                JoinTime=now,
                LastTime=now,
                GuildId=this.Id
            };
            this.Data.Members.Add(dbMember);
            changeTime = TimeUtil.timestamp;
        }


        //离开逻辑 作业
        public void Leave(Character member)
        {
            
        }

        public void PostProcess(Character character,NetMessageResponse message)
        {
            if (message.Guild == null)
            {
                message.Guild = new GuildResponse();
                message.Guild.Result = Result.Success;
                message.Guild.Guild = this.GuildInfo(character);
            }
        }

        internal NGuildInfo GuildInfo(Character character)
        {
            NGuildInfo info = new NGuildInfo()//公会信息
            {
                Id = this.Id,
                GuildName = this.Name,
                Notice = this.Data.Notice,
                leaderId = this.Data.LeaderID,
                leaderName = this.Data.LeaderName,
                createTime = (long)TimeUtil.GetTimestamp(this.Data.CreateTime),
                memberCount = this.Data.Members.Count
            };
            if (character != null)//说明是当前公会成员  可以看到成员信息
            {
                info.Members.AddRange(GetMemberInfos());
                if (character.Id == this.Data.LeaderID)//判断是封号斗罗
                {
                    info.Applies.AddRange(GetApplyInfos());//可以审批信息
                }
            }
            return info;
           
        }

        private List<NGuildMemberInfo> GetMemberInfos()
        {
            List<NGuildMemberInfo> members = new List<NGuildMemberInfo>();
            foreach(var member in this.Data.Members)
            {
                var memberInfo = new NGuildMemberInfo()
                {
                    Id = member.Id,
                    characterId = member.CharacterId,
                    Position = (GuildTitle)member.Position,
                    joinTime = (long)TimeUtil.GetTimestamp(member.JoinTime),
                    lastTime = (long)TimeUtil.GetTimestamp(member.LastTime),
                };
                //应该增加更多检查
                var character = CharacterManager.Instance.GetCharatcer(member.CharacterId);
                if (character != null)//更新角色信息
                {
                    memberInfo.Info = character.GetBasicInfo();
                    memberInfo.Status = 1;
                    member.Level = character.TCharacter.Level;
                    member.Name = character.TCharacter.Name;
                    member.LastTime = DateTime.Now;
                    if (member.Id == this.Data.LeaderID)
                    {
                        this.Leader = character;
                    }
                }
                else//角色离线
                {
                    memberInfo.Info = this.GetMemberInfo(member);
                    memberInfo.Status = 0;
                    if (member.Id == this.Data.LeaderID)
                    {
                        this.Leader = null;
                    }
                }
                members.Add(memberInfo);
            }
            return members;
        }

        private NCharacterInfo GetMemberInfo(TGuildMember member)
        {
            return new NCharacterInfo()
            {
                Id = member.CharacterId,
                Name = member.Name,
                Class = (CharacterClass)member.Class,
                Level = member.Level
            };
        }

        private List<NGuildApplyInfo> GetApplyInfos()
        {
            List<NGuildApplyInfo> applies = new List<NGuildApplyInfo>();
            foreach(var apply in this.Data.Applies)
            {
                applies.Add(new NGuildApplyInfo()
                {
                    characterId = apply.CharacterId,
                    GuildId = apply.GuildId,
                    Class = apply.Class,
                    Level = apply.Level,
                    Name=apply.Name,
                    Result=(ApplyResult)apply.Result
                });
            }
            return applies;
        }

    }
}
