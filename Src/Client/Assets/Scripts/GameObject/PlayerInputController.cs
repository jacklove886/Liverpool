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
    private Vector3 lastPos;
	//private float lastSync = 0;

    [Header("旋转同步")]
    private Vector3 lastDirection; // 上一帧的方向
    //private float rotationSyncThreshold = 5f; // 旋转同步

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
        lastDirection = this.transform.forward;
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
            rb.velocity = new Vector3(moveDirection.x * currentspeed / 100f, rb.velocity.y, moveDirection.z * currentspeed / 100f);
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
        if (logicOffset > 100)
        {
            this.character.SetPosition(GameObjectTool.WorldToLogic(this.rb.transform.position));
            this.SendEntityEvent(EntityEvent.EventNone);
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
