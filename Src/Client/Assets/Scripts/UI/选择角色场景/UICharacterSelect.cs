using Models;
using SkillBridge.Message;
using Services;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Linq;
using Managers;

public class UICharacterSelect : MonoBehaviour
{
    [Header("基础信息")]
    public SkillBridge.Message.NCharacterInfo info;
    private CharacterClass charClass;                            // 枚举值 获得角色的int值
    private const int classCount = 3;                           // 职业数量常量
    private int currentIndex;                                   //角色当前列表中的索引
    public int classIndex;                                     //角色当前职业中的索引

    [Header("选择/创建面板")]
    public GameObject panelSelect;                              
    public GameObject panelCreate;                              

    [Header("生成信息的父物体")]
    public Transform uiCharList;                                

    [Header("滚动条角色信息面板的预制体")]
    public GameObject uiCharacterMessage;                       

    [Header("创建面板滚动条中角色列表")]
    public List<GameObject> uiChars = new List<GameObject>();   

    [Header("职业介绍")]
    public Text descs;                                         

    [Header("创建界面填写昵称的框")]
    public InputField nameInputField;                          

    [Header("添加角色的按钮")]
    public Button creatCharacter;                             

    [Header("选择角色类型的按钮")]
    public Button[] selectClass;                              

    [Header("已经创建好的角色的按钮")]
    public Button selectCharacter;                             

    [Header("最上面的角色图标")]
    public Image[] titles;                                      

    [Header("职业图标")]
    public Image[] confirmPicture;                              

    [Header("角色3D模型")]
    public GameObject[] characterClassPrefab;                  

    [Header("角色背景图")]
    public Image[] imageBackGround;                            

    [Header("初始面板")]
    public GameObject originalPanel;                            

    private void Awake()
    {
        UserService.Instance.OnCharacterCreate += OnCharacterCreate;
        UserService.Instance.OnCharacterDelete += OnCharacterDelete;
    }

    void Start()
    {
        InitCharacterSelect(true); 
    }

    private void OnDestroy()
    {
        UserService.Instance.OnCharacterCreate -= OnCharacterCreate;
        UserService.Instance.OnCharacterDelete -= OnCharacterDelete;
    }

    //初始化选择角色页面
    public void InitCharacterSelect(bool init)
    {
        originalPanel.SetActive(true);
        panelCreate.SetActive(false);
        panelSelect.SetActive(true);
        SoundManager.Instance.audioClipPlay.clip= SoundManager.Instance.openChooseCharacterClip;
        SoundManager.Instance.audioClipPlay.Play();
        if (init)
        {
            foreach (var old in uiChars)
            {
                Destroy(old);
            }
            uiChars.Clear();
        }
        // 检查是否为空
        if (User.Instance == null ||
        User.Instance.Info == null ||
        User.Instance.Info.Player == null ||
        User.Instance.Info.Player.Characters == null)
        {
            Debug.LogWarning("角色列表为空，无法初始化角色选择界面");
            return;
        }
        print("当前角色数量为" + User.Instance.Info.Player.Characters.Count);

        for (int i = 0; i < User.Instance.Info.Player.Characters.Count; i++)
        {
            var cha = User.Instance.Info.Player.Characters[i];
            GameObject go = Instantiate(uiCharacterMessage);
            go.transform.SetParent(uiCharList, false);
            go.name = cha.Name;
            UICharacterMessage charInfo = go.GetComponent<UICharacterMessage>();
            charInfo.SetInfo(User.Instance.Info.Player.Characters[i]);

            Button button = go.GetComponentInChildren<Button>();
            int index = i;
            button.onClick.AddListener(() =>
            {
                OnClickSelectCharacter(index);
            });
            uiChars.Add(go);
            go.SetActive(true);
        }
        for (int j = 0; j < classCount; j++)
        {
            characterClassPrefab[j].SetActive(false);
        }

    }


    //点击创建角色的按钮
    public void OnClickCreateCharater()
    {
        panelSelect.SetActive(false);
        panelCreate.SetActive(true);
        OnClickSelectClass(1);//默认选了第一个职业
        originalPanel.SetActive(false);
    }


    //点击选择职业类型的按钮
    public void OnClickSelectClass(int charClass)
    {
        this.charClass = (CharacterClass)charClass;

        //角色类型有几个就小于几 改常量就行
        for (int i = 0; i < classCount; i++)
        {
            //标题改为相应的职业
            titles[i].gameObject.SetActive(i == charClass - 1);
            //按钮改为相应的职业
            confirmPicture[i].gameObject.SetActive(i == charClass - 1);
            //背景图片改为相应的职业
            imageBackGround[i].gameObject.SetActive(i == charClass - 1);
            //角色3D模型控制
            characterClassPrefab[i].SetActive(i == charClass - 1);
            //选择角色播放音效
            if (i == charClass - 1)
            {
                SoundManager.Instance.audioClipPlay.PlayOneShot(SoundManager.Instance.characterAudioClip1[i]);
                Animator animator = characterClassPrefab[i].GetComponentInChildren<Animator>();
                animator.SetTrigger("SelectClass");
            }
            //播放动画

        }
        descs.text = DataManager.Instance.Characters[charClass].Description;
    }


