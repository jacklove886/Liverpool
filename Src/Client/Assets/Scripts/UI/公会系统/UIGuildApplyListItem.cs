using SkillBridge.Message;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Common.Utils;
using System;
using Services;

public class UIGuildApplyListItem : ListView.ListViewItem
{
    public Text nickname;
    public Text @class;
    public Text level;

    public NGuildApplyInfo Info;

    internal void SetItemInfo(NGuildApplyInfo item)
    {
        this.Info = item;
        this.nickname.text = this.Info.Name;
        switch (this.Info.Class.ToString())
        {
            case "Warrior": this.@class.text = "战士"; break;
            case "Wizard": this.@class.text = "法师"; break;
            case "Archer": this.@class.text = "游侠"; break;
        }
        this.level.text = this.Info.Level.ToString();
    }

    public void OnClickAccept()
    {
        MessageBox.Show(string.Format("要通过:[{0}]的公会申请吗", Info.Name), "审批申请", MessageBoxType.Confirm, "同意", "拒绝")
        .OnYes = () =>
        {
            GuildService.Instance.SendGuildJoinApply(true, this.Info);
        };
    }

    public void OnClickReject()
    {

        MessageBox.Show(string.Format("要通过:[{0}]的公会申请吗", Info.Name), "审批申请", MessageBoxType.Confirm, "同意", "拒绝")
        .OnYes = () =>
        {
            GuildService.Instance.SendGuildJoinApply(false, this.Info);
        };
    }
}
