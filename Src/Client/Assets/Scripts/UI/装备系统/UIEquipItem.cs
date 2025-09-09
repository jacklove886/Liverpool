using Managers;
using Models;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UIEquipItem : MonoBehaviour,IPointerClickHandler
{

    public Image icon;
    public Text title;
    public Text level;
    public Text LimitClass;
    public Text limitCategory;
    public Image background;
    public Sprite normalBg;
    public Sprite selectedBg;
    private bool selected;
    public bool Selected
    {
        get { return selected; }
        set
        {
            selected = value;
            this.background.sprite = selected ? selectedBg : normalBg;
        }
    }

    public int index { get; set; }
    private UICharEquip UICharEquip;

    private Item item;

    private bool isEquiped = false;//代表是否是装备列表 false为道具列表

    public void SetEquipItem(int idx,Item item,UICharEquip owner,bool equiped)
    {
        Debug.Log("SetEquipItem called: equiped = " + equiped);
        this.UICharEquip = owner;
        this.index = idx;
        this.item = item;
        this.isEquiped = equiped;
        this.icon.overrideSprite = Resloader.Load<Sprite>(this.item.Define.Icon);
        if (!isEquiped)
        {
            this.title.text = this.item.Define.Name;
            this.level.text = this.item.Define.Level.ToString();
            switch (this.item.Define.LimitClass.ToString())
            {
                case "Warrior": this.LimitClass.text = "战士"; break;
                case "Wizard": this.LimitClass.text = "法师"; break;
                case "Archer": this.LimitClass.text = "游侠"; break;
            }
            this.limitCategory.text = this.item.Define.Category;
        }       
    }

    //指针点击处理器  鼠标按下执行
    public void OnPointerClick(PointerEventData eventData)
    {
        if (this.isEquiped)//如果这个已经装备 
        {
            UnEquip();
        }
        else
        {
            if (this.selected)
            {
                DoEquip();
                this.Selected = false;
            }
            else
            {
                this.Selected = true;
            }
        }
    }

    private void DoEquip()
    {
        var msg = MessageBox.Show(string.Format("要装备{0}吗?", this.item.Define.Name), "确认", MessageBoxType.Confirm);
        msg.OnYes = () =>
        {
            var oldEquip = EquipManager.Instance.GetEquip(item.EquipInfo.Slot);
            if (oldEquip != null)
            {
                var newmsg= MessageBox.Show(string.Format("要替换掉{0}吗?", oldEquip.Define.Name), "确认", MessageBoxType.Confirm);
                newmsg.OnYes = () =>
                {
                    this.UICharEquip.DoEquip(this.item);
                };
            }
            else{
                this.UICharEquip.DoEquip(this.item);
            }  
        };
    }

    private void UnEquip()
    {
        var msg = MessageBox.Show(string.Format("要取下装备{0}吗?", this.item.Define.Name), "确认", MessageBoxType.Confirm);
        msg.OnYes = () =>
        {
            this.UICharEquip.DoUnEquip(this.item);
        };
    }
}
