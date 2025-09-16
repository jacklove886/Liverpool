using Managers;
using Models;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UIRideItem : ListView.ListViewItem
{
    public Image icon;
    public Text title;
    public Text level;
    public Text LimitClass;
    public Text limitCategory;
    public Image background;
    public Sprite normalBg;
    public Sprite selectedBg;
    public Item item;

    public override void OnSelected(bool selected)
    {
        background.overrideSprite = selected ? selectedBg : normalBg;
    }


    public void SetRideItem(Item item)
    {
        this.item = item;
        this.title.text = this.item.Define.Name;
        this.level.text = ("LV  " + this.item.Define.Level).ToString();
        switch (this.item.Define.LimitClass.ToString())
        {
            case "Warrior": this.LimitClass.text = "战士"; break;
            case "Wizard": this.LimitClass.text = "法师"; break;
            case "Archer": this.LimitClass.text = "游侠"; break;
        }
        this.limitCategory.text = this.item.Define.Category;  //装备类型
        this.icon.overrideSprite = Resloader.Load<Sprite>(this.item.Define.Icon);
    }

}
