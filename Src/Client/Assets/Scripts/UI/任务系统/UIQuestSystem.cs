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
        QuestManager.Instance.OnQuestStatusChanged += RefreshUI;//任务状态改变要刷新UI
    }

    void OnSelectTab(int index)
    {
        showAvaiableList = index == 1;//如果是1就是可接任务 否则就是进行中
        RefreshUI();
    }

    private void OnDestroy()
    {
        QuestManager.Instance.OnQuestStatusChanged -= RefreshUI;
    }

    void RefreshUI()
    {
        ClearAllQuestList();//清空再初始化
        InitAllQuestItems();
    }

    void RefreshUI(Quest quest)
    {
        ClearAllQuestList();
        InitAllQuestItems();
    }


    void InitAllQuestItems()
    {
        foreach(var kv in QuestManager.Instance.allQuests)//遍历所有可用任务
        {
            if (showAvaiableList)//可接任务列表
            {
                if (kv.Value.Info != null)
                {
                    continue;
                }
            }
            else//进行中的任务列表
            {
                if(kv.Value.Info == null)//没接的任务
                {
                    continue;
                }
                if (kv.Value.Info.Status == SkillBridge.Message.QuestStatus.Complated|| kv.Value.Info.Status == SkillBridge.Message.QuestStatus.Finished)
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
        SetDefaultSelection();
    }

    void SetDefaultSelection()
    {
        if (this.listMain.items.Count > 0)//如果有主线任务
        {
            this.listMain.SelectedItem = this.listMain.items[0];//默认选第一个任务
            OnQuestSelected(this.listMain.items[0]);//手动调用设置属性
            return;
        }
        if (this.listBranch.items.Count > 0)//如果有支线任务
        {
            this.listBranch.SelectedItem = this.listBranch.items[0];
            OnQuestSelected(this.listBranch.items[0]);
            return;
        }
        else//列表为空
        {
            this.questInfo.ShowEmptyQuestState();
        }
    }


    void ClearAllQuestList()
    {
        this.listMain.RemoveAll();
        this.listBranch.RemoveAll();
    }

    public void OnQuestSelected(ListView.ListViewItem item)
    {
        if (item.owner == this.listMain)
        {
            if (this.listBranch.selectedItem != null)
            {
                this.listBranch.selectedItem.Selected = false;//用属性
                this.listBranch.selectedItem = null;  // 清除引用  不清除的话listBranch.selectedItem仍然是上次点击的那个物体
            }
        }
        if (item.owner == this.listBranch)
        {
            if (this.listMain.selectedItem != null)
            {
                this.listMain.selectedItem.Selected = false;
                this.listMain.selectedItem = null; 
            }       
        }
        UIQuestItem questItem = item as UIQuestItem;
        this.questInfo.SetQuestInfo(questItem.quest);
    }
}
