using System;
using System.Collections.Generic;
using Entities;
using SkillBridge.Message;
using UnityEngine;

public class RideController: MonoBehaviour
{
    public Transform mountPoint;//骑乘点 坐垫
    public EntityController rider;//骑乘者
    public Vector3 offset;//偏移
    private Animator anim;

    private void Start()
    {
        this.anim = GetComponent<Animator>();
    }

    private void Update()
    {
        if (this.mountPoint == null || this.rider == null) return;
        //TransformDirection变换方向
        this.rider.SetRidePosition(this.mountPoint.position + this.mountPoint.TransformDirection(this.offset));
    }

    internal void SetRider(EntityController rider)
    {
        this.rider = rider;
    }


    public void OnEntityEvent(EntityEvent entityEvent, int param)
    {
        switch (entityEvent)
        {
            case EntityEvent.EventIdle:
                anim.SetBool("Move", false);
                anim.SetTrigger("Idle");
                break;

            case EntityEvent.EventMove:
                anim.SetBool("Move", true);
                break;
            default: break;
        }
    }
}
