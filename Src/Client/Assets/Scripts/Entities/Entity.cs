using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using SkillBridge.Message;

namespace Entities
{
    public class Entity
    {
        [Header("实体信息")]
       public int entityId;        //实体ID 
       public Vector3Int position;    //位置 
       public Vector3Int direction;   //方向
       public int speed;           //速度

       [Header("网络数据")]
       private  NEntity entityData;//网络实体数据  
       
       public NEntity EntityData
       {
           get
            {
                UpadateEntityData();
                return entityData;
            }
           set { entityData = value; this.SetEntityData(value);}
       }

       public Entity(NEntity entity)
       {
        this.entityId=entity.Id;
        this.entityData=entity;
        this.SetEntityData(entity);
       }

       
       public virtual void OnUpdate(float delta)
       {
         if(this.speed!=0)
         {
            Vector3 dir=this.direction;
            this.position +=Vector3Int.RoundToInt(dir*speed*delta/100f);
         }
       }

        public void SetEntityData(NEntity entity)
        {
            this.position = this.position.FromNVector3(entity.Position);
            this.direction = this.direction.FromNVector3(entity.Direction);
            this.speed = entity.Speed;
        }

        public void UpadateEntityData()
        {
            entityData.Position.FromVector3Int(this.position);
            entityData.Direction.FromVector3Int(this.direction);
            entityData.Speed = this.speed;
        }
    }
}