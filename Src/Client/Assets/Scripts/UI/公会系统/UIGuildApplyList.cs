using Managers;
using Services;
using SkillBridge.Message;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIGuildApplyList : UIWindow {

    public GameObject itemPrefab;
    public ListView listMain;
    public Transform itemRoot;//Content


    void Start ()
    {
        GuildService.Instance.OnGuildUpdate += UpdateList;//注册公会更新信息
        GuildService.Instance.SendGuildListRequest();//打开界面就发送消息给服务器 强制刷新
        this.UpdateList();
	}

    private void OnDestroy()
    {
        GuildService.Instance.OnGuildUpdate -= UpdateList;
    }

    void UpdateList()
    {
        ClearList();
        InitItems();
    }

    private void InitItems()
    {
        foreach(var item in GuildManager.Instance.guildInfo.Applies)
        {
            GameObject go = Instantiate(itemPrefab, this.listMain.transform);
            UIGuildApplyListItem ui = go.GetComponent<UIGuildApplyListItem>();
            ui.SetItemInfo(item);
            this.listMain.AddItem(ui);    
        }
    }

    private void ClearList()
    {
        listMain.RemoveAll();
    }
}
