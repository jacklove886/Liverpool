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

public class UIShop : UIWindow {

    public Text title;//标题
    public Text money;//价格

    public PageView pageView;
    private ShopDefine shop;
    public Transform[] itemRoot;//绑定的Content
    public GameObject shopItem;

    public Button sellbutton;
    public Text sellbuttonText;

    private void Start()
    {
        StatusService.Instance.RegisterStatusNofity(StatusType.Money, OnMoneyChanged);
    }

    private void OnDestroy()
    {
        StatusService.Instance.UnregisterStatusNotify(StatusType.Money, OnMoneyChanged);
    }


    IEnumerator InitItems()//初始化道具
    {
        int count = 0;
        int page = 0;
        //DataManager.Instance.ShopItems是嵌套字典结构，外层键是商店ID，内层键是商品ID,Value是商品配置
        foreach (var kv in DataManager.Instance.ShopItems[shop.ID])
        {
            if(kv.Value.Status>0)//可出售状态
            {
                if (count >=16)//如果道具超过16个 超过的放到下一页
                {
                    count = 0;
                    page++;  
                    if (page >= itemRoot.Length)//页码超过总的页数  目前总共两页
                    {
                        MessageBox.Show("商品数量过多!", "错误提示");
                        break;
                    }
                }
                GameObject go = Instantiate(shopItem, itemRoot[page]);
                UIShopItem ui = go.GetComponent<UIShopItem>();
                ui.SetShopItem(kv.Key, kv.Value, this);
                count++;   
            }
        }
        yield return null;
    }

    IEnumerator InitSellItems()
    {
        int count = 0;
        int page = 0;
        foreach(var kv in ItemManager.Instance.Items)
        {
            if (count >= 16)
            {
                count = 0;
                page++;
                if (page >= itemRoot.Length)
                {
                    MessageBox.Show("商品数量过多!", "错误提示");
                    break;
                }
            }
            GameObject go = Instantiate(shopItem, itemRoot[page]);
            UIShopItem ui = go.GetComponent<UIShopItem>();
            ui.SetSellItem(kv.Key,kv.Value);
            count++;
        }
        yield return null;
    }

    public void SetShop(ShopDefine shop)
    {
        this.sellbutton.onClick.AddListener(OnClickBuy);
        this.sellbuttonText.text = "购买";
        this.shop = shop;
        this.title.text = shop.Name;
        this.money.text = User.Instance.CurrentCharacter.Gold.ToString();
        StartCoroutine(InitItems());
    }

    public void SetSellShop(Item item)
    {
        this.sellbutton.onClick.AddListener(OnClickSell);
        this.sellbuttonText.text = "出售";
        this.title.text = "出售商品";
        this.money.text = item.Define.SellPrice.ToString();
        StartCoroutine(InitSellItems());
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
            MessageBox.Show("请选择要购买的道具", "提示");
            return;
        }
        ShopManager.Instance.BuyItem(this.shop.ID, this.selectedItem.shopItemID);
    }

    //绑定在购买按钮上
    public void OnClickSell()
    {
        //没有选中的商品
        if (this.selectedItem == null)
        {
            MessageBox.Show("请选择要出售的道具", "提示");
            return;
        }
        UIInputBox ui = UIManager.Instance.Show<UIInputBox>();
        ui.title.text = "出售商品";
        ui.emptyTips = "数量不能为空";
        ui.message.text = "请输入要出售的数量";
        ui.OnSubmit += (string text, out string tips) =>
          {
              int num;
              if (!int.TryParse(text, out num) || num <= 0)
              {
                  tips = "请输入大于0的数字";
                  return false;
              }
              if(num > selectedItem.sellItem.Count)
              {
                  tips = "出售数量超过拥有的数量";
                  return false;
              }
              ShopManager.Instance.SellItem(this.selectedItem.shopItemID, num);
              tips = "";
              return true;
          };       
    }

    private bool OnMoneyChanged(Nstatus status)
    {
        money.text = User.Instance.CurrentCharacter.Gold.ToString();
        return true;
    }
}
