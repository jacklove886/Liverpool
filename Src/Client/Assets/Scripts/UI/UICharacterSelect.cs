using Models;
using SkillBridge.Message;
using Services;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class UICharacterSelect : MonoBehaviour
{
    [Header("基础信息")]
    public SkillBridge.Message.NCharacterInfo info;
    private CharacterClass charClass;                            // 枚举值 获得角色的int值
    private const int classCount = 3;                           // 职业数量常量

    [Header("选择/创建面板")]
    public GameObject panelSelect;                              // 角色选择面板
    public GameObject panelCreate;                              // 角色创建面板

    [Header("生成信息的父物体")]
    public Transform uiCharList;                                // 生成信息的父物体

    [Header("滚动条角色信息面板的预制体")]
    public GameObject uiCharacterMessage;                       // 滚动条角色信息面板的预制体

    [Header("创建面板滚动条中角色列表")]
    public List<GameObject> uiChars = new List<GameObject>();   // 滚动条中角色列表

    [Header("职业介绍")]
    public Text descs;                                          // 职业介绍

    [Header("创建界面填写昵称的框")]
    public InputField nameInputField;                           // 创建界面填写昵称的框

    [Header("添加角色的按钮")]
    public Button creatCharacter;                               // 添加角色的按钮

    [Header("选择角色类型的按钮")]
    public Button[] selectClass;                                // 选择角色类型的按钮

    [Header("已经创建好的角色的按钮")]
    public Button selectCharacter;                              // 已经创建好的角色的按钮

    [Header("最上面的角色图标")]
    public Image[] titles;                                      // 最上面的角色图标

    [Header("职业图标")]
    public Image[] confirmPicture;                              // 职业图标

    [Header("角色3D模型")]
    public GameObject[] characterClassPrefab;                   // 角色3D模型

    [Header("角色背景图")]
    public Image[] imageBackGround;                             // 角色背景图

    [Header("音效播放器")]
    public AudioSource audioClipPlay;                           // 音效播放器

    [Header("角色音效")]
    public AudioClip[] characterAudioClip1;                      // 角色音效

    [Header("角色音效")]
    public AudioClip[] characterAudioClip2;                      // 角色音效


    private void Awake()
    {
        UserService.Instance.OnCharacterCreate += OnCharacterCreate;
    }

    void Start()
    {
        DataManager.Instance.Load();//等以后可以写在LoadManager里
        InitCharacterSelect(true);
    }

    private void OnDestroy()
    {
        UserService.Instance.OnCharacterCreate -= OnCharacterCreate;
    }

    //初始化选择角色页面
    public void InitCharacterSelect(bool init)
    {
        panelCreate.SetActive(false);
        panelSelect.SetActive(true);
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
        print("当前创建角色数量为" + User.Instance.Info.Player.Characters.Count);

        for (int i = 0; i < User.Instance.Info.Player.Characters.Count; i++)
        {
            var cha = User.Instance.Info.Player.Characters[i];
            GameObject go = Instantiate(uiCharacterMessage);
            go.transform.SetParent(uiCharList, false);
            go.name = cha.Name;
            UICharacterMessage charInfo = go.GetComponent<UICharacterMessage>();
            charInfo.SetInfo(User.Instance.Info.Player.Characters[i]);

            Button button = go.GetComponentInChildren<Button>();
            int idx = i;
            button.onClick.AddListener(() => {
                OnClickSelectCharacter(idx);
            });

            uiChars.Add(go);
            go.SetActive(true);
        }
        // 如果有角色，自动选择第一个
        if (User.Instance.Info.Player.Characters.Count > 0)
        {
            OnClickSelectCharacter(0);
        }

    }


    //点击创建角色的按钮
    public void OnClickCreateCharater()
    {
        panelSelect.SetActive(false);
        panelCreate.SetActive(true);
        OnClickSelectClass(1);//默认选了第一个职业
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
                audioClipPlay.clip = characterAudioClip2[i];
                audioClipPlay.Play();
                Animator animator = characterClassPrefab[i].GetComponent<Animator>();
                animator.SetTrigger("SelectClass");
            }
            //播放动画
            
        }
        descs.text = DataManager.Instance.Characters[charClass.ToString()].Description;
    }

    //点击选择已经创建好的角色的按钮
    public void OnClickSelectCharacter(int index)
    {
        var character = User.Instance.Info.Player.Characters[index];
        User.Instance.CurrentCharacter = character;
        // 获得一个索引值来匹配当前选中的角色
        int classIndex = GetClassIndex(character.Class);

        // 控制角色按钮高亮
        for (int i = 0; i < User.Instance.Info.Player.Characters.Count; i++)
        {
            UICharacterMessage uICharacterMessage = uiChars[i].GetComponent<UICharacterMessage>();
            uICharacterMessage.selected = (index == i);
        }

        //控制角色3D模型显示
        for (int i = 0; i < classCount; i++)
        {
            characterClassPrefab[i].SetActive(i == classIndex);

            imageBackGround[i].gameObject.SetActive(i == classIndex);
            if (i == classIndex)
            {
                Animator animator = characterClassPrefab[i].GetComponent<Animator>();
                animator.SetTrigger("Click");
                audioClipPlay.clip = characterAudioClip1[i];
                audioClipPlay.Play();
            }      
        }
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

    //点击创建角色的按钮  创建完角色  进入游戏
    public void OnClickCreateCharacterSuccess()
    {
        if (string.IsNullOrEmpty(nameInputField.text))
        {
            MessageBox.Show("请输入角色昵称");
            return;
        }
        UserService.Instance.SendCharacterCreate(this.nameInputField.text, this.charClass);

    }

    //直接点击开始游戏的按钮
    public void OnClickStartGame()
    {
        //如果选了某个角色 可以进入主城 否则请选择角色
        if(User.Instance.CurrentCharacter == null)
        {
            MessageBox.Show("请选择角色");
            return;
        }
        else
        {
            MessageBox.Show("开始冒险");
        }
        UserService.Instance.SendGameEnter(User.Instance.CurrentCharacter.Id);
    }

    //绑定的事件
    void OnCharacterCreate(Result result, string message)
    {
        if (result == Result.Success)
        {
            // 清空昵称输入框
            nameInputField.text = "";
            MessageBox.Show("创建成功！");
            InitCharacterSelect(true);
        }
        else
        {
            MessageBox.Show(message, "错误", MessageBoxType.Error);
        }
    }
}
