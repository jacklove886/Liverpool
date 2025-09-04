using Entities;
using SkillBridge.Message;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Managers
{
    interface IEntityNotify
    {
        void OnEntityRemoved();
        void OnEntityChange(Entity entity);
        void OnEntityEvent(EntityEvent @event,float horizontal = 0, float vertical = 0);
    }

    class EntityManager:Singleton<EntityManager>
    {
        Dictionary<int, Entity> entities = new Dictionary<int, Entity>();

        Dictionary<int, IEntityNotify> notifies = new Dictionary<int, IEntityNotify>();

        public void RegisterEntityChangeNotify(int entityId, IEntityNotify notify)
        {
            notifies[entityId] = notify;
        }

        public void AddEntity(Entity entity)
        {
            entities[entity.entityId] = entity;
        }

        public void RemoveEntity(NEntity entity)
        {
            entities.Remove(entity.Id);
            if (notifies.ContainsKey(entity.Id))
            {
                notifies[entity.Id].OnEntityRemoved();
                notifies.Remove(entity.Id);
            }
        }

        //收到其他人的移动 要将那个人的移动实现出来
        internal void OnEntitySync(NEntitySync data)
        {
            Entity entity = null;
            entities.TryGetValue(data.Id, out entity);
            if (entity != null)
            {
                if (data != null)
                {
                    //更新数据
                    entity.EntityData = data.Entity;
                }
                if (notifies.ContainsKey(data.Id))
                {
                    //通知EntityController实体数据发生变化
                    notifies[entity.entityId].OnEntityChange(entity);
                    // 通知EntityController处理实体事件
                    notifies[entity.entityId].OnEntityEvent(data.Event);
                }
            }
        }
    }
}
