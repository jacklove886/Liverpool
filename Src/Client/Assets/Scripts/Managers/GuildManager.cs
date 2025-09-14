using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SkillBridge.Message;

namespace Managers
{
    class GuildManager : Singleton<GuildManager>
    {
        public NGuildInfo guildInfo;//公会信息

        public NGuildMemberInfo myMemberInfo;

        public bool HasGuild//是否有公会
        {
            get { return this.guildInfo != null; }
        }

        public void Init(NGuildInfo guild)
        {
            this.guildInfo = guild;
        }

        public void ShowGuild()//点击公会图标
        {
            if (this.HasGuild)
            {
                UIManager.Instance.Show<UIGuild>();
            }
            else
            {
                var win = UIManager.Instance.Show<UIGuildPopNoGuild>();//显示创建公会的界面
                win.OnClose += PopNoGuild_OnClose;
            }
        }

        private void PopNoGuild_OnClose(UIWindow sender, UIWindow.WindowResult result)
        {
            if (result == UIWindow.WindowResult.Yes)
            {
                //创建公会
                UIManager.Instance.Show<UIGuildPopCreate>();
            }
            else if(result == UIWindow.WindowResult.No)
            {
                //加入公会
                UIManager.Instance.Show<UIGuildList>();
            }
        }
    }
}
