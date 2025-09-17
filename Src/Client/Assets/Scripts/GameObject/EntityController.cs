using SkillBridge.Message;
using System.Collections;
using UnityEngine;
using Entities;
using Models;
using Managers;
using System;

public class EntityController : MonoBehaviour, IEntityNotify
{

    [Header("组件引用")]
    public Rigidbody rb;  
    public Animator anim;  
    public float[] jumpTime = { 1.8f, 1.3f, 1.3f };  // 跳跃持续的时间 战士、法师、游侠

    [Header("实体数据")]
    public Entity entity;  // 实体逻辑对象
    public int currentCharacterClass;  // 角色的职业类型索引

    [Header("位置和方向")]
    public Vector3 position;  
    public Vector3 direction;  

    [Header("角色类型")]
    public bool isPlayer=false;  // 是否为玩家角色

    public RideController rideController;//坐骑控制器
    public int currentRide = 0;
    public Transform rideBone;//坐骑骨骼

    void Start()
    {
        StartCoroutine(waitTime());
        if (User.Instance.CurrentCharacter != null)
        {
            currentCharacterClass = (int)User.Instance.CurrentCharacter.Class - 1;
        }

        if (entity != null)  
        {
            EntityManager.Instance.RegisterEntityChangeNotify(entity.entityId, this);
            UpdateTransform();  
        }

        if (isPlayer)  
        {
            rb.useGravity = true;  // 只有玩家受重力影响
        }
        else
        {
            rb.useGravity = false;
        }
    }

    IEnumerator waitTime()
    {
        yield return null;
    }

    void UpdateTransform()
    {
        position = GameObjectTool.LogicToWorld(entity.position);  
        direction = GameObjectTool.LogicToWorld(entity.direction);  

        rb.MovePosition(position);  
        transform.forward = direction;
    }

    void FixedUpdate()
    {
        if (entity == null) return;  

        entity.OnUpdate(Time.fixedDeltaTime);

        if (!isPlayer)
        {
            UpdateTransform();
        }
    }


    public void OnEntityRemoved()
    {
        if (UIWorldElementManager.Instance != null)
        {
            UIWorldElementManager.Instance.RemoveCharacterNameBar(this.transform);
        }
        Destroy(this.gameObject);
    }

    public void OnEntityEvent(EntityEvent entityEvent,int param)
    {

        switch (entityEvent)
        {
            case EntityEvent.EventIdle:
                SetIdleAnimation();  
                break;

            case EntityEvent.EventMove:
                SetMovementAnimation();  
                break;

            case EntityEvent.EventRun:
                SetRunAnimation(); 
                break;

            case EntityEvent.EventJump:
                Jump(); 
                break;
            case EntityEvent.EventRide:
                Ride(param);
                break;
        }
        //坐骑也有单独的动画事件  人移动 坐骑也要移动
        if (this.rideController != null) this.rideController.OnEntityEvent(entityEvent, param);
    }

    public void Ride(int rideId)
    {
        if (currentRide == rideId) return;
        currentRide = rideId;
        if(currentRide>0)
        {
            //加载坐骑
            this.rideController = GameObjectManager.Instance.LoadRide(rideId, this.transform);
        }
        else
        {
            Destroy(this.rideController.gameObject);
            this.rideController = null;
        }
        if (this.rideController == null)
        {
            this.anim.transform.localPosition = Vector3.zero;//// 重置动画位置
            this.anim.SetLayerWeight(1, 0); //关闭动画层1权重
        }
        else
        {
            this.rideController.SetRider(this);//设置自己是骑乘者
            this.anim.SetLayerWeight(1, 1);
        }
    }

    public void SetRidePosition(Vector3 position)
    {
        //绑定角色的位置永远在马屁股那里
        this.anim.transform.position = position + (this.anim.transform.position - this.rideBone.position);
    }

    
    private void SetIdleAnimation()
    {
        anim.SetBool("Move", false);  
        anim.SetBool("Run", false);  
    }

    
    private void SetMovementAnimation()
    {
        anim.SetBool("Move", true);  
        anim.SetBool("Run", false);  
    }


    
    private void SetRunAnimation()
    {
        anim.SetBool("Run", true);  
        anim.SetBool("Move", false);  
    }

    
    //跳跃事件
    private void Jump()
    {
        anim.SetTrigger("Jump");  

               

        // 等待跳跃动画播放后 才播放走路跑步音效
        StartCoroutine(JumpWaitTime());
    }

    
    IEnumerator JumpWaitTime()
    {
        yield return new WaitForSeconds(jumpTime[currentCharacterClass]);  
    }

    private void HandleRideEvent()
    {
        anim.SetTrigger("Ride");

    }

    void OnDestroy()
    {
        if (entity != null)  
        {
            Debug.LogFormat("消失的玩家：{0},位置{1}", entity.entityId, entity.position);  
        }
        if (UIWorldElementManager.Instance != null)
        {
            UIWorldElementManager.Instance.RemoveCharacterNameBar(this.transform);
        }
    }

    public void OnEntityChange(Entity entity, int param)
    {

    }
}