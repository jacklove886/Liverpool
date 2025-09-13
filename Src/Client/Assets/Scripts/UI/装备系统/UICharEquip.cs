using Managers;
using Models;
using SkillBridge.Message;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UICharEquip : UIWindow
{
    public Text title;
    public Text money;
    public Text characterName;

    public GameObject itemPrefab;

    public GameObject itemEquipedIcon;

    public Transform itemListRoot;

    public List<Transform> slots;//装备栏

    public GameObject[] EquipText;//装备栏根节点

    private void Start()
    {
        RefreshUI();
        EquipManager.Instance.OnEquipChange += RefreshUI;
    }

    private void OnDestroy()
    {
        EquipManager.Instance.OnEquipChange -= RefreshUI;
    }

    private void RefreshUI()//刷新界面
    {
        ClearAllEquipList();//把左边装备列表清空
        InitAllEquipItems();//初始化左边装备列表
        ClearEuipedList();//把中间已经装备的列表清空
        InitEquipedItems();//初始化中间装备列表
        var cha = User.Instance.CurrentCharacter;
        //设置信息
        this.money.text = cha.Gold.ToString();
        characterName.text = cha.Name + "  LV " + cha.Level;
    }

    //初始化道具列表
    void InitAllEquipItems()
    {
        foreach(var kv in ItemManager.Instance.Items)//遍历拥有的所有道具
        {
            //类型如果是装备才显示在装备页面
            if (kv.Value.Define.Type == ItemType.Equip&&kv.Value.Define.LimitClass==User.Instance.CurrentCharacter.Class)
            {
                if (EquipManager.Instance.Contains(kv.Key))
                {
                    continue;//已经装备就不显示在道具列表了
                }
                GameObject go = Instantiate(itemPrefab, itemListRoot);//创建左边道具列表
                UIEquipItem ui = go.GetComponent<UIEquipItem>();
                //false表示不是装备列表 是左边道具列表   true就表示是装备列表
                ui.SetEquipItem(kv.Key, kv.Value, this, false);
            }
        }
    }

    //清空装备列表
    void ClearAllEquipList()
    {
        foreach(var item in itemListRoot.GetComponentsInChildren<UIEquipItem>())
        {
            Destroy(item.gameObject);
        }
    }

    //清空道具列表
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

    //初始化装备列表
    void InitEquipedItems()
    {
        for(int i = 0; i < (int)EquipSlot.SlotMax; i++)
        {
            var item = EquipManager.Instance.Equips[i];        
            if (item != null)//如果格子上有装备
            {
                GameObject go = Instantiate(itemEquipedIcon, slots[i]);
                EquipText[i].SetActive(false);//文字隐藏
                UIEquipItem ui = go.GetComponent<UIEquipItem>();
                ui.SetEquipItem(i, item, this, true);
            }
            else
            {
                EquipText[i].SetActive(true);
            }
        }
    }

    private UIEquipItem selectedItem;//当前选中的道具
    public void SelectEquipItem(UIEquipItem item)
    {
        if (selectedItem != null)//如果有选中的道具
        {
            selectedItem.Selected = false;//取消选中状态
        }
        selectedItem = item;//更新当前选中的道具
        if (selectedItem != null)//如果新选中的道具不为空
        {
            selectedItem.Selected = true;//设置新道具为选中状态
        }
    }

    //穿装备  由UIEquipItem调用
    public void DoEquip(Item item)
    {
        EquipManager.Instance.EuqipItem(item);
    }

    //脱装备
    public void DoUnEquip(Item item)
    {
        EquipManager.Instance.UnEuqipItem(item);
    }
}
