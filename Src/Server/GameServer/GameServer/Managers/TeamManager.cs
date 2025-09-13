using Common;
using GameServer.Entities;
using GameServer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameServer.Managers
{
    class TeamManager:Singleton<TeamManager>//管理所有队伍
    {
        public List<Team> Teams = new List<Team>();//方便遍历
        public Dictionary<int, Team> CharacterTeams = new Dictionary<int, Team>();//方便查询

        public void Init()
        {

        }

        //预留的扩展功能 暂时没有维护字典
        public Team GetTeamByCharacter(int characterId)
        {
            Team team = null;
            this.CharacterTeams.TryGetValue(characterId, out team);//通过characterId查询到他的team
            return team;
        }

        internal void AddTeamMember(Character leader, Character member)
        {
            if (leader.Team == null)//队长没有队伍
            {
                leader.Team = CreatTeam(leader);
            }
            leader.Team.AddMember(member);
        }

        private Team CreatTeam(Character leader)
        {
            Team team = null;
            for(int i = 0; i < this.Teams.Count; i++)//遍历所有的队伍
            {
                team = this.Teams[i];
                if (team.members.Count == 0)//如果是空队伍
                {
                    team.AddMember(leader);//把leader加到这个空队伍里
                    return team;
                }
            }
            team = new Team(leader);//已经保存过上面的信息 并new Team调用构造函数
            this.Teams.Add(team);
            team.Id = this.Teams.Count;
            return team;
        }
    }
}
