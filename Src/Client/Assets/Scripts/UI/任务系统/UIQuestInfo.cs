using System;
using System.Collections;
using System.Collections.Generic;
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
        }

        this.rewardMoney.text = quest.Define.RewardGold.ToString();
        this.rewardExp.text = quest.Define.RewardExp.ToString();

        //自动设置为LayoutVertical
        foreach (var fitter in this.GetComponentsInChildren<ContentSizeFitter>())
        {
            fitter.SetLayoutVertical();
        }  
    }

    public void OnClickAbandon()
    {

    }
}
