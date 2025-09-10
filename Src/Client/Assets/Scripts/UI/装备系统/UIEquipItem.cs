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

    //设置信息
    public void SetEquipItem(int idx,Item item,UICharEquip UICharEquip, bool equiped)
    {
        Debug.Log("SetEquipItem called: equiped = " + equiped);
        this.UICharEquip = UICharEquip;
        this.index = idx;//保存装备的索引
        this.item = item;//保存道具对象
        this.isEquiped = equiped;      
        if (!isEquiped)
        {
            this.title.text = this.item.Define.Name;
            this.level.text = ("LV  "+this.item.Define.Level).ToString();
            switch (this.item.Define.LimitClass.ToString())
            {
                case "Warrior": this.LimitClass.text = "战士"; break;
                case "Wizard": this.LimitClass.text = "法师"; break;
                case "Archer": this.LimitClass.text = "游侠"; break;
            }
            this.limitCategory.text = this.item.Define.Category;  //装备类型
        }
        //两个列表的图标都要显示
        this.icon.overrideSprite = Resloader.Load<Sprite>(this.item.Define.Icon);
    }

    //指针点击处理器  鼠标按下执行
    public void OnPointerClick(PointerEventData eventData)
    {
        if (this.isEquiped)//如果这个已经装备 
        {
            UnEquip();
        }
        else//如果没装备
        {
            if (this.selected)//表示当前已选中
            {
                DoEquip();//穿装备
                this.UICharEquip.SelectEquipItem(null);//清除选中项
            }
            else//当前没选中
            {
                this.UICharEquip.SelectEquipItem(this);//设为选中状态
            }
        }
    }

    private void DoEquip()
    {
        var msg = MessageBox.Show(string.Format("要装备{0}吗?", this.item.Define.Name), "确认", MessageBoxType.Confirm);
        msg.OnYes = () =>
        {
            //获取槽位上的装备
            var oldEquip = EquipManager.Instance.GetEquip(item.EquipInfo.Slot);
            if (oldEquip != null)//如果不为空 说明已有装备
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
