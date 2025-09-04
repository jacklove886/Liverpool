using SkillBridge.Message;  
using System.Collections;  
using UnityEngine;  
using Entities;  
using Models;
using Managers;

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

    public void OnEntityEvent(EntityEvent entityEvent)
    {
        int currentCharacter = (int)User.Instance.CurrentCharacter.Class;  // 获取当前角色职业类型

        switch (entityEvent)
        {
            case EntityEvent.EventIdle:

                StopMovementAudio();  
                SetIdleAnimation();  
                break;

            case EntityEvent.EventMove:

                SetMovementAnimation();  
                PlayMovementAudio();  
                break;

            case EntityEvent.EventRun:

                SetRunAnimation(); 
                PlayRunAudio();  
                break;

            case EntityEvent.EventJump:

                HandleJumpEvent(); 
                break;
        }
    }

    
    private void StopMovementAudio()
    {
        if (AudioManager.Instance.audioClipPlay.isPlaying)  
        {
            AudioManager.Instance.audioClipPlay.Stop();  
        }
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

    
    private void PlayMovementAudio()
    {
        AudioManager.Instance.audioClipPlay.clip = AudioManager.Instance.walkAudioClip[currentCharacterClass];  
        AudioManager.Instance.audioClipPlay.Play();  
    }

    
    private void SetRunAnimation()
    {
        anim.SetBool("Run", true);  
        anim.SetBool("Move", false);  
    }

    
    private void PlayRunAudio()
    {
        AudioManager.Instance.audioClipPlay.clip = AudioManager.Instance.runAudioClip[currentCharacterClass];  
        AudioManager.Instance.audioClipPlay.Play();  
    }

    //跳跃事件
    private void HandleJumpEvent()
    {
        anim.SetTrigger("Jump");  

        
        StopMovementAudio();

        
        AudioManager.Instance.jumpaudioClipPlay.PlayOneShot(AudioManager.Instance.jumpAudioClip[currentCharacterClass]);

        // 等待跳跃动画播放后 才播放走路跑步音效
        StartCoroutine(JumpWaitTime());
    }

    
    IEnumerator JumpWaitTime()
    {
        yield return new WaitForSeconds(jumpTime[currentCharacterClass]);  
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

    public void OnEntityChange(Entity entity)
    {

    }
}