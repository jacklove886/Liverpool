using Managers;
using Models;
using Services;
using SkillBridge.Message;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class UIGuild : UIWindow
{
    public GameObject itemPrefab;
    public ListView listMain;
    public UIGuildInfo UIGuildInfo;
    public UIGuildMemberItem selectedItem;

    public GameObject panelNormal;//普通成员界面
    public GameObject panelAdmin;//管理员界面
    public GameObject panelLeader;//会长界面

    private void Start()
    {
        GuildService.Instance.OnGuildUpdate += UpdateUI;
        GuildService.Instance.OnGuildClose += CloseUIGuild;
        this.listMain.onItemSelected += this.OnGuildMemberSelected;  
        this.UpdateUI();
    }

    private void OnDestroy()
    {
        GuildService.Instance.OnGuildUpdate -= UpdateUI;
    }

    private void UpdateUI()
    {
        this.UIGuildInfo.Info = GuildManager.Instance.guildInfo;

        ClearList();
        InitItems();

        if (GuildManager.Instance!=null&&GuildManager.Instance.myMemberInfo != null)
        {
            this.panelNormal.SetActive(GuildManager.Instance.myMemberInfo.Position == GuildTitle.None);
            this.panelAdmin.SetActive(GuildManager.Instance.myMemberInfo.Position == GuildTitle.VicePresident);
            this.panelLeader.SetActive(GuildManager.Instance.myMemberInfo.Position == GuildTitle.President);
        }        
    }

    private void OnGuildMemberSelected(ListView.ListViewItem item)
    {
        this.selectedItem = item as UIGuildMemberItem;
    }

    private void InitItems()
    {
        var sortitems= GuildManager.Instance.guildInfo.Members.OrderBy(v=>v.Position== GuildTitle.President?0:v.Position==GuildTitle.VicePresident?1:2);//排序 会长在上面
        foreach (var item in sortitems)
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
        MessageBox.Show(string.Format("要晋升[{0}]为副会长吗", selectedItem.Info.Info.Name), "晋升成员", MessageBoxType.Confirm, "光荣升职", "容我三思").OnYes = () =>
        {
            GuildService.Instance.SendGuildAdmin(GuildAdminCommand.Promote, this.selectedItem.Info.Info.Id);
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
            MessageBox.Show("对方已经是普通成员了！");
            return;
        }
        if (selectedItem.Info.Position == GuildTitle.President)
        {
            MessageBox.Show("胆敢挑衅会长！","大胆");
            return;
        }
        MessageBox.Show(string.Format("罢免[{0}]为普通成员吗", selectedItem.Info.Info.Name), "罢免成员", MessageBoxType.Confirm, "降职", "容我三思").OnYes = () =>
        {
            GuildService.Instance.SendGuildAdmin(GuildAdminCommand.Depose, this.selectedItem.Info.Info.Id);
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
        MessageBox.Show(string.Format("要将会长转让给[{0}]吗", selectedItem.Info.Info.Name), "旺铺转让", MessageBoxType.Confirm, "转让", "容我三思").OnYes = () =>
        {
            GuildService.Instance.SendGuildAdmin(GuildAdminCommand.Transfer, this.selectedItem.Info.Info.Id);
        };
    }
    public void OnClickKickOut()
    {
        if (selectedItem == null)
        {
            MessageBox.Show("请选择踢出的成员");
            return;
        }
        if (selectedItem.Info.Info.Id == User.Instance.CurrentCharacter.Id)
        {
            MessageBox.Show("不能踢自己哦");
            return;
        }
        if ((int)GuildManager.Instance.myMemberInfo.Position<= (int)selectedItem.Info.Position)
        {
            MessageBox.Show("只能踢出职位比你低的成员！");
            return;
        }
        MessageBox.Show(string.Format("确定要踢出[{0}]吗", selectedItem.Info.Info.Name), "踢出成员", MessageBoxType.Confirm, "强势踢出", "容我三思").OnYes = () =>
        {
            GuildService.Instance.SendGuildAdmin(GuildAdminCommand.Kickout, selectedItem.Info.Info.Id);
        };
    }

    public void OnClickLeave()
    {
        if (GuildManager.Instance.guildInfo.memberCount > 1)
        {
            if (GuildManager.Instance.myMemberInfo.Position == GuildTitle.President)
            {
                MessageBox.Show("离开公会前请转让会长职位", "提示");
                return;
            }
        }      
        MessageBox.Show(string.Format("确定要离开[{0}]吗", UIGuildInfo.Info.GuildName), "离开公会", MessageBoxType.Confirm, "离开", "容我三思").OnYes = () =>
        {
            GuildService.Instance.SendGuildLeaveRequest(GuildManager.Instance.myMemberInfo.Info.Guild.Id, User.Instance.CurrentCharacter.Id);
        };
    }

    void CloseUIGuild()
    {
        Close();
    }

    public void OnClickChat()
    {
        if (selectedItem == null)
        {
            MessageBox.Show("请选择要私聊的好友", "私聊");
        }
        if (selectedItem.Info.Id == User.Instance.CurrentCharacter.Id)
        {
            MessageBox.Show("不能私聊自己哦", "私聊");
        }
        if (selectedItem != null)
        {
            ChatManager.Instance.StartPrivateChat(selectedItem.Info.Info.Id, selectedItem.Info.Info.Name);
            Close();
        }        

    }
}
