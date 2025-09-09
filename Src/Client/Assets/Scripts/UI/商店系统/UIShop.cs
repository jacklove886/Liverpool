using Common.Data;
using Managers;
using Models;
using Services;
using SkillBridge.Message;
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

    private void Start()
    {

        StatusService.Instance.RegisterStatusNofity(StatusType.Money, OnMoneyChanged);
    }

    private void OnDestroy()
    {
        StatusService.Instance.UnregisterStatusNotify(StatusType.Money, OnMoneyChanged);
    }


    IEnumerator InitItems()
    {
        //DataManager.Instance.ShopItems是嵌套字典结构，外层键是商店ID，内层键是商品ID,Value是商品配置
        foreach (var kv in DataManager.Instance.ShopItems[shop.ID])
        {
            if(kv.Value.Status>0)//可出售状态
            {
                //暂时都放在第一个Content容器里
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
        selectedItem = item;//将传入的item作为当前选择的Item
    }

    //绑定在购买按钮上
    public void OnClickBuy()
    {
        //没有选中的商品
        if (this.selectedItem == null)
        {
            MessageBox.Show("请选择要购买的道具", "购买提示");
            return;
        }
        ShopManager.Instance.BuyItem(this.shop.ID, this.selectedItem.shopItemID);
    }

    public void OnClickClose()
    {
        UIManager.Instance.Close(typeof(UIShop));
    }

    private bool OnMoneyChanged(Nstatus status)
    {
        money.text = User.Instance.CurrentCharacter.Gold.ToString();
        return true;
    }
}
