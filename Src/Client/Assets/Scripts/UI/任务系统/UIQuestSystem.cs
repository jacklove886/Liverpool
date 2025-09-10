using Common.Data;
using Managers;
using Models;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIQuestSystem : UIWindow {

    public Text title;

    public GameObject UIQuestItem;

    public TabView Tabs;
    public ListView listMain;//主线列表
    public ListView listBranch;//支线列表

    public UIQuestInfo questInfo;

    private bool showAvaiableList = false;//是否显示可接任务


    void Start ()
    {
        this.listMain.onItemSelected += this.OnQuestSelected;
        this.listBranch.onItemSelected += this.OnQuestSelected;
        this.Tabs.OnTabSelect += OnSelectTab;
        RefreshUI();
        //QuestManager.Instance.OnQuestChanged+=RefreshUI;
    }

    void OnSelectTab(int index)
    {
        showAvaiableList = index == 1;//如果是1就是可接任务 否则就是进行中
        RefreshUI();
    }

    private void OnDestroy()
    {
        //QuestManager.Instance.OnQuestChanged-=RefreshUI;
    }

    void RefreshUI()
    {
        ClearAllQuestList();
        InitAllQuestItems();
    }

    void InitAllQuestItems()
    {
        foreach(var kv in QuestManager.Instance.allQuestt)
        {
            if (showAvaiableList)
            {
                if (kv.value.Info != null)
                {
                    continue;
                }
            }
            else
            {
                if(kv.value.Info == null)
                {
                    continue;
                }
            }

            GameObject go=Instantiate(UIQuestItem,kv.Value.Define.Type==QuestType.Main)
        }
    }
    

    void ClearAllQuestList()
    {
        this.listMain.RemoveAll();
        this.listBranch.RemoveAll();
    }

    public void DoEquip(Item item)
    {
        EquipManager.Instance.OnEquipItem(item);
    }

    public void UnEquip(Item item)
    {
        EquipManager.Instance.UnEuqipItem(item);
    }
    public void OnQuestSelected(ListView.ListViewItem item)
    {
        UIQuestItem questItem = item as QIQuestItem;
        this.questInfo.SetQuestInfo(questInfo.quest);
    }


}
