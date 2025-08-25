using Models;
using SkillBridge.Message;
using Services;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class UICharacterSelect : MonoBehaviour
{
    CharacterClass charClass;//枚举值 获得角色的int值
    [Header("选择/创建面板")]
    public GameObject panelSelect;//角色选择面板
    public GameObject panelCreate;//角色创建面板
    [Header("职业介绍")]
    public Text descs;//职业介绍
    [Header("添加角色的按钮")]
    public  Button[] creatCharacter;//添加角色的按钮
    [Header("选择角色类型的按钮按钮")]
    public Button[] selectClass;//选择角色类型的按钮
    [Header("已经创建好的角色的按钮")]
    public Button[] selectCharacter; //已经创建好的角色的按钮
    [Header("最上面的角色图标")]
    public Image[] titles;//最上面的角色图标
    [Header("职业图标")]
    public Image[] confirmPicture;//职业图标
    public UICharacterView uICharacterView;//实例化UICharacterView类
    private int selectCharacterIdx = -1;

    // Use this for initialization
    void Start ()
    {
        InitCharacterSelect(true);

    }
    //初始化UI页面
    public void InitCharacterSelect(bool init)
    {
        panelCreate.SetActive(false);
        panelSelect.SetActive(true);

    }
    // Update is called once per frame
    void Update ()
    {


     }
    //点击创建角色的按钮
    public void onClickCreateCharacer()
    {
        panelSelect.SetActive(false);
        panelCreate.SetActive(true);
    }
    //点击选择职业类型的按钮
    public void onClickSelectClass(int charClass)
    {
        this.charClass = (CharacterClass)charClass;
        uICharacterView.CurrentCharacter = charClass - 1;
        for (int i = 0; i < 3; i++)
        {
            //标题改为相应的职业
            titles[i].gameObject.SetActive(i == charClass - 1);
            //按钮改为相应的职业
            confirmPicture[i].gameObject.SetActive(i == charClass - 1);
        }
        //descs.text = DataManager.Instance.Characters[charClass].Description;
    }
    //点击选择已经创建好的角色的按钮
    public void onClickSelectCharacter(int idx)
    {
        selectCharacterIdx = idx;
        var cha = User.Instance.Info.Player.Characters[idx];
        Debug.LogFormat("Select Char:[{0}]{1}[{2}]", cha.Id, cha.Name, cha.Class);
        User.Instance.CurrentCharacter = cha;
        uICharacterView.CurrentCharacter = idx;
    }

}
