using Entities;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UINameBar : MonoBehaviour {

    public Text characterName;
    public Character character;
    public Transform owner;  //名称跟随角色
    public Transform Camera;//摄像机位置
    public float height = 2.0f;//姓名条离角色的距离


    void Start ()
    {
        
    }
	
	
	void Update ()
    {
        UpdateInfo();
        if (owner != null)
        {
            transform.position = owner.position + Vector3.up * height;//永远在玩家头上
            transform.forward = Camera.transform.forward;//永远朝向摄像机
        }
    }

    void UpdateInfo()
    {
        if (this.character != null)
        {
            string name = character.Name + "  " + character.Info.Level+"级";
            if(name!= characterName.text)
            {
                characterName.text = name;
            }
        }
    }
}
