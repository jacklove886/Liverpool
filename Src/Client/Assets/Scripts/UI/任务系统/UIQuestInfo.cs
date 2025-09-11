using System;
using System.Collections;
using System.Collections.Generic;
using Managers;
using Models;
using UnityEngine;
using UnityEngine.UI;

public class UIQuestInfo : MonoBehaviour//任务信息列表
{

    public Text title;
    public Text[] targets;//任务目标
    public Text description;//描述
    public UIBagItemIcon[] rewardItems;//任务奖励

    public Text rewardMoney;//奖励金额
    public Text rewardExp;//奖励经验
    public GameObject image;
    public GameObject guimie;
    public GameObject button1;
    public GameObject button2;

    void Start () {
		
	}
	

    public void SetQuestInfo(Quest quest)//设置信息
    {
        this.title.text = string.Format("[{0}]{1}", quest.Define.Type, quest.Define.Name);
        if (quest.Info == null)
        {
            this.description.text = quest.Define.Dialog;
        }
        else
        {
            if (quest.Info.Status == SkillBridge.Message.QuestStatus.Complated)
            {
                this.description.text = quest.Define.DialogFinish;
            }
            if (quest.Info.Status == SkillBridge.Message.QuestStatus.InProgress)
            {
                this.description.text = quest.Define.DialogIncomplete;
            }
        }
        SetRewardItem(quest);
        this.rewardMoney.text = "金币 : "+quest.Define.RewardGold.ToString();
        this.rewardExp.text = "经验值 : " + quest.Define.RewardExp.ToString();
        this.rewardMoney.gameObject.SetActive(true);
        this.rewardExp.gameObject.SetActive(true);
        image.gameObject.SetActive(true);
        button1.gameObject.SetActive(true);
        button2.gameObject.SetActive(true);
        guimie.gameObject.SetActive(false);

        //自动设置为LayoutVertical
        foreach (var fitter in this.GetComponentsInChildren<ContentSizeFitter>())
        {
            fitter.SetLayoutVertical();
        }  
    }

    void SetRewardItem(Quest quest)
    {
        for (int i = 0; i < rewardItems.Length; i++)
        {
            rewardItems[i].gameObject.SetActive(false);
        }
        int rewardIndex = 0;
        int[] rewardIds = { quest.Define.RewardItem1, quest.Define.RewardItem2, quest.Define.RewardItem3 };
        int[] rewardCounts = { quest.Define.RewardItem1Count, quest.Define.RewardItem2Count, quest.Define.RewardItem3Count };

        //小于奖励数量长度  小于图标数量长度
        for (int i = 0; i < rewardIds.Length && rewardIndex < rewardItems.Length; i++)
        {
            if (rewardIds[i] > 0)
            {
                var itemDefine = DataManager.Instance.Items[rewardIds[i]];
                rewardItems[rewardIndex].SetMainIcon(itemDefine.Icon, rewardCounts[i].ToString());
                rewardItems[rewardIndex].gameObject.SetActive(true);
                rewardIndex++;
            }
            else
            {

            }
        }
    }
}
