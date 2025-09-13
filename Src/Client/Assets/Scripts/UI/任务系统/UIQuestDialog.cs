using Models;
using SkillBridge.Message;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIQuestDialog : UIWindow
{

    public UIQuestInfo questInfo;

    public Quest quest;
    public GameObject openButtons;
    public GameObject submitButtons;

    private void Start()
    {
        
    }

    public void SetQuest(Quest quest)//设置任务
    {
        this.quest = quest;
        this.UpdateQuest();
        if (this.quest.Info == null)//没有网络信息 说明没有接受任务
        {
            openButtons.SetActive(true);
            submitButtons.SetActive(false);
        }
        else
        {
            if (this.quest.Info.Status == QuestStatus.Complated)//如果任务已完成  可以提交
            {
                submitButtons.SetActive(true);
                openButtons.SetActive(false);
            }
            else
            {
                openButtons.SetActive(false);
                submitButtons.SetActive(false);
            }
        }
     }

    void UpdateQuest()
    {
        if (this.quest != null)
        {
            if (this.questInfo != null)
            {
                this.questInfo.SetQuestInfo(quest);//设置信息
            }
        }
    }

}
