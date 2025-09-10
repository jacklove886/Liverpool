using Models;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIQuestItem : ListView.ListViewItem {

    public Text title;
    public Image background;
    public Sprite normalBg;
    public Sprite selectBg;
    public Quest quest;

public override void onSelected(bool selected)
    {
        this.background.overrideSprite= selected?selectBg: normalBg;
    }

    void Start () {
		
	}
	


    public void SetQuestInfo(Quest item)
    {
        this.quest = item;
        if (this.title != null)
        {
            this.title.text = quest.Define.Name;
        }
    }
}
