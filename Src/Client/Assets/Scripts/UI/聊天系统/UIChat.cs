using Candlelight.UI;
using Managers;
using SkillBridge.Message;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class UIChat : MonoBehaviour {

    public HyperText chatMessage;//聊天显示内容

    public TabView channelTabs;//频道切换

    public InputField messageInputField;//消息输入框

    public Text chatTargetPerson;//私聊对象
    public GameObject chatTargetPersonImage;//私聊对象

    public Dropdown channelDropdown;//下拉框

    void Start () {
        this.channelTabs.OnTabSelect += OnDisplayChannelSelected;
        ChatManager.Instance.Onchat += RefreshUI;//有聊天消息发出 更新UI
	}

    private void OnDestroy()
    {
        this.channelTabs.OnTabSelect -= OnDisplayChannelSelected;
        ChatManager.Instance.Onchat -= RefreshUI;
    }

    void Update()
    {
        InputManager.Instance.IsInputMode = messageInputField.isFocused;//如果正在输入模式
    }

    public void OnDisplayChannelSelected(int index)
    {
        //切换选择频道
        ChatManager.Instance.displayChannel = (ChatManager.LocalChannel)index;
        RefreshUI();
    }

    public void RefreshUI()
    {
        //获取消息
        this.chatMessage.text = ChatManager.Instance.GetCurrentMessages();
        //确保左下角的频道是正确的
        this.channelDropdown.value = (int)ChatManager.Instance.sendChannel - 1;
        if (ChatManager.Instance.sendChannel == ChatManager.LocalChannel.Private)//如果频道是私聊
        {
            this.chatTargetPersonImage.SetActive(true);//显示私聊对象
            if (ChatManager.Instance.PrivateID != 0)
            {
                this.chatTargetPerson.text = ChatManager.Instance.PrivateName + ":";
            }
            else
            {
                this.chatTargetPerson.text = "<无>";
            }
        }
        else
        {
            this.chatTargetPersonImage.SetActive(false);
        }
    }

    public void OnClickChatLink(HyperText text, HyperText.LinkInfo link)
    {
        if (string.IsNullOrEmpty(link.Name)) return;//链接为空
        if (link.Name.StartsWith("c:"))//c是Character的缩写
        {
            string[] strs = link.Name.Split(":".ToCharArray());//用:分割字符串
            UIPopCharMenu menu = UIManager.Instance.Show<UIPopCharMenu>();
            //例如<a name="c:1001:Name" class="player">Name</a> 角色 Character
            //<a name="i:1001:Name" class="player">Name</a> 道具 Item
            menu.targetName = strs[2]; 
            menu.targetId = int.Parse(strs[1]);
            Canvas canvas = menu.GetComponent<Canvas>();
            canvas.sortingOrder = 10;        
        }
    }

    public void OnClickSend()
    {
        OnEndInput();
    }

    public void OnEndInput()
    {
        if (!string.IsNullOrEmpty(messageInputField.text.Trim()))//Trim会过滤空白字符 也就是过滤输入为空或者全是空白字符
        {
            SendChat(messageInputField.text);
        }
        this.messageInputField.text = "";//清空输入框
    }

    private void SendChat(string text)
    {
        ChatManager.Instance.SendChat(text);
    }


    public void OnChanelChanged()
    {
        if (ChatManager.Instance.sendChannel == (ChatManager.LocalChannel)(channelDropdown.value + 1))
        {
            return;
        }
        if(!ChatManager.Instance.SetSendChannel((ChatManager.LocalChannel)channelDropdown.value + 1))
        {
            this.channelDropdown.value = (int)ChatManager.Instance.sendChannel - 1;
        }
        else
        {
            this.RefreshUI();
        }
    }
}
