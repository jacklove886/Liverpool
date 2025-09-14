using Managers;
using Models;
using Services;
using SkillBridge.Message;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIGuild : UIWindow
{
    public GameObject itemPrefab;
    public ListView listMain;
    public UIGuildInfo uiInfo;
    public UIGuildMemberItem selectedItem;

    public GameObject panelNormal;//普通成员界面
    public GameObject panelAdmin;//管理员界面
    public GameObject panelLeader;//会长界面

    private void Start()
    {
        GuildService.Instance.OnGuildUpdate += UpdateUI;
        this.listMain.onItemSelected += this.OnGuildMemberSelected;
        this.UpdateUI();
    }

    private void OnDestroy()
    {
        GuildService.Instance.OnGuildUpdate -= UpdateUI;
    }

    private void UpdateUI()
    {
        this.uiInfo.Info = GuildManager.Instance.guildInfo;

        ClearList();
        InitItems();

        this.panelNormal.SetActive(GuildManager.Instance.myMemberInfo.Position == GuildTitle.None);
        this.panelAdmin.SetActive(GuildManager.Instance.myMemberInfo.Position == GuildTitle.VicePresident);
        this.panelLeader.SetActive(GuildManager.Instance.myMemberInfo.Position == GuildTitle.President);
    }

    private void OnGuildMemberSelected(ListView.ListViewItem item)
    {
        this.selectedItem = item as UIGuildMemberItem;
    }

    private void InitItems()
    {
        foreach (var item in GuildManager.Instance.guildInfo.Members)
        {
            GameObject go = Instantiate(itemPrefab, this.listMain.transform);
            UIGuildMemberItem ui = go.GetComponent<UIGuildMemberItem>();
            ui.SetGuildInfo(item);
            this.listMain.AddItem(ui);
        }
    }

    private void ClearList()
    {
        this.listMain.RemoveAll();
    }

    public void OnClickApplyList()
    {
        UIManager.Instance.Show<UIGuildApplyList>();
    }

    public void OnClickKickout()
    {
        if (selectedItem == null)
        {
            MessageBox.Show("请选择要踢出的成员");
            return;
        }
        if (selectedItem.Info.Info.Id == User.Instance.CurrentCharacter.Id)
        {
            MessageBox.Show("不能踢自己哦");
            return;
        }
        MessageBox.Show(string.Format("要踢出[{0}]吗", selectedItem.Info.Info.Name), "踢出成员", MessageBoxType.Confirm, "强势踢出", "容我三思").OnYes = () =>
        {
            GuildService.Instance.SendAdminCommand(GuildAdminCommand.Kickout, this.selectedItem.Info.Info.Id);
       
        };
    }

    public void OnClickPromote()
    {
        if (selectedItem == null)
        {
            MessageBox.Show("请选择要晋升的成员");
            return;
        }
        if (selectedItem.Info.Info.Id == User.Instance.CurrentCharacter.Id)
        {
            MessageBox.Show("不能晋升自己哦");
            return;
        }
        if (selectedItem.Info.Position!= GuildTitle.None)
        {
            MessageBox.Show("对方身份也很高贵！");
            return;
        }
        MessageBox.Show(string.Format("要晋升[{0}]为魂斗罗吗", selectedItem.Info.Info.Name), "晋升成员", MessageBoxType.Confirm, "光荣升职", "容我三思").OnYes = () =>
        {
            GuildService.Instance.SendAdminCommand(GuildAdminCommand.Promote, this.selectedItem.Info.Info.Id);
        };
    }
    public void OnClickDepose()
    {
        if (selectedItem == null)
        {
            MessageBox.Show("请选择要罢免的成员");
            return;
        }
        if (selectedItem.Info.Info.Id == User.Instance.CurrentCharacter.Id)
        {
            MessageBox.Show("不能罢免自己哦");
            return;
        }
        if (selectedItem.Info.Position == GuildTitle.None)
        {
            MessageBox.Show("对方已经是魂师了！");
            return;
        }
        if (selectedItem.Info.Position == GuildTitle.President)
        {
            MessageBox.Show("胆敢挑衅封号斗罗！","大胆");
            return;
        }
        MessageBox.Show(string.Format("罢免[{0}]为魂师吗", selectedItem.Info.Info.Name), "罢免成员", MessageBoxType.Confirm, "降职", "容我三思").OnYes = () =>
        {
            GuildService.Instance.SendAdminCommand(GuildAdminCommand.Depose, this.selectedItem.Info.Info.Id);
        };
    }

    public void OnClickTransfer()
    {
        if (selectedItem==null)
        {
            MessageBox.Show("请选择要转让的成员");
            return;
        }
        if (selectedItem.Info.Info.Id == User.Instance.CurrentCharacter.Id)
        {
            MessageBox.Show("不能转让给自己哦");
            return;
        }
        MessageBox.Show(string.Format("要将封号斗罗转让给[{0}]吗", selectedItem.Info.Info.Name), "旺铺转让", MessageBoxType.Confirm, "转让", "容我三思").OnYes = () =>
        {
            GuildService.Instance.SendAdminCommand(GuildAdminCommand.Transfer, this.selectedItem.Info.Info.Id);
        };
    }
    public void OnClickSetNotice()
    {
        MessageBox.Show("扩展作业");
    }

    public void OnClickLeave()
    {
        MessageBox.Show("扩展作业");
    }

    public void OnClickChat()
    {
        MessageBox.Show("暂未开放");
    }
}
