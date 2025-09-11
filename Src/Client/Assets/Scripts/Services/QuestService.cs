using Managers;
using Models;
using Network;
using SkillBridge.Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Services
{
    class QuestService:Singleton<QuestService>
    {
        public QuestService()//构造函数
        {
            //发送请求 订阅响应消息并调用方法   
            MessageDistributer.Instance.Subscribe<QuestAcceptResponse>(this.OnQuestAccept);
            MessageDistributer.Instance.Subscribe<QuestSubmitResponse>(this.OnQuestSubmit);
        }

        public void Dispose()
        {
            //取消订阅
            MessageDistributer.Instance.Unsubscribe<QuestAcceptResponse>(this.OnQuestAccept);
            MessageDistributer.Instance.Unsubscribe<QuestSubmitResponse>(this.OnQuestSubmit);
        }

        public bool SendQuestAccept(Quest quest)
        {
            NetMessage message = new NetMessage();
            message.Request = new NetMessageRequest();
            message.Request.questAccept = new QuestAcceptRequest();
            message.Request.questAccept.QuestId = quest.Define.ID;
            NetClient.Instance.SendMessage(message);
            return true;
        }

        private void OnQuestAccept(object sender, QuestAcceptResponse response)
        {
            Debug.LogFormat("收到任务接受响应:{0}", response.Result);
            if (response.Result == Result.Success)
            {
                QuestManager.Instance.OnQuestAccepted(response.Quest);
            }
            else
            {
                MessageBox.Show("任务接受失败！" + response.Errormsg, "任务失败", MessageBoxType.Error);
            }
        }

        public bool SendQuestSubmit(Quest quest)
        {
            NetMessage message = new NetMessage();
            message.Request = new NetMessageRequest();
            message.Request.questSubmit = new QuestSubmitRequest();
            message.Request.questSubmit.QuestId = quest.Define.ID;
            NetClient.Instance.SendMessage(message);
            return true;
        }

        private void OnQuestSubmit(object sender, QuestSubmitResponse response)
        {
            Debug.LogFormat("收到任务提交响应:{0}", response.Result);
            if (response.Result == Result.Success)
            {
                QuestManager.Instance.OnQuestSubmited(response.Quest);
            }
            else
            {
                MessageBox.Show("任务提交失败！" + response.Errormsg, "任务失败",MessageBoxType.Error);
            }
        }  
    }
}
