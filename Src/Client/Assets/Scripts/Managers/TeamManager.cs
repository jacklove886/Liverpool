using Models;
using SkillBridge.Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Managers
{
    class TeamManager: Singleton<TeamManager>
    {
        public void Init()
        {

        }

        public void UpdateTeamInfo(NTeamInfo team)
        {
            User.Instance.TeamInfo = team;
            ShowTeamUI(team != null);
        }

        public void ShowTeamUI(bool show)
        {
            if (UIMain.Instance != null)//判断主UI存不存在 有可能正在切换场景
            {
                UIMain.Instance.ShowTeamUI(show);
            }
        }

    }
}
