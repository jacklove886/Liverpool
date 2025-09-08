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

        Dictionary<StatusType, StatusNotifyHandle> eventMap = new Dictionary<StatusType, StatusNotifyHandle>();

        public void Init()
        {

        }

        public void RegisterStatusNofity(StatusType function,StatusNotifyHandle action)
        {
            if (!eventMap.ContainsKey(function))
            {
                eventMap[function] = action;
            }
            else
            {
                eventMap[function] += action;
            }
        }

        public void UnregisterStatusNotify(StatusType function, StatusNotifyHandle action)
        {
            if (eventMap.ContainsKey(function))
            {
                eventMap[function] -= action;
            }
        }

        public StatusService()
        {
            MessageDistributer.Instance.Subscribe<StatusNotify>(this.OnStatusNotify);
        }

        public void Dispose()
        {
            MessageDistributer.Instance.Unsubscribe<StatusNotify>(this.OnStatusNotify);
        }

        private void OnStatusNotify(object sender, StatusNotify notify)
        {
            foreach(Nstatus status in notify.Status)
            {
                Notify(status);
            }
        }

        private void Notify(Nstatus status)
        {
            Debug.LogFormat("StatusNotify:[{0}[{1}][{2}][{3}]", status.Type, status.Action, status.Id, status.Value);

            if (status.Type == StatusType.Money)
            {
                if (status.Action == StatusAction.Add)
                {
                    User.Instance.AddGold(status.Value);
                }
                else if (status.Action == StatusAction.Delete)
                {
                    User.Instance.AddGold(-status.Value);
                }
            }

            //如果不是钱就发通知
            StatusNotifyHandle handler;
            if(eventMap.TryGetValue(status.Type,out handler))
            {
                handler(status);
            };
        }
    }
}
