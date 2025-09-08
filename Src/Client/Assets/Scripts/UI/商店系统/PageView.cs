using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PageView : MonoBehaviour {

    public Button leftArrow;//左箭头
    public Button rightArrow;//右箭头
    public Text pageText;//页码
    public Scrollbar scrollbar;//滑动条

    public GameObject[] pagePages;//页码

    int index = -1;

    IEnumerator Start()
    {
        leftArrow.onClick.AddListener(()=> SelectPage(index-1));
        rightArrow.onClick.AddListener(() => SelectPage(index + 1));
        yield return new WaitForEndOfFrame();
        SelectPage(0);
    }

    public void SelectPage(int pageIndex)
    {
        if (this.index != pageIndex)//如果索引和当前的不一样 切换背包
        {
            for (int i = 0; i < pagePages.Length; i++)
            {
                pagePages[i].SetActive(i == pageIndex);
                
            }
            this.index = pageIndex;//更新索引
            UpdatePageText();
        }
    }

    private void UpdatePageText()
    {
        pageText.text = ((index + 1) +"/"+ (pagePages.Length)).ToString();//更新文本

        leftArrow.interactable = index > 0;
        rightArrow.interactable=index< pagePages.Length - 1;
    }


}
