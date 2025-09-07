using Managers;
using Models;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIBag : UIWindow
{
    public Text money;

    public Transform[] pages;

    public GameObject bagItem;

    List<Image> slots;//背包槽
    
	void Start ()
    {
        if (slots == null)
        {
            slots = new List<Image>();
            for(int page = 0; page < this.pages.Length; page++)
            {
                slots.AddRange(this.pages[page].GetComponentsInChildren<Image>(true));
            }
        }
        StartCoroutine(InitBags());
	}
	
	IEnumerator InitBags()
    {
        for(int i = 0; i < BagManager.Instance.Items.Length; i++)
        {
            var item = BagManager.Instance.Items[i];
            if (item.ItemID > 0)
            {
                GameObject go = Instantiate(bagItem, slots[i].transform);
                go.name = "第"+i+"个道具";
                var ui = go.GetComponent<UIItemIcon>();
                var def = ItemManager.Instance.Items[item.ItemID].Define;
                ui.SetMainIcon(def.Icon, item.Count.ToString());
            }
        }
        yield return null;
    }

    public void SetTitle(string title)
    {
        this.money.text = User.Instance.CurrentCharacter.Id.ToString();
    }

    public void OnReset()
    {
        BagManager.Instance.Reset();
    }
}
