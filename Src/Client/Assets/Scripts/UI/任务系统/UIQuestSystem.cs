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


    void Start()
    {
        this.listMain.onItemSelected += this.OnQuestSelected;//订阅主线任务列表选中的事件
        this.listBranch.onItemSelected += this.OnQuestSelected;//订阅支线任务列表选中的事件
        this.Tabs.OnTabSelect += OnSelectTab;//进行中任务和可接任务之间的切换
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
        foreach(var kv in QuestManager.Instance.allQuests)
        {
            if (showAvaiableList)
            {
                if (kv.Value.Info != null)
                {
                    continue;
                }
            }
            else
            {
                if(kv.Value.Info == null)
                {
                    continue;
                }
            }

            //如果是主线 放到主线任务列里
            GameObject go = Instantiate(UIQuestItem, kv.Value.Define.Type == QuestType.Main ? this.listMain.transform : this.listBranch.transform);
            UIQuestItem ui = go.GetComponent<UIQuestItem>();
            ui.SetQuestInfo(kv.Value);
            if (kv.Value.Define.Type == QuestType.Main)
            {
                this.listMain.AddItem(ui);
            }
            else
            {
                this.listBranch.AddItem(ui);
            }
        }
    }


    void ClearAllQuestList()
    {
        this.listMain.RemoveAll();
        this.listBranch.RemoveAll();
    }

    public void OnQuestSelected(ListView.ListViewItem item)
    {
        UIQuestItem questItem = item as UIQuestItem;
        this.questInfo.SetQuestInfo(questItem.quest);
    }

    public void OnClickClose()
    {
        UIManager.Instance.Close(typeof(UIQuestSystem));
    }

}
