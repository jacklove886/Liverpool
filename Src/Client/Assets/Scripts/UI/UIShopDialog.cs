using Managers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIShopDialog : UIWindow
{

    public Text title;//标题
    public Text Introduce;//介绍
    public Text YesButtonText;
    public Text NoButtonText;

    public int shopParam;//存储商店ID

    public override void OnYesClick()
    {
        Close(); // 关闭对话框
        ShopManager.Instance.ShowShop(shopParam);//NpcTestManager对shopParam赋值了
    }

    public override void OnNoClick()
    {
        Close();
        ShopManager.Instance.ShowSellShop();
    }
}
