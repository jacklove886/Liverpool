using Entities;
using SkillBridge.Message;
using System;
using System.Collections.Generic;


namespace Managers
{
    interface IEntityNotify
    {
        void OnEntityRemoved();
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
    }
}
