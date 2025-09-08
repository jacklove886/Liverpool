using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

public class TabButton : MonoBehaviour //单个标签按钮  是TabView的子物体
{
    public Sprite activeImage;//激活图片
    public Sprite normalImage;//正常图片

    public TabView tabView;

    public int tabIndex = 0;

    private Image tabImage;

    private void Start()
    {
        tabImage = this.GetComponent<Image>();
        normalImage = tabImage.sprite;
        this.GetComponent<Button>().onClick.AddListener(OnClick);
    }

    public void Select(bool select)
    {
        tabImage.overrideSprite = select ? activeImage : normalImage;
    }

    void OnClick()
    {
        this.tabView.SelectTab(this.tabIndex);//传入索引 选择背包
    }
}
