using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Entities;
using SkillBridge.Message;
using Models;
using Managers;
using Services;

public class PlayerInputController : MonoBehaviour {

	[Header("角色状态")]
    SkillBridge.Message.CharacterState state;//角色的状态(移动，停止)
    public Character character; //角色实体

    [Header("物理组件")]
    public Rigidbody rb;
    public EntityController entityController;//实体控制器

    [Header("移动参数")]
    public int currentspeed;//目前速度
    public int realspeed;//真实速度
    public bool isGround=true; //是否在地面
    public bool isRunning = false; //是否在跑步
    public float vertical;
    public float horizontal;

    [Header("位置同步")]
    private Vector3 lastPos;// 上次同步的位置

    [Header("旋转同步")]
    private float lastSyncRotation = 0f;  // 上次同步的旋转角度


    void Start () {
        state =CharacterState.Idle;
		if(this.character==null)
		{
			DataManager.Instance.Load();
			NCharacterInfo characterinfo=new NCharacterInfo();
			characterinfo.Id=1;
			characterinfo.Name="玩家1";
			characterinfo.Tid=1;
			characterinfo.Entity=new NEntity();
			characterinfo.Entity.Position=new NVector3();
			characterinfo.Entity.Direction=new NVector3();
			characterinfo.Entity.Direction.X=0;
			characterinfo.Entity.Direction.Y=100;
			characterinfo.Entity.Direction.Z=0;
			this.character=new Character(characterinfo);

			if(entityController!=null)
			{
				entityController.entity=this.character;
			}
		}
    }

    void FixedUpdate()
    {
        if (character == null) return;

        vertical = Input.GetAxis("Vertical");   
        horizontal = Input.GetAxis("Horizontal");
        // 按住shift进入跑步状态
        isRunning = Input.GetKey(KeyCode.LeftShift);

        // 移动处理
        if (Mathf.Abs(vertical) > 0.01f || Mathf.Abs(horizontal) > 0.01f)
        {
            if (!isRunning)
            {
                // 只在状态或方向变化时发送
                if (state != CharacterState.Move)
                {
                    state = CharacterState.Move;
                    currentspeed = this.character.Move();
                    this.SendEntityEvent(EntityEvent.EventMove);
                }
            }

            //跑步状态
            else
            {
                // 只在状态变化时发送
                if (state != CharacterState.Run)
                {
                    state = CharacterState.Run;
                    currentspeed = this.character.Run();
                    this.SendEntityEvent(EntityEvent.EventRun);
                }
            }

            // 角色移动
            Vector3 moveDirection = (transform.forward * vertical + transform.right * horizontal).normalized;
            float speedzoom = currentspeed / 100f;
            rb.velocity = new Vector3(moveDirection.x * speedzoom, rb.velocity.y, moveDirection.z * speedzoom);
        }

        //Idle状态
        else
        {
            if (state != CharacterState.Idle)
            {
                state = CharacterState.Idle;
                this.rb.velocity = Vector3.zero;
                currentspeed=this.character.Stop();
                this.SendEntityEvent(EntityEvent.EventIdle);
            }
        }


        // 按空格实现跳跃
        if (Input.GetButtonDown("Jump"))
        {
            this.SendEntityEvent(EntityEvent.EventJump);
        }
    }

    private void LateUpdate()
	{
        if (character == null) return;

        Vector3 offset = this.rb.transform.position - lastPos;
        this.realspeed = (int)(offset.magnitude * 100f / Time.deltaTime);
        this.lastPos = this.rb.transform.position;

        Vector3Int goLogicPos = GameObjectTool.WorldToLogic(this.rb.transform.position);
        float logicOffset = (goLogicPos - this.character.position).magnitude;

        float currentRotation = this.transform.eulerAngles.y;

        // 计算旋转差值
        float rotationOffset = Mathf.Abs(Mathf.DeltaAngle(lastSyncRotation, currentRotation));

        if (logicOffset > 5f|| rotationOffset>5f)
        {
            this.character.SetPosition(GameObjectTool.WorldToLogic(this.rb.transform.position));//同步位置

            Vector3 forwardDirection = this.transform.forward;
            Vector3Int logicDirection = GameObjectTool.WorldToLogic(forwardDirection);
            this.character.SetDirection(logicDirection);//同步旋转
            lastSyncRotation = currentRotation;// 记录本次同步的旋转角度

            this.SendEntityEvent(EntityEvent.EventNone);//发送更新事件
        }
        this.transform.position = this.rb.transform.position;
	}


    void SendEntityEvent(EntityEvent entityEvent)
    {
        // 本地动画立即执行
        if (entityController != null)
        {
            entityController.OnEntityEvent(entityEvent);
        }

        MapService.Instance.SendMapEntitySync(entityEvent, character.EntityData);
    }
}
