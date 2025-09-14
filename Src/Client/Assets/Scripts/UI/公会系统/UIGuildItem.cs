using SkillBridge.Message;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIGuildItem : ListView.ListViewItem {

    public Text guildId;
    public Text guildName;
    public Text guildNumber;
    public Text guildLeader;

    public Image background;
    public Sprite normalBg;
    public Sprite selectedBg;

    public override void OnSelected(bool selected)
    {
        this.background.overrideSprite = selected ? selectedBg : normalBg;
    }

    public NGuildInfo Info;

    public void SetGuildInfo(NGuildInfo item)
    {
        Info = item;
        this.guildId.text = item.Id.ToString();
        this.guildName.text = item.GuildName;
        this.guildNumber.text = item.memberCount.ToString();
        this.guildLeader.text = item.leaderName;
    }
}
