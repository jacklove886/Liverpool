using SkillBridge.Message;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Common.Utils;

public class UIGuildMemberItem : ListView.ListViewItem
{

    public Text nickname;
    public Text @class;
    public Text level;
    public Text position;
    public Text joinTime;
    public Text status;

    public Image background;
    public Sprite normalBg;
    public Sprite selectedBg;

    public override void OnSelected(bool selected)
    {
        this.background.overrideSprite = selected ? selectedBg : normalBg;
    }


    public NGuildMemberInfo Info;


    public void SetGuildInfo(NGuildMemberInfo item)
    {
        this.Info = item;
        this.nickname.text = this.Info.Info.Name;
        this.@class.text = this.Info.Info.Class.ToString();
        this.level.text = this.Info.Info.Level.ToString();
        this.position.text = this.Info.Position.ToString();
        this.joinTime.text = TimeUtil.GetTime(this.Info.joinTime).ToShortDateString();
        this.status.text = this.Info.Status == 1 ? "在线" : TimeUtil.GetTime(this.Info.lastTime).ToString();
    }
}
