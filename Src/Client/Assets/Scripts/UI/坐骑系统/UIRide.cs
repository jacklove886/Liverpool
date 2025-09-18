
using Managers;
using Models;
using SkillBridge.Message;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIRide : UIWindow {

    public GameObject itemPrefab;
    public ListView listMain;
    public Text Description;
    public Image Puppy;
    public UIRideItem selectedItem;
    public Image RideImage;
    private void Start()
    {
        RefreshUI();
        this.listMain.onItemSelected += this.OnItemSelected;
    }

    private void OnItemSelected(ListView.ListViewItem item)
    {
        this.selectedItem = item as UIRideItem;
        this.Description.text = this.selectedItem.item.Define.Description;
    }

    private void RefreshUI()//刷新界面
    {
        ClearItems();
        InitItems();
    }

    //初始化道具列表
    void InitItems()
    {
        foreach (var kv in ItemManager.Instance.Items)//遍历拥有的所有道具
        {
            //类型如果是坐骑才显示在坐骑页面
            if (kv.Value.Define.Type == ItemType.Ride && (kv.Value.Define.LimitClass ==CharacterClass.None|| kv.Value.Define.LimitClass == User.Instance.CurrentCharacter.Class))
            {
                if (EquipManager.Instance.Contains(kv.Key)) continue;
                GameObject go = Instantiate(itemPrefab, listMain.transform);
                UIRideItem ui = go.GetComponent<UIRideItem>();
                ui.SetRideItem(kv.Value);
                this.listMain.AddItem(ui);              
            }
            
        }
        if (listMain != null && listMain.items.Count > 0)
        {
            listMain.SelectedItem = listMain.items[0];
            OnItemSelected(listMain.SelectedItem);
        }
    }

    //清空装备列表
    void ClearItems()
    {
        listMain.RemoveAll();
    }


    public void OnClickRide()
    {
        if (this.selectedItem == null)
        {
            MessageBox.Show("请选择要召唤的坐骑", "提示");
            return;
        }
        User.Instance.Ride(this.selectedItem.item.Id);
    }

}
