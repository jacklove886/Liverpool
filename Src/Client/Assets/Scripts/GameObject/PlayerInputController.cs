using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Entities;
using SkillBridge.Message;

public class PlayerInputController : MonoBehaviour {

	[Header("角色状态")]
    SkillBridge.Message.CharacterState state;//角色的状态(移动，停止)
    public Character character; //角色实体
    public bool isPlayer=false; //是否是玩家

    [Header("物理组件")]
    public Rigidbody rb;
    public EntityController entityController;//实体控制器

    [Header("移动参数")]
    public float rotateSpeed=2.0f;//旋转的速度
    public float turnAngle=10;//旋转角度的阈值
    public int speed;
    public bool onAir=false; //是否在空中

    [Header("位置同步")]
    private Vector3 lastPos;
	
	void Start () {
		state=SkillBridge.Message.CharacterState.Idle;
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
	
	void FixedUpdate () 
	{
		if(character==null)
		{
			return;
		}

		float v=Input.GetAxis("Vertical");//垂直处理

		if(v>0.01)
		{
			if(state!=SkillBridge.Message.CharacterState.Move)
			{
				state=SkillBridge.Message.CharacterState.Move;
				this.character.MoveForward();
				this.SendEntityEvent(EntityEvent.MoveFwd);
			}
			rb.velocity=
			rb.velocity.y*Vector3.up+GameObjectTool.LogicToWorld(character.direction)*(character.speed+9.81f)/100f;
		}
		else if(v<-0.01)
		{
			if(state!=SkillBridge.Message.CharacterState.Move)
			{
				state=SkillBridge.Message.CharacterState.Move;
                this.character.MoveBack();
				this.SendEntityEvent(EntityEvent.MoveBack);
			}
			rb.velocity=
			rb.velocity.y*Vector3.up+GameObjectTool.LogicToWorld(character.direction)*(character.speed+9.81f)/100f;
		}
		else
		{
			if (state != SkillBridge.Message.CharacterState.Idle)
        	{
            state = SkillBridge.Message.CharacterState.Idle;
            this.rb.velocity = Vector3.zero;
            this.character.Stop();
            this.SendEntityEvent(EntityEvent.Idle);
       		}
		}

		float h=Input.GetAxis("Horizontal");//水平处理
		if(h<-0.1||h>0.1)
		{
			this.transform.Rotate(0,h*rotateSpeed,0);
			Vector3 dir=GameObjectTool.LogicToWorld(character.direction);
			Quaternion rotation=new Quaternion();
			rotation.SetFromToRotation(dir,this.transform.forward);

			if(rotation.eulerAngles.y>this.turnAngle&&rotation.eulerAngles.y<(360-this.turnAngle))
			{
				character.SetDirection(GameObjectTool.WorldToLogic(this.transform.forward));
				rb.transform.forward=this.transform.forward;
				this.SendEntityEvent(EntityEvent.None);
			}
		}

		
		if (Input.GetButtonDown("Jump"))// 跳跃处理
		{
			this.SendEntityEvent(EntityEvent.Jump);
		}
	}

	private void LateUpdate()
	{
    Vector3 offset = this.rb.transform.position - lastPos;
    this.speed = (int)(offset.magnitude * 100f / Time.deltaTime);
    this.lastPos = this.rb.transform.position;

    if ((GameObjectTool.WorldToLogic(this.rb.transform.position) - this.character.position).magnitude > 50)
    	{
        this.character.SetPosition(GameObjectTool.WorldToLogic(this.rb.transform.position));
        this.SendEntityEvent(EntityEvent.None);
    	}
    this.transform.position = this.rb.transform.position;
	}

	void SendEntityEvent(EntityEvent entityEvent)
	{
		if(entityController!=null)
		{
			entityController.OnEntityEvent(entityEvent);
		}
	}
}
