using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIQuestItem : ListView.ListViewItem {

    public Text title;
    public Image background;
    public Sprite normalBg;
    public Sprite selectBg;

    public override void onSelected(bool selected)
    {
        this.background.overrideSprite= selected?selectBg: normalBg;
    }

    //public Quest quest;
    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
