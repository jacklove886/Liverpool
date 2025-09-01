using SkillBridge.Message;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Entities;

public class EntityController : MonoBehaviour {

    [Header("组件引用")]
    public Rigidbody rb;
    public Animator anim;
    //private AnimatorStateInfo currentstateInfo;//当前动画状态

    [Header("实体数据")]
    public Entity entity;

    [Header("位置和方向")]
    public UnityEngine.Vector3 position;    // Unity世界位置
    public UnityEngine.Vector3 direction;   // Unity世界方向
    //private Quaternion rotation;
    //public UnityEngine.Vector3 lastPosition;  // 上一帧位置
    //public Quaternion lastRotation;          // 上一帧旋转

    [Header("移动参数")]
    public float speed;                     //移动速度
    public float animSpeed=1.5f;            //动画速度
    public float jumpForce=3.0f;            //跳跃力度

    [Header("角色类型")]
    public bool isPlayer=false;             //是否是玩家
	
	void Start () {
		if(entity!=null)
		{
			this.UpdateTransform();
		}

		if(!isPlayer)
		{
			rb.useGravity=false; //不是玩家就不受重力的影响
		}
	}
	
	void UpdateTransform()
	{
		this.position = GameObjectTool.LogicToWorld(entity.position);
		this.direction = GameObjectTool.LogicToWorld(entity.direction);

		this.rb.MovePosition(this.position);
		this.transform.forward = this.direction;
		//this.lastPosition = this.position;
		//this.lastRotation = this.rotation;
	}
	
	void FixedUpdate () 
	{
		if(entity==null)
		{
			return;
		}
		this.entity.OnUpdate(Time.fixedDeltaTime);//更新实体的逻辑

		if(!isPlayer)
		{
			this.UpdateTransform();//不是玩家需要同步位置
		}
	}

	public void OnEntityEvent(EntityEvent entityEvent, float horizontal = 0, float vertical = 0)
    {
        switch (entityEvent)
        {
            case EntityEvent.EventIdle:
                anim.SetFloat("Horizontal", 0);
                anim.SetFloat("Vertical", 0);
                anim.SetBool("Move", false);
                break;
            case EntityEvent.EventMoveFwd:
            case EntityEvent.EventMoveBack:
            case EntityEvent.EventMoveLeft:
            case EntityEvent.EventMoveRight:
                // 用H和V的值
                Debug.LogFormat("设置动画参数: H={0}, V={1}, Move={2}", horizontal, vertical, true);
                anim.SetFloat("Horizontal", horizontal);
                anim.SetFloat("Vertical", vertical);
                anim.SetBool("Move", true);
                break;
            case EntityEvent.EventJump:
                anim.SetTrigger("Jump");
                break;
        }
    }

    void OnDestroy()
    {
        if (entity != null)
            Debug.LogFormat("消失的玩家：{0} " ,entity.entityId);

        /*if (UIWorldElementManager.Instance != null)
        {
            UIWorldElementManager.Instance.RemoveCharacterNameBar(this.transform);
        }*/
    }
}
