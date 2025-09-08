using GameServer.Entities;
using SkillBridge.Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameServer.Managers
{
    public class StatusManager
    {
        Character Owner;

        private List<Nstatus> Status { get; set; }

        public bool HasStatus
        {
            get { return this.Status.Count > 0; }
        }

        public StatusManager(Character owner)//构造函数
        {
            this.Owner = owner;
            this.Status = new List<Nstatus>();
        }

        public void AddStatus(StatusType type,int id,int value,StatusAction action)
        {
            this.Status.Add(new Nstatus()
            {
                Type = type,
                Id = id,
                Value = value,
                Action = action
            });
        }


        public void AddGoldChange(int goldDelta)
        {
            if (goldDelta > 0)
            {
                this.AddStatus(StatusType.Money, 0, goldDelta, StatusAction.Add);
            }
            if (goldDelta < 0)
            {
                this.AddStatus(StatusType.Money, 0, -goldDelta, StatusAction.Delete);
            }
        }

        public void AddItemChange(int id,int count,StatusAction action)
        {
            this.AddStatus(StatusType.Item, id, count, action);
        }

        public void ApplyResponse(NetMessageResponse message)
        {
            if (message.statusNotify == null)
            {
                message.statusNotify = new StatusNotify();
            }
            foreach(var status in this.Status)
            {
                message.statusNotify.Status.Add(status);
            }
            this.Status.Clear();
        }
    }
}
