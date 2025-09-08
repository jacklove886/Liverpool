using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIBagItemIcon : MonoBehaviour {

    public Image iconImage;//图标名字
    public Image secondImage;

    public Text countText;//数量

	public void SetMainIcon(string iconName,string text)
    {
        this.iconImage.overrideSprite = Resloader.Load<Sprite>(iconName);
        this.countText.text = text;
    }
}
