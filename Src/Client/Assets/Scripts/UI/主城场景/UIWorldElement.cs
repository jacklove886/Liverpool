using Entities;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIWorldElement : MonoBehaviour {

    public float height = 2.0f;//姓名条离角色的距离
    public Transform owner;  //名称跟随角色
    public Transform Camera;//摄像机位置
    public Character character;

    public void SetHeight(Character character)
    {
        if (character.Info.Name == "古神祭司"|| character.Info.Name == "落日恶鬼")
        {
            height = 3f;
        }
    }

    void Update ()
    {
        this.transform.position = owner.position + Vector3.up * height;//永远在玩家头上
        if (Camera != null) 
        this.transform.forward = Camera.transform.forward;//永远朝向摄像机
    }
}
