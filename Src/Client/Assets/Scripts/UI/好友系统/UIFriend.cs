using Managers;
using Models;
using Services;
using Network;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIFriend : UIWindow {

    public GameObject itemPrefab;
    public ListView listMain;
    public Transform itemRoot;//Content
    public UIFriendItem selectedItem;
    public InputField inputField;

    void Start ()
    {
        if (listMain == null || this == null) 
            return;
        FriendService.Instance.OnFriendUpdate = RefreshUI;//打开就接收通知 会自动刷新
        listMain.onItemSelected += OnFriendSelected;
        RefreshUI();
        inputField.onEndEdit.AddListener(OnInputFinished);//监听搜索内容

    }

    private void OnInputFinished(string nameOrId)
    {
        inputField.text = nameOrId;
    }



    private void OnDestroy()
    {
        
    }

    public void OnFriendSelected(ListView.ListViewItem item)
    {
        //将item安全转换为UIFriendItem
        this.selectedItem=item as UIFriendItem;
    }

    public void OnClickFriendAdd()
    {
        InputBox.Show("输入要添加的好友名称或ID", "添加好友").OnSubmit += OnFriendAddSubmit;
    }

    private bool OnFriendAddSubmit(string input, out string tips)
    {
        tips = "";
        int friendId = 0;
        string friendName = "";
        if(!int.TryParse(input,out friendId))//尝试转换input为friendId 转换成功说明输入的是Id
        {
            friendName = input;
        }
        if (friendId == User.Instance.CurrentCharacter.Id || friendName == User.Instance.CurrentCharacter.Name)
        {
            tips = "不能添加自己哦！";
            return false;
        }
        FriendService.Instance.SendFriendAddRequest(friendId, friendName);
        return true;
    }

    public void OnClickFriendChat()
    {
        MessageBox.Show("暂未开放", "敬请期待");
    }

    public void OnClickFriendRemove()
    {
        if (selectedItem == null)
        {
            MessageBox.Show("请选择要删除的好友","您好");
            return;
        }
        MessageBox.Show(string.Format("确定要删除好友[{0}]吗", selectedItem.Info.friendInfo.Name), "删除好友", MessageBoxType.Confirm,"删除","取消").OnYes=()=>
            FriendService.Instance.SendFriendRemoveRequest(selectedItem.Info.Id,selectedItem.Info.friendInfo.Id);//选中好友的ID和那条记录的ID  
    }

    public void OnClickFriendTeamInvite()//组队功能
    {
        if (selectedItem == null)
        {
            MessageBox.Show("请选择要邀请的好友");
            return;
        }
        if (selectedItem.Info.Status == 0)
        {
            MessageBox.Show("请选择在线的好友");
            return;
        }
        MessageBox.Show(string.Format("确定要邀请好友[{0}]加入队伍吗", selectedItem.Info.friendInfo.Name), "组队邀请", MessageBoxType.Confirm).OnYes = () =>
        {
            TeamService.Instance.SendTeamInviteRequest(this.selectedItem.Info.friendInfo.Id, this.selectedItem.Info.friendInfo.Name);
        };
    }

    void RefreshUI()
    {
        ClearFriendList();
        InitFriendItems();
    }

    void InitFriendItems()
    {
        if (listMain == null || itemPrefab == null) 
            return;
        foreach (var item in FriendManager.Instance.allfriends)
        {
            GameObject go = Instantiate(itemPrefab, this.listMain.transform);
            if (item.Status == 1)
            {              
                go.transform.SetAsFirstSibling();               
            }
            UIFriendItem ui = go.GetComponent<UIFriendItem>();
            ui.SetFriendInfo(item);
            listMain.AddItem(ui);
        }
        if (listMain != null && listMain.items.Count > 0)
        {
            listMain.SelectedItem = listMain.items[0];//默认选中第一个
        }
    }

    void ClearFriendList()
    {
        listMain.RemoveAll();
    }

    public void OnClickClose()
    {
        UIManager.Instance.Close(typeof(UIFriend));
    }
}
