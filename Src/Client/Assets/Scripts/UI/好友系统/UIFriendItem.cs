using SkillBridge.Message;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIFriendItem : ListView.ListViewItem {

    public Text chaCame;
    public Text chaClass;
    public Text level;
    public Text status;

    public Image background;
    public Sprite normalBg;
    public Sprite selectedBg;
    //private bool isEquiped = false;

    public override void OnSelected(bool selected)
    {
        background.sprite = selected ? selectedBg : normalBg;
    }

    public NFriendInfo Info;

    public void SetFriendInfo(NFriendInfo itemInfo)
    {
        Info = itemInfo;
        chaCame.text = Info.friendInfo.Name;
        chaClass.text = Info.friendInfo.Class.ToString();
        level.text = (Info.friendInfo.Level+" 级").ToString();
        status.text = Info.Status == 1 ? "在线" : "离线";
    }
}
