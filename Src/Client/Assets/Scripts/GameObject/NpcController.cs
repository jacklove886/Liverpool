using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Common.Data;
using Managers;
using Models;

public class NpcController : MonoBehaviour {

    public int npcId;
    private Animator anim;
    public bool inInteractive;//正在对话中

    private Color originColor;
    private Renderer meshrenderer;

    NpcDefine npc;


    void Start()
    {
        //获取组件
        npc = NpcManager.Instance.GetNpcDefine(npcId);//获取NPC的数据
        anim = gameObject.GetComponent<Animator>();
        meshrenderer = gameObject.GetComponentInChildren<SkinnedMeshRenderer>();

        originColor = meshrenderer.sharedMaterial.color;//原始颜色是初始化时候的颜色
        StartCoroutine(Actions());
    }

    //休闲动作的行为
    IEnumerator Actions()
    {
        while (true)
        {
            if (inInteractive) yield return new WaitForSeconds(3f);//如果在交互 等两秒
            else yield return new WaitForSeconds(Random.Range(8f, 15f));//等8-15秒播放Relax动画
            Relax();
        }
    }

    void Relax()
    {
        anim.SetTrigger("Relax");
    }

    private void Interactive()
    {
        if (!inInteractive)
        {
            inInteractive = true;
            StartCoroutine(DoInteractive());
        }
    }

    //交互方法
    IEnumerator DoInteractive()
    {
        yield return FaceToPlayer();//面向玩家
        if (NpcManager.Instance.Interactive(npc))//调用NpcManagaer的方法
        {
            anim.SetTrigger("Talk");
        }
        yield return new WaitForSeconds(3f);//防止三秒内重复点击
        inInteractive = false;
    }

    IEnumerator FaceToPlayer()
    {
        Vector3 faceTo = (User.Instance.CurrentCharacterObject.transform.position - this.transform.position).normalized;
        //Angle计算的是两个向量之间的夹角
        while (Mathf.Abs(Vector3.Angle(this.gameObject.transform.forward, faceTo)) > 5)//角度差大于5度
        {
            this.gameObject.transform.forward = Vector3.Lerp(this.gameObject.transform.forward, faceTo, Time.deltaTime * 5f);
            yield return null;
        }
    }

    private void OnMouseDown()
    {
        Interactive();
    }

    private void OnMouseOver()
    {
        Highlight(true);
    }

    private void OnMouseEnter()
    {
        Highlight(true);
    }

    private void OnMouseExit()
    {
        Highlight(false);
    }

    void Highlight(bool highlight)
    {
        if (highlight)
        {
            if (meshrenderer.material.color != Color.white)
                meshrenderer.material.color =  Color.white;//白色代替原始颜色
        }
        else 
        {
            if (meshrenderer.material.color != originColor)
                meshrenderer.material.color =  originColor;//取消高亮 颜色变回去
        }
    }
}

