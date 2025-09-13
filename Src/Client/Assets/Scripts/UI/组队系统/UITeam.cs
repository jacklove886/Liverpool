using Models;
using Services;
using SkillBridge.Message;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UITeam : UIWindow
{
    public ListView list;
    public Text text;
    public UITeamItem[] items;
    private NTeamInfo teamInfo;

    void Start ()
    {
        if (User.Instance.TeamInfo == null)//如果没有队伍
        {
            this.gameObject.SetActive(false);          
        }
        else{
            foreach (var item in items)
            {
                list.AddItem(item);
            }
        }      
        teamInfo = User.Instance.TeamInfo;

    }

    private void OnEnable()//打开的时候更新组队信息
    {
        UpdateTeamUI();
    }

    public void ShowTeam(bool show)
    {
        this.gameObject.SetActive(show);
        if (show)
        {
            UpdateTeamUI();
        }
    }

    private void UpdateTeamUI()
    {
        if (teamInfo == null) return;
        this.text.text = string.Format("我的队伍({0}/5", teamInfo.Members.Count);

        for(int i = 0; i < 5; i++)
        {
            if(i< teamInfo.Members.Count)
            {
                //判断i队员的ID是不是队长的ID
                this.items[i].SetMemberInfo(i, teamInfo.Members[i], teamInfo.Members[i].Id==teamInfo.Leader);
                this.items[i].gameObject.SetActive(true);
            }
            else
            {
                this.items[i].gameObject.SetActive(false);
            }
        }

    }

    public void OnClickLeave()
    {
        MessageBox.Show("确定要离开队伍吗", "退出队伍", MessageBoxType.Confirm).OnYes = () =>
          {
              TeamService.Instance.SendTeamLeaveRequest(teamInfo.Id);
          };
    }



}
