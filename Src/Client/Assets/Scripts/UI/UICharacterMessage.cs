using SkillBridge.Message;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UICharacterMessage : MonoBehaviour
{
    [Header("角色信息显示")]
    public Text charLevel;//角色等级
    public Text charClass;//角色职业
    public Text charName;//角色姓名
    public Image imageEmpty; // 角色空图像（用来存储自拍照）
    [Header("删除按钮")]
    public Button deleteButton; // 删除按钮
    [Header("自拍照")]
    public Sprite[] imageCharacter; // 角色自拍照
    [Header("选中效果")]
    public Image highlight;//图片高亮
    public bool selected
    {
        get { return highlight.enabled; }
        set { highlight.enabled = value; }
    }

    public SkillBridge.Message.NCharacterInfo info;

    void Start()
    {
        UpdateUI();

    }

    void UpdateUI()
    {
        if (info != null)
        {
            this.charLevel.text = this.info.Level.ToString() + "级";
            this.charName.text = this.info.Name;
            if (info.Class.ToString() == "Warrior")
            {
                charClass.text = "战士";
                imageEmpty.sprite = imageCharacter[0];      
            }

            else if (info.Class.ToString() == "Wizard")
            {
                charClass.text = "法师";
                imageEmpty.sprite = imageCharacter[1];
            }
               
            else if (info.Class.ToString() == "Archer")
            {
                charClass.text = "游侠";
                imageEmpty.sprite = imageCharacter[2];
            }

        }
    }

    //设置角色信息
    public void SetInfo(SkillBridge.Message.NCharacterInfo newInfo)
    {
        this.info = newInfo;
        UpdateUI();
    }

}
