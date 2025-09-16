using Candlelight.UI;
using Managers;
using Models;
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
        this.channelTabs.OnTabSelect += OnDisplayChannelSelected;//切换按钮就调用方法
        ChatManager.Instance.Onchat += RefreshUI;//有聊天消息发出 更新UI
	}

    private void OnDestroy()
    {
        this.channelTabs.OnTabSelect -= OnDisplayChannelSelected;
        ChatManager.Instance.Onchat -= RefreshUI;
    }

    void Update()
    {
        InputManager.Instance.IsInputMode = messageInputField.isFocused;//如果正在输入  IsInputMode为false
    }

    public void OnDisplayChannelSelected(int index)
    {
        //切换选择频道  
        if (User.Instance.TeamInfo == null && index == 3)
        {
            ChatManager.Instance.Messages[3].Add(new ChatMessage()
            {
                Channel=ChatChannel.Team,
                Message = "你没有加入任何队伍",
                FromName = "系统"
            });               
        }
        else if (User.Instance.CurrentCharacter.Guild == null && index == 4)
        {
            ChatManager.Instance.Messages[4].Add(new ChatMessage()
            {
                Channel = ChatChannel.Guild,
                Message = "你没有加入任何公会",
                FromName = "系统"
            });
        }
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
            if (ChatManager.Instance.toId != 0)
            {
                //设置文本为私聊对象名字
                this.chatTargetPerson.text = ChatManager.Instance.toName + ":";
            }
            else
            {
                this.chatTargetPerson.text = "<无>";
            }
        }
        else//非私聊频道不显示
        {
            this.chatTargetPersonImage.SetActive(false);
        }
    }

    public void OnChanelChanged()//下拉框发生改变
    {
        //举例:本来就是世界 又选了世界 不发生变化
        if (ChatManager.Instance.sendChannel == (ChatManager.LocalChannel)(channelDropdown.value + 1))
        {
            return;
        }
        //选了队伍或者公会  但本身又没有
        if (!ChatManager.Instance.SetSendChannel((ChatManager.LocalChannel)channelDropdown.value + 1))
        {
            //(int)sendChannel不是综合的时候  切换回原本的频道
            if ((int)ChatManager.Instance.sendChannel - 1 >= 0)
            {
                this.channelDropdown.value = (int)ChatManager.Instance.sendChannel-1;
            }
            //因为(int)sendChannel初始默认是0  再-1会出现枚举值异常的情况
            else
            {
                this.channelDropdown.value = 0;
            }
        }
        else//成功发生变化 刷新UI
        {
            this.RefreshUI();
        }
    }

    public void OnClickChatLink(HyperText text, HyperText.LinkInfo link)
    {
        if (string.IsNullOrEmpty(link.Name)) return;//链接为空
        if (link.Name.StartsWith("c:"))//以c开头的才能点击
        {
            string[] strs = link.Name.Split(":".ToCharArray());//用:分割字符串
            UIPopCharMenu menu = UIManager.Instance.Show<UIPopCharMenu>();
            //例如<a name="c:1001:Name" class="player">Name</a> 角色 Character
            //<a name="i:1001:Name" class="player">Name</a> 道具 Item
            menu.targetName = strs[2];//对应分割后索引是2的元素
            menu.targetId = int.Parse(strs[1]);//对应分割后索引是1的元素
            Canvas canvas = menu.GetComponent<Canvas>();
            canvas.sortingOrder = 10;//设置层级
        }
    }

    public void OnClickSend()//点击发送按钮
    {
        OnEndInput();
    }

    public void OnEndInput()//点击发送按钮或者回车都会调用这个方法
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


}
