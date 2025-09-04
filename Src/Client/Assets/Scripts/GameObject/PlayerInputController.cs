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
    private float lastSyncTime = 0;
    private float syncWaitTime = 0.1f; // 每0.1秒同步一次

    void Start () {
        state =SkillBridge.Message.CharacterState.Idle;
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
                if (state != CharacterState.Move)
                {
                    state = CharacterState.Move;
                    currentspeed = this.character.Move();
                }
                if (Mathf.Abs(vertical) >= Mathf.Abs(horizontal)) // 前后移动为主
                {
                    if (vertical > 0)
                    {
                        this.SendEntityEvent(EntityEvent.EventMoveFwd, horizontal, vertical);
                    }
                    else
                    {
                        this.SendEntityEvent(EntityEvent.EventMoveBack, horizontal, vertical);
                    }
                }
                else // 左右移动为主
                {
                    if (horizontal > 0)
                    {
                        this.SendEntityEvent(EntityEvent.EventMoveRight, horizontal, vertical);
                    }
                    else
                    {
                        this.SendEntityEvent(EntityEvent.EventMoveLeft, horizontal, vertical);
                    }
                }
            }
            
            else
            {
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

        else
        {
            if (state != CharacterState.Idle)
            {
                state = CharacterState.Idle;
                this.rb.velocity = Vector3.zero;
                currentspeed=this.character.Stop();
                this.SendEntityEvent(EntityEvent.EventIdle, 0, 0);
            }
        }


        // 按空格实现跳跃
        if (Input.GetButtonDown("Jump"))
        {
            this.SendEntityEvent(EntityEvent.EventJump, 0, 0);
        }
    }

    private void LateUpdate()
	{
        if (character == null) return;

        Vector3 offset = this.rb.transform.position - lastPos;
        this.realspeed = (int)(offset.magnitude * 100f / Time.deltaTime);
        this.lastPos = this.rb.transform.position;

        if ((GameObjectTool.WorldToLogic(this.rb.transform.position) - this.character.position).magnitude > 50)
    	{
            this.character.SetPosition(GameObjectTool.WorldToLogic(this.rb.transform.position));
            this.SendEntityEvent(EntityEvent.EventNone);
    	}
        this.transform.position = this.rb.transform.position;
	}

	void SendEntityEvent(EntityEvent entityEvent, float horizontal = 0, float vertical = 0)
	{
		if(entityController!=null)
		{
			entityController.OnEntityEvent(entityEvent, horizontal, vertical);
		}

        if (Time.time - lastSyncTime > syncWaitTime)
        {
            MapService.Instance.SendMapEntitySync(entityEvent, character.EntityData);
            lastSyncTime = Time.time;
        }
    }
}
