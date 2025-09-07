using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIItemIcon : MonoBehaviour {

    public Image mainImage;//图标名字
    public Image secondImage;

    public Text countText;//数量

	public void SetMainIcon(string iconName,string text)
    {
        this.mainImage.overrideSprite = Resloader.Load<Sprite>(iconName);
        this.countText.text = text;
    }
}