    //点击选择已经创建好的角色的按钮
    public void OnClickSelectCharacter(int index)
    {
        originalPanel.SetActive(false);
        var character = User.Instance.Info.Player.Characters[index];
        User.Instance.CurrentCharacter = character;
        // 获得一个索引值来匹配当前选中的角色
        classIndex = GetClassIndex(character.Class);

        // 控制角色按钮高亮以及删除按钮和自拍照的显示(逻辑都是一样的 只有当前选中的才生效)
        for (int i = 0; i < User.Instance.Info.Player.Characters.Count; i++)
        {
            UICharacterMessage uICharacterMessage = uiChars[i].GetComponent<UICharacterMessage>();
            uICharacterMessage.selected = (index == i);
            uICharacterMessage.deleteButton.gameObject.SetActive(index == i);
            uICharacterMessage.imageEmpty.gameObject.SetActive(index == i);
        }

        //控制角色3D模型显示和删除按钮的显示
        for (int i = 0; i < classCount; i++)
        {
            characterClassPrefab[i].SetActive(i == classIndex);

            imageBackGround[i].gameObject.SetActive(i == classIndex);
            if (i == classIndex)
            {
                Animator animator = characterClassPrefab[i].GetComponentInChildren<Animator>();
                animator.SetTrigger("Click");
                SoundManager.Instance.audioClipPlay.clip = SoundManager.Instance.characterAudioClip2[(int)User.Instance.CurrentCharacter.Class - 1];
                SoundManager.Instance.audioClipPlay.Play();
            }
        }
        currentIndex = index;
    }

    // 获取职业对应的数组索引
    private int GetClassIndex(SkillBridge.Message.CharacterClass characterClass)
    {
        switch (characterClass)
        {
            case SkillBridge.Message.CharacterClass.Warrior: return 0;
            case SkillBridge.Message.CharacterClass.Wizard: return 1;
            case SkillBridge.Message.CharacterClass.Archer: return 2;
            default: return 0;
        }
    }

    //点击创建角色的按钮  创建完角色
    public void OnClickCreateCharacterSuccess()
    {
        if (string.IsNullOrEmpty(nameInputField.text))
        {
            MessageBox.Show("请输入角色昵称");
            return;
        }
        UserService.Instance.SendCharacterCreate(this.nameInputField.text, this.charClass);
    }

    //点击删除按钮 删除角色
    public void OnClickDeleteCharacter()
    {
        if (User.Instance.CurrentCharacter == null)
        {
            MessageBox.Show("请选择要删除的角色");
            return;
        }
        else
        {
            UIMessageBox msgBox= MessageBox.Show("确认要删除角色吗？", "删除角色", MessageBoxType.Confirm, "确认", "取消");
            msgBox.OnYes = () =>
            {
                // 发送删除请求，传入当前选中角色的名字
                UserService.Instance.SendCharacterDelete(User.Instance.CurrentCharacter.Name);
            };
            msgBox.OnNo = () =>
            {
                //什么都不做;
            };
        }
    }

    //直接点击开始游戏的按钮
    public void OnClickStartGame()
    {
        //如果选了某个角色 可以进入主城 否则请选择角色
        if (User.Instance.CurrentCharacter == null)
        {
            MessageBox.Show("请选择角色");
            return;
        }
        else
        {
            //传入进入游戏角色的索引值(按职业划分的)
            UserService.Instance.SendGameEnter(currentIndex);
        }
    }

    //绑定的事件
    void OnCharacterCreate(Result result, string message)
    {
        if (result == Result.Success)
        {
            // 清空昵称输入框
            nameInputField.text = "";
            MessageBox.Show("角色创建成功！");
            InitCharacterSelect(true);
        }
        else
        {
            MessageBox.Show(message, "创建失败", MessageBoxType.Error);
        }
    }

    void OnCharacterDelete(Result result, string message)
    {
        if (result == Result.Success)
        {
            MessageBox.Show("角色删除成功");
            User.Instance.CurrentCharacter = null; // 清空当前选中的角色
            InitCharacterSelect(true);
        }
        else
        {
            MessageBox.Show(message, "删除失败", MessageBoxType.Error);
        }
    }
}
