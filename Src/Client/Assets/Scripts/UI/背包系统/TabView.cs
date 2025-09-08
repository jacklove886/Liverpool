using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

public class TabView: MonoBehaviour//标签管理器
{
    public TabButton[] tabButtons;//按钮
    public GameObject[] tabPages;//页码

    public int index = -1;

    IEnumerator Start()
    {
        for(int i = 0; i < tabButtons.Length; i++)
        {
            tabButtons[i].tabView = this;
            tabButtons[i].tabIndex = i;//告诉按钮所在的索引
        }
        yield return new WaitForEndOfFrame();//等待UI准备好  也是等待一帧
        SelectTab(0);
    }

    public void SelectTab(int tabindex)
    {
        if (this.index != tabindex)//如果索引和当前的不一样 切换背包
        {
            for(int i = 0; i < tabButtons.Length; i++)
            {
                tabButtons[i].Select(i == tabindex);
                tabPages[i].SetActive(i == tabindex);
            }
            this.index = tabindex;//更新索引
        }
    }
    
}
