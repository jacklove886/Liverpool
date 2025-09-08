using Managers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIDialog : UIWindow{

    public Text title;//标题
    public Text Introduce;//介绍
    public Text ButtonText;//按钮文字

    public int shopParam;

    public override void OnYesClick()
    {
        ShopManager.Instance.ShowShop(shopParam);
        Close(); // 关闭对话框
    }
}
