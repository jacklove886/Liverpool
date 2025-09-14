using System;
using System.Collections;
using System.Collections.Generic;
using SkillBridge.Message;
using UnityEngine;
using UnityEngine.UI;

public class UITeamItem : ListView.ListViewItem
{
    public int index;
    public NCharacterInfo info;
    public Text characterName;
    public Image classIcon;//职业图标
    public Image LeaderIcon;//队长图标

    public Image background;
    public Sprite normalBg;
    public Sprite selectedBg;

    public override void OnSelected(bool selected)
    {
        background.overrideSprite = selected ? selectedBg : normalBg;
    }

    private void Awake()
    {
        background.enabled=false;
    }

    public void SetMemberInfo(int index, NCharacterInfo item, bool isLeader)
    {
        background.enabled = true;
        this.index = index;
        this.info = item;
        this.characterName.text = this.info.Level.ToString().PadRight(4)+"级"+ this.info.Name;
        this.classIcon.overrideSprite = SpriteManager.Instance.classIcons[(int)this.info.Class - 1];
        this.LeaderIcon.gameObject.SetActive(isLeader);
    }
}
