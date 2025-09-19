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
        public TGuild TGuild;

        public int Id { get { return this.TGuild.Id; } }

        public string Name { get { return this.TGuild.Name; } }


        public double changeTime;

        public Guild(TGuild Tguild)//构造函数
        {
            this.TGuild = Tguild;
        }

        //加入公会申请
        public bool JoinApply(NGuildApplyInfo apply)
        {
            //从数据库里查找
            var oldApply = this.TGuild.Applies.FirstOrDefault(v => v.CharacterId == apply.characterId);
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
            this.TGuild.Applies.Add(dbApply);
            DBService.Instance.Save();

            this.changeTime = TimeUtil.timestamp;
            return true;
        }
        

        //审批
        public bool JoinAppove(NGuildApplyInfo apply)
        {
            //Result==0表示没审批过
            var oldApply = this.TGuild.Applies.FirstOrDefault(v => v.CharacterId == apply.characterId&&v.Result==0);
            if (oldApply == null)//没有这条申请请求
            {
                return false;
            }
            oldApply.Result = (int)apply.Result;
            if (apply.Result == ApplyResult.Accept)
            {
                this.AddMember(apply.characterId, apply.Name, apply.Class, apply.Level, GuildTitle.None);
            }
            if(apply.Result == ApplyResult.Reject)
            {
                var applyRecord = DBService.Instance.Entities.TGuildApplies.FirstOrDefault(v => v.Guild.Id == this.Id && v.CharacterId == apply.characterId);
                if (applyRecord != null)
                {
                    DBService.Instance.Entities.TGuildApplies.Remove(applyRecord);
                    this.TGuild.Applies.Remove(oldApply);//在网络中删除
                }
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
            this.TGuild.Members.Add(dbMember);
            var character = CharacterManager.Instance.GetCharatcer(characterId);
            if (character != null)//如果角色在线
            {
                character.TCharacter.GuildId = this.Id;
            }
            else//不在线 直接操作数据库  支持离线玩家被邀请进入公会
            {
                TCharacter dbcharacter = DBService.Instance.Entities.Characters.SingleOrDefault(c => c.ID == characterId);
                if(dbcharacter!=null)
                dbcharacter.GuildId = this.Id;
            }

            changeTime = TimeUtil.timestamp;
        }

        //离开公会
        public void Leave(Character member)
        {
            var dbMember = GetDBMember(member.Id);
            if (dbMember != null)
            {
                DBService.Instance.Entities.TGuildMembers.Remove(dbMember);
                this.TGuild.Members.Remove(dbMember);
            }
            member.TCharacter.GuildId = 0;
            member.Guild = null;

            if (TGuild.Members.Count == 0)//公会没人了
            {
                DisbandGuild();
            }
            DBService.Instance.Save();
            changeTime = TimeUtil.timestamp;  
        }

        private void DisbandGuild()
        {
            while (TGuild.Applies.Any())
            {
                var a = TGuild.Applies.First();
                TGuild.Applies.Remove(a); //
                DBService.Instance.Entities.TGuildApplies.Remove(a); 
            }
            var Guild = DBService.Instance.Entities.Guilds.FirstOrDefault(v => v.Id == this.Id);
            if (Guild != null)
            {
                GuildManager.Instance.Guilds.Remove(this.Id);
                GuildManager.Instance.GuildNames.Remove(this.Name);
                DBService.Instance.Entities.Guilds.Remove(Guild);
            }
        }


        public NGuildInfo GuildInfo(Character character)
        {
            NGuildInfo info = new NGuildInfo()//公会信息
            {
                Id = this.Id,
                GuildName = this.Name,
                Notice = this.TGuild.Notice,
                leaderId = this.TGuild.LeaderID,
                leaderName = this.TGuild.LeaderName,
                createTime = (long)TimeUtil.GetTimestamp(this.TGuild.CreateTime),
                memberCount = this.TGuild.Members.Count
            };          
            if (character != null)//说明是当前公会成员  可以看到成员信息
            {
                info.Members.AddRange(GetMemberInfos());
                var currentmemer = GetDBMember(character.Id);
                if (currentmemer!=null&&currentmemer.Position!=(int)GuildTitle.None)//不是普通成员
                {
                    info.Applies.AddRange(GetApplyInfos());//可以审批信息
                }
            }
            return info;
           
        }

        private List<NGuildMemberInfo> GetMemberInfos()
        {
            List<NGuildMemberInfo> members = new List<NGuildMemberInfo>();
            foreach(var member in this.TGuild.Members)
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
                }
                else//角色离线
                {
                    memberInfo.Info = this.GetMemberInfo(member);
                    memberInfo.Status = 0;
                }
                members.Add(memberInfo);
            }
            return members;
        }

        public NCharacterInfo GetMemberInfo(TGuildMember member)
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
            foreach(var apply in this.TGuild.Applies)
            {
                if (apply.Result != (int)ApplyResult.None) continue;//如果已经审批过 跳过这条审批 只发送未筛选的申请列表
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

        public TGuildMember GetDBMember(int characterId)
        {
            foreach(var member in this.TGuild.Members)//从数据库中查找成员
            {
                if (member.CharacterId == characterId)
                {
                    return member;
                }
            }
            return null;
        }

        internal void ExcuteAdmin(GuildAdminCommand command, int targetId, int soureId)
        {
            var target = GetDBMember(targetId);
            var source = GetDBMember(soureId);
            switch (command)
            {
                case GuildAdminCommand.Promote:
                    target.Position = (int)GuildTitle.VicePresident;
                    break;
                case GuildAdminCommand.Depose:
                    target.Position = (int)GuildTitle.None;
                    break;
                case GuildAdminCommand.Transfer:
                    target.Position = (int)GuildTitle.President;
                    source.Position = (int)GuildTitle.VicePresident;
                    this.TGuild.LeaderID = targetId;
                    this.TGuild.LeaderName = target.Name;
                    break;
                case GuildAdminCommand.Kickout:
                    Character targetCharacter = CharacterManager.Instance.GetCharatcer(targetId);
                    if (targetCharacter != null)//在线
                    {
                        targetCharacter.LeaveGuild();
                        var memberToRemove = this.TGuild.Members.FirstOrDefault(m => m.CharacterId == targetId);
                        if (memberToRemove != null)
                        {
                            DBService.Instance.Entities.TGuildMembers.Remove(memberToRemove); 
                            this.TGuild.Members.Remove(memberToRemove); 
                        }
                    }
                    else
                    {                       
                       RemoveGuildMember(targetId);
                    }
                    var removeApply = DBService.Instance.Entities.TGuildApplies.FirstOrDefault(v => v.CharacterId == targetId);//删除申请记录
                    if (removeApply != null)
                    {
                        DBService.Instance.Entities.TGuildApplies.Remove(removeApply);
                    }
                    break;
            }
            DBService.Instance.Save();
            changeTime = TimeUtil.timestamp;
        }

        public void RemoveGuildMember(int characterId)//离线操作专用
        {
            var removeGuildMember = DBService.Instance.Entities.TGuildMembers.FirstOrDefault(v => v.CharacterId == characterId);
            if (removeGuildMember != null)
            {
                DBService.Instance.Entities.TGuildMembers.Remove(removeGuildMember);
            }
            var removeCharacter = DBService.Instance.Entities.Characters.FirstOrDefault(v => v.ID == characterId);
            if (removeCharacter != null)
            {
                removeCharacter.GuildId = 0;
            }
        }


        public void PostProcess(Character character, NetMessageResponse message)
        {
            if (message.Guild == null&&character.Guild==this)
            {
                message.Guild = new GuildResponse();
                message.Guild.Result = Result.Success;
                message.Guild.Guild = this.GuildInfo(character);
            }
        }

    }
}
