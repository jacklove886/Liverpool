using Managers;
using Models;
using SkillBridge.Message;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UICharEquip : MonoBehaviour
{
    public Text title;
    public Text money;

    public GameObject itemPrefab;

    public GameObject itemEquipedIcon;

    public Transform itemListRoot;

    public List<Transform> slots;

    private void Start()
    {
        RefreshUI();
        EquipManager.Instance.OnEquipChange += RefreshUI;
    }

    private void OnDestroy()
    {
        EquipManager.Instance.OnEquipChange -= RefreshUI;
    }

    private void RefreshUI()
    {
        ClearAllEquipList();//把左边装备列表清空
        InitAllEquipItems();//初始化左边装备列表
        ClearEuipedList();//把中间已经装备的列表清空
        InitEquipedItems();//初始化中间装备列表
        this.money.text = User.Instance.CurrentCharacter.Gold.ToString();
    }

    void InitAllEquipItems()
    {
        foreach(var kv in ItemManager.Instance.Items)
        {
            //类型如果是装备才显示
            if (kv.Value.Define.Type == ItemType.Equip&&kv.Value.Define.LimitClass==User.Instance.CurrentCharacter.Class)
            {
                if (EquipManager.Instance.Contains(kv.Key))
                {
                    continue;//已经装备就不显示在道具列表了
                }
                GameObject go = Instantiate(itemPrefab, itemListRoot);
                Text text = go.transform.GetChild(0).GetComponent<Text>();
                text.text = "";
                UIEquipItem ui = go.GetComponent<UIEquipItem>();
                //false表示不是装备列表 是左边道具列表   true就表示是装备列表
                ui.SetEquipItem(kv.Key, kv.Value, this, false);
            }
        }
    }

    void ClearAllEquipList()
    {
        foreach(var item in itemListRoot.GetComponentsInChildren<UIEquipItem>())
        {
            Destroy(item.gameObject);
        }
    }

    void ClearEuipedList()
    {
        foreach(var item in slots)
        {
            if (item.childCount > 0)
            {
                Destroy(item.GetChild(0).gameObject);
            }
        }
    }

    void InitEquipedItems()
    {
        for(int i = 0; i < (int)EquipSlot.SlotMax; i++)
        {
            var item = EquipManager.Instance.Equips[i];
            if (item != null)//如果格子上有装备
            {
                GameObject go = Instantiate(itemEquipedIcon, slots[i]);
                Text text = go.transform.GetChild(0).GetComponent<Text>();
                text = null;
                UIEquipItem ui = go.GetComponent<UIEquipItem>();
                ui.SetEquipItem(i, item, this, true);
            }
        }
    }

    //穿装备  由格子调用
    public void DoEquip(Item item)
    {
        EquipManager.Instance.EuqipItem(item);
    }

    //脱装备
    public void DoUnEquip(Item item)
    {
        EquipManager.Instance.UnEuqipItem(item);
    }

    public void OnClickClose()
    {
        UIManager.Instance.Close(typeof(UICharEquip));
    }

}
