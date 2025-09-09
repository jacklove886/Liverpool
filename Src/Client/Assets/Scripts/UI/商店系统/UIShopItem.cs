using Common.Data;
using Managers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UIShopItem : MonoBehaviour,ISelectHandler
{

    [Header("商品栏的信息")]
    public Image iconImage;//图标
    public Text nameText;//名称
    public Text countText;//数量
    public Text priceText;//价格
    public Text limitClass;//限制的职业

    public Image background;//背景
    public Sprite normalBg;
    public Sprite selectBg;//选中的背景

    private bool selected;
    public bool Selected//属性
    {
        get { return selected; }
        set
        {
            selected = value;
            //选中即selectBg 未选中就是正常Bg
            this.background.overrideSprite = selected ? selectBg : normalBg;
        }
    }

    [Header("数据结构")]
    public UIShop shop;
    public int shopItemID { get; set; }
    public ItemDefine item;
    public ShopItemDefine shopItem;



    void Start () {
		
	}
	
	public void SetShopItem(int id,ShopItemDefine shopItem,UIShop owner)
    {
        this.shop = owner;
        this.shopItemID = id;
        this.shopItem = shopItem;//存储配置信息 里面有具体价格 数量 状态
        this.item = DataManager.Instance.Items[this.shopItem.ItemID];

        this.nameText.text = this.item.Name;
        this.countText.text = "x"+shopItem.Count.ToString();
        this.priceText.text = shopItem.Price.ToString();
        switch (this.item.LimitClass.ToString())
        {
            case "Warrior":  this.limitClass.text = "战士"; break;
            case "Wizard" :  this.limitClass.text = "法师"; break;
            case "Archer" :  this.limitClass.text = "游侠"; break; 
        }
        this.iconImage.overrideSprite = Resloader.Load<Sprite>(item.Icon);
    }

    //Unity内置接口
    public void OnSelect(BaseEventData eventData)//方法
    {
        this.Selected = true;
        this.shop.SelectShopItem(this);
    }
}
