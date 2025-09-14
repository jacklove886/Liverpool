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
        switch (this.Info.Info.Class.ToString())
        {
            case "Warrior": this.@class.text = "战士"; break;
            case "Wizard": this.@class.text = "法师"; break;
            case "Archer": this.@class.text = "游侠"; break;
        }
        this.level.text = this.Info.Info.Level.ToString();
        switch ((int)this.Info.Position)
        {
            case 0:
                this.position.text= "魂师"; break;
            case 1: 
                this.position.text = "封号斗罗"; break;
            case 2: 
                this.position.text = "魂斗罗"; break;
        }
        this.joinTime.text = TimeUtil.GetTime(this.Info.joinTime).ToShortDateString();
        this.status.text = this.Info.Status == 1 ? "在线" : TimeUtil.GetTime(this.Info.lastTime).ToString();
    }
}
