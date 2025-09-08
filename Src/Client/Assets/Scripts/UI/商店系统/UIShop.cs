using Common.Data;
using Managers;
using Models;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIShop : MonoBehaviour {

    public Text title;//标题
    public Text money;//价格

    private ShopDefine shop;
    public Transform[] itemRoot;//绑定的Content
    public GameObject shopItem;


    IEnumerator InitItems()
    {
        foreach(var kv in DataManager.Instance.ShopItems[shop.ID])
        {
            if(kv.Value.Status>0)//可出售状态
            {
                GameObject go = Instantiate(shopItem, itemRoot[0]);
                UIShopItem ui = go.GetComponent<UIShopItem>();
                ui.SetShopItem(kv.Key, kv.Value, this);
            }
        }
        yield return null;
    }

    public void SetShop(ShopDefine shop)
    {
        this.shop = shop;
        this.title.text = shop.Name;
        this.money.text = User.Instance.CurrentCharacter.Gold.ToString();
        StartCoroutine(InitItems());
    }

    private UIShopItem selectedItem;
    public void SelectShopItem(UIShopItem item)
    {
        if (selectedItem != null)
        {
            selectedItem.Selected = false;
        }
        selectedItem = item;
    }

    public void OnClickBuy()
    {
        if (this.selectedItem == null)
        {
            MessageBox.Show("请选择要购买的道具", "购买提示");
            return;
        }
        if (!ShopManager.Instance.BuyItem(this.shop.ID, this.selectedItem.shopItemID))
        {
            
        }
    }
}
