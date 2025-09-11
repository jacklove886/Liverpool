using Common;
using GameServer.Entities;
using Network;
using SkillBridge.Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameServer.Services
{
    class QuestService
    {

        public QuestService()//构造函数
        {
            //订阅请求消息并调用方法    发送响应
            MessageDistributer.Instance.Subscribe<QuestAcceptRequest>(this.OnQuestAccept);
            MessageDistributer.Instance.Subscribe<QuestSubmitRequest>(this.OnQuestSubmit);
        }

        public void Init()
        {

        }

        private void OnQuestAccept(NetConnection<NetSession> sender, QuestAcceptRequest request)
        {
            Character character = sender.Session.Character;
            Log.InfoFormat("接受任务请求:角色:{0}:任务:{1}", character.Id, request.QuestId);
            
            sender.Session.Response.questAccept = new QuestAcceptResponse();
            Result result = character.QuestManager.AcceptQuest(sender, request.QuestId);
            sender.Session.Response.questAccept.Result = result;
            sender.SendResponse();
        }

        private void OnQuestSubmit(NetConnection<NetSession> sender, QuestSubmitRequest request)
        {
            Character character = sender.Session.Character;
            Log.InfoFormat("提交任务请求:角色:{0}:任务:{1}", character.Id, request.QuestId);

            sender.Session.Response.questSubmit = new QuestSubmitResponse();
            Result result = character.QuestManager.SubmitQuest(sender, request.QuestId);
            sender.Session.Response.questSubmit.Result = result;
            sender.SendResponse();
        }
        
    }
}
