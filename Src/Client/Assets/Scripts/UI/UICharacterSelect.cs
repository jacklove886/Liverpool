using Models;
using SkillBridge.Message;
using Services;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class UICharacterSelect : MonoBehaviour
{
    public SkillBridge.Message.NCharacterInfo info;
    CharacterClass charClass;//枚举值 获得角色的int值
    public Transform charTransform;//角色信息栏的位置
    [Header("选择/创建面板")]
    public GameObject panelSelect;//角色选择面板
    public GameObject panelCreate;//角色创建面板
    [Header("生成信息的父物体")]
    public Transform uiCharList;//生成信息的父物体
    [Header("滚动条角色信息面板的预制体")]
    public GameObject uiCharacterMessage;//滚动条角色信息面板的预制体
    [Header("创建面板滚动条中角色当前索引")]
    public List<GameObject> uiChars = new List<GameObject>();//滚动条中角色当前的索引   
    [Header("职业介绍")]
    public Text descs;//职业介绍
    [Header("创建界面填写昵称的框")]
    public InputField nameInputField;//创建界面填写昵称的框
    [Header("添加角色的按钮")]
    public  Button creatCharacter;//添加角色的按钮
    [Header("选择角色类型的按钮按钮")]
    public Button[] selectClass;//选择角色类型的按钮
    [Header("已经创建好的角色的按钮")]
    public Button selectCharacter; //已经创建好的角色的按钮
    [Header("最上面的角色图标")]
    public Image[] titles;//最上面的角色图标
    [Header("职业图标")]
    public Image[] confirmPicture;//职业图标
    [Header("角色背景图")]
    public Image[] imageBackGround;//角色背景图
    public UICharacterView uICharacterView;//实例化UICharacterView类
    private int selectCharacterIdx = -1;


    private void Awake()
    {
        UserService.Instance.OnCharacterCreate += OnCharacterCreate;
    }
    // Use this for initialization
    void Start ()
    {
        DataManager.Instance.Load();//等以后可以卸载LoadManager里
        InitCharacterSelect(true);
        charTransform.position = new Vector3(-15, 55, 0);
    }

    private void OnDestroy()
    {
        //UserService.Instance.OnCharacterCreate -= OnCharacterCreate;
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
        // 添加空值检查
        if (User.Instance == null ||
        User.Instance.Info == null ||
        User.Instance.Info.Player == null ||
        User.Instance.Info.Player.Characters == null)
        {
            Debug.LogWarning("角色列表为空，无法初始化角色选择界面");
            return;
        }
        print("当前创建角色数量为"+User.Instance.Info.Player.Characters.Count);

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
    }

    // Update is called once per frame
    void Update ()
    {


     }

    //点击创建角色的按钮
    public void OnClickCreateCharacer()
    {
        panelSelect.SetActive(false);
        panelCreate.SetActive(true);
        OnClickSelectClass(1);//默认选了第一个职业
    }


    //点击选择职业类型的按钮
    public void OnClickSelectClass(int charClass)
    {
        this.charClass = (CharacterClass)charClass;
        uICharacterView.UpdateCharacter(new SkillBridge.Message.NCharacterInfo
        {
            Class = (CharacterClass)charClass
        });
        for (int i = 0; i < 3; i++)
        {
            //标题改为相应的职业
            titles[i].gameObject.SetActive(i == charClass - 1);
            //按钮改为相应的职业
            confirmPicture[i].gameObject.SetActive(i == charClass - 1);
            //背景图片改为相应的职业
            imageBackGround[i].gameObject.SetActive(i == charClass - 1);
        }
        descs.text = DataManager.Instance.Characters[charClass.ToString()].Description;
    }

    //点击选择已经创建好的角色的按钮
    public void OnClickSelectCharacter(int idx)
    {
        selectCharacterIdx = idx;
        var cha = User.Instance.Info.Player.Characters[idx];
        User.Instance.CurrentCharacter = cha;

        var list = User.Instance.Info.Player.Characters;
        selectCharacterIdx = idx;
        cha = list[idx];
        Debug.LogFormat("Select Char:[{0}] {1} [{2}]", cha.Id, cha.Name, cha.Class);
        User.Instance.CurrentCharacter = cha;
        uICharacterView.UpdateCharacter(cha);


        for (int i = 0; i < User.Instance.Info.Player.Characters.Count; i++)
        {
            UICharacterMessage uICharacterMessage = uiChars[i].GetComponent<UICharacterMessage>();
            uICharacterMessage.selected=idx==i;
        }
    }

    //点击创建角色的按钮  创建完角色  进入游戏
    public void OnClickStartGame()
    {
        if (string.IsNullOrEmpty(nameInputField.text))
        {
            MessageBox.Show("请输入角色昵称");
            return;
        }
        UserService.Instance.SendCharacterCreate(this.nameInputField.text, this.charClass);

    }

    //直接点击开始游戏的按钮
    public void OnDirectClickStartGame()
    {
        //如果选了某个角色 可以进入主城 否则请选择角色
        MessageBox.Show("进入主城");
    }
    //绑定的事件
    void OnCharacterCreate(Result result, string message)
    {
        if (result == Result.Success)
        {
            MessageBox.Show("进入主城");
            InitCharacterSelect(true);
        }
        else
        {
            MessageBox.Show(message, "错误", MessageBoxType.Error);
        }
    }
}
