using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Entities;
using SkillBridge.Message;
using Models;
using Managers;
using Services;
using UnityEngine.AI;

public class PlayerInputController : MonoBehaviour {

    [Header("角色状态")]
    SkillBridge.Message.CharacterState state;//角色的状态(移动，停止)
    public Character character; //角色实体

    [Header("物理组件")]
    public Rigidbody rb;
    public EntityController entityController;//实体控制器

    [Header("移动参数")]
    public int currentSpeed;//目前速度
    public int NavSpeed;//导航速度
    public bool isGround=true; //是否在地面
    public bool isRunning = false; //是否在跑步
    public float vertical;
    public float horizontal;
    public bool isRide;

    [Header("位置同步")]
    private Vector3 lastPos;// 上次同步的位置

    [Header("旋转同步")]
    private float lastSyncRotation = 0f;  // 上次同步的旋转角度

    private NavMeshAgent agent;//导航代理

    private bool autoNav = false;//当前是否寻路

    void Start () {
        state =CharacterState.Idle;
		if(entityController!=null)
		{
			entityController.entity=this.character;
		}
        if (agent == null)
        {
            agent = this.gameObject.AddComponent<NavMeshAgent>();//添加代理组件
            agent.stoppingDistance = 3f;//离目标点3距离停止
        }
    }

    public void StartNav(Vector3 target)//开始寻路
    {
        StartCoroutine(BeginNav(target));
    }

    IEnumerator BeginNav(Vector3 target)
    {
        agent.SetDestination(target);//设置目标点
        yield return null;
        autoNav = true;
        if (state != CharacterState.Move)
        {
            state = CharacterState.Run;
            NavSpeed = character.Run();
            SendEntityEvent(EntityEvent.EventRun);
            agent.speed = NavSpeed / 100f;
        }
    }

    public void StopNav()//结束寻路
    {
        autoNav = false;
        MainPlayerCamera.Instance.SetMouseControl(true);
        agent.ResetPath();//清空路径
        if (state != CharacterState.Idle)
        {
            state = CharacterState.Idle;
            rb.velocity = Vector3.zero;
            character.Stop();
            SendEntityEvent(EntityEvent.EventIdle);
        }
        NavPathRender.Instance.SetPath(null, Vector3.zero);
    }

    public void NavMove()
    {
        MainPlayerCamera.Instance.SetMouseControl(false);
        if (agent.pathPending || agent.pathStatus != NavMeshPathStatus.PathComplete)
        { return; }//寻路没完成

        if (agent.pathStatus == NavMeshPathStatus.PathInvalid)//寻路失败
        {
            StopNav();
            return;
        }

        if (Mathf.Abs(Input.GetAxis("Vertical")) > 0.1f || Mathf.Abs(Input.GetAxis("Horizontal")) > 0.1f)
        {
            StopNav();
            return;
        }
        NavPathRender.Instance.SetPath(agent.path, agent.destination);//更新实时路径

        if (agent.isStopped || agent.remainingDistance < 3f)//寻路停止或者离目标距离小于3
        {
            StopNav();
            return;
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            UIManager.Instance.Show<UISetting>();
        }
        if (Input.GetKeyDown(KeyCode.R))
        {
            if (entityController.currentRide == 0)
            {
                User.Instance.Ride(ItemManager.Instance.GetRideId());
            }
            else
            {
                User.Instance.Ride(entityController.currentRide);
            }          
        }
    }

    void FixedUpdate()
    {
        if (character == null) return;

        if (autoNav)
        {
            NavMove();
            return;
        }

        if (InputManager.Instance!=null&&InputManager.Instance.IsInputMode) return;//如果正在输入模式
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
                    if (isRide)
                    {
                        currentSpeed = this.character.Ride();
                    }
                    if (!isRide)
                    {
                        currentSpeed = this.character.Move();
                    }                   
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
                    if (isRide)
                    {
                        currentSpeed = this.character.Ride();
                    }
                    if (!isRide)
                    {
                        currentSpeed = this.character.Run();
                    }
                    this.SendEntityEvent(EntityEvent.EventRun);
                }
            }

            // 角色移动
            Vector3 moveDirection = (transform.forward * vertical + transform.right * horizontal).normalized;
            float speedzoom = currentSpeed / 100f;
            rb.velocity = new Vector3(moveDirection.x * speedzoom, rb.velocity.y, moveDirection.z * speedzoom);
        }

        //Idle状态
        else
        {
            if (state != CharacterState.Idle)
            {
                state = CharacterState.Idle;
                this.rb.velocity = Vector3.zero;
                currentSpeed=this.character.Stop();
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
        this.lastPos = this.rb.transform.position;

        Vector3Int goLogicPos = GameObjectTool.WorldToLogic(this.rb.transform.position);
        float positionOffset = (goLogicPos - this.character.position).magnitude;

        float currentRotation = this.transform.eulerAngles.y;

        // 计算旋转差值
        float rotationOffset = Mathf.Abs(Mathf.DeltaAngle(lastSyncRotation, currentRotation));

        if (positionOffset > 1f|| rotationOffset>2f)
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


    public void SendEntityEvent(EntityEvent entityEvent,int param=0)
    {
        // 本地动画立即执行
        if (entityController != null)
        {
            entityController.OnEntityEvent(entityEvent,param);
        }

        MapService.Instance.SendMapEntitySync(entityEvent, character.EntityData, param);
    }
}
