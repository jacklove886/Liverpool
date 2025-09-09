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
    class StatusService : Singleton<StatusService>, IDisposable
    {
        public delegate bool StatusNotifyHandle(Nstatus status);//委托
        //哈希只需要一个Key 不需要Value
        HashSet<StatusNotifyHandle> handlers = new HashSet<StatusNotifyHandle>();
        //StatusType 支持四种状态类型  Money Exp SkillPoint Item 四种变更
        Dictionary<StatusType, StatusNotifyHandle> eventMap = new Dictionary<StatusType, StatusNotifyHandle>();

        public void Init()
        {

        }

        //订阅注册机制 和NPC很像
        public void RegisterStatusNofity(StatusType function,StatusNotifyHandle action)
        {
            if (handlers.Contains(action))
            {
                return;
            }
            if (!eventMap.ContainsKey(function))
            {
                eventMap[function] = action;
            }
            else
            {
                eventMap[function] += action;
            }
            handlers.Add(action);
        }

        //取消订阅
        public void UnregisterStatusNotify(StatusType function, StatusNotifyHandle action)
        {
            if (eventMap.ContainsKey(function))
            {
                eventMap[function] -= action;
            }
        }


        public StatusService()//构造函数  订阅服务器的StatusNotify消息
        {
            MessageDistributer.Instance.Subscribe<StatusNotify>(this.OnStatusNotify);
        }

        public void Dispose()
        {
            MessageDistributer.Instance.Unsubscribe<StatusNotify>(this.OnStatusNotify);
        }

        //接收到消息后自动调用方法   遍历所有变更的情况 确保所有状态正确处理
        private void OnStatusNotify(object sender, StatusNotify notify)
        {
            foreach(Nstatus status in notify.Status)
            {
                Notify(status);
            }
        }

        //处理变更情况
        private void Notify(Nstatus status)
        {
            Debug.LogFormat("StatusNotify:[{0}[{1}][{2}][{3}]", status.Type, status.Action, status.Id, status.Value);

            //如果是金币
            if (status.Type == StatusType.Money)
            {
                //如果是增加
                if (status.Action == StatusAction.Add)
                {
                    User.Instance.AddGold(status.Value);
                }
                //如果是减少
                else if (status.Action == StatusAction.Delete)
                {
                    User.Instance.AddGold(-status.Value);
                }
            }

            //如果不是钱就发通知  
            StatusNotifyHandle handler;//handler是委托变量
            if(eventMap.TryGetValue(status.Type,out handler))//如果有调用的方法
            {
                handler(status);//调用这个方法
            };
        }
    }
}
