using Managers;
using Models;
using Services;
using SkillBridge.Message;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIBag : UIWindow
{
    public Text money;

    public Transform[] pages;//背包页面(绑定Content）

    public GameObject bagItem;//预制体 方便展示血瓶还是蓝瓶...

    List<Image> slots;//所有的背包栏 目前20×3个
    
	void Start ()
    {
        if (slots == null)
        {
            slots = new List<Image>();
            for(int i = 0; i < this.pages.Length; i++)
            {
                slots.AddRange(this.pages[i].GetComponentsInChildren<Image>(true));
            }
        }
        StartCoroutine(InitBags());
	}

    IEnumerator InitBags()//初始化背包显示
    {
        for(int i = 0; i < BagManager.Instance.Items.Length; i++)
        {
            var item = BagManager.Instance.Items[i];
            if (item.ItemID > 0)
            {
                GameObject go = Instantiate(bagItem, slots[i].transform);//展示正确的道具图片 生成在对应的栏位下
                go.name = "第"+(i+1)+"个道具";
                var ui = go.GetComponent<UIBagItemIcon>();
                var def = ItemManager.Instance.Items[item.ItemID].Define;
                ui.SetMainIcon(def.Icon, item.Count.ToString());
            }
            SetMoney();
        }
        yield return null;
    }

    public void SetMoney()
    {
        this.money.text = User.Instance.CurrentCharacter.Gold.ToString();
    }

    public void OnReset()
    {
        BagManager.Instance.Reset();//整理背包功能
    }

    public void OnClickClose()
    {
        UIManager.Instance.Close(typeof(UIBag));
    }
}
