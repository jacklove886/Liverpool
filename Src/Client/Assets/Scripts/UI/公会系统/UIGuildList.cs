using Managers;
using Services;
using SkillBridge.Message;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIGuildList : UIWindow {

    public GameObject itemPrefab;
    public ListView listMain;
    public Transform itemRoot;//Content
    public UIGuildInfo uiInfo;
    public UIGuildItem selectedItem;

    void Start ()
    {
        this.listMain.onItemSelected += this.OnGuildMemberSelected;
        this.uiInfo.Info = null;
        GuildService.Instance.OnGuildListResult += UpdateGuildList;//监听列表刷新
        GuildService.Instance.OnGuildListClose += Close;
        GuildService.Instance.SendGuildListRequest();//发送刷新请求
    }

    private void OnGuildMemberSelected(ListView.ListViewItem item)
    {
        this.selectedItem = item as UIGuildItem;
        this.uiInfo.Info = this.selectedItem.Info;//刷新界面
    }

    private void OnDestroy()
    {
        GuildService.Instance.OnGuildListResult -= UpdateGuildList;
    }

    private void UpdateGuildList(List<NGuildInfo> guilds)
    {
        ClearList();
        InitItems(guilds);
    }

    private void InitItems(List<NGuildInfo> guilds)//初始化公会列表
    {
        foreach(var guild in guilds)
        {
            GameObject go = Instantiate(itemPrefab, this.listMain.transform);
            UIGuildItem ui = go.GetComponent<UIGuildItem>();
            ui.SetGuildInfo(guild);
            this.listMain.AddItem(ui);
        }
        if (listMain != null && listMain.items.Count > 0)
        {
            listMain.SelectedItem = listMain.items[0];//默认选中第一个
        }
    }

    private void ClearList()
    {
        listMain.RemoveAll();
    }

    public void OnClickJoin()
    {
        if (selectedItem == null)
        {
            MessageBox.Show("请选择要加入的公会");
            return;
        }
        MessageBox.Show(string.Format("确定要加入公会[{0}]吗", selectedItem.Info.GuildName), "申请加入公会", MessageBoxType.Confirm).OnYes = () =>
        {
            GuildService.Instance.SendGuildJoinRequest(this.selectedItem.Info.Id);//发加入公会的请求
        };
    }

    void Close()
    {
        Close();
        GuildManager.Instance.ShowGuild();
    }
}
