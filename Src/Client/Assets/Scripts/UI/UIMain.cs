using Models;
using Services;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIMain : MonoSingleton<UIMain> {

    public Text myNameandLevel;
    public GameObject escPanel;//返回选择角色界面的按钮
    private GameObject go;//预制体局部变量
    public bool escPanelState = false;//开启状态 按ESC进行开启关闭

    protected override void OnStart ()
    {
        SoundManager.Instance.bgmaudioClipPlay.clip = SoundManager.Instance.bgmInMainCityClip;
        SoundManager.Instance.bgmaudioClipPlay.Play();
        Cursor.visible = false;
        UpdateAvatar();

        go = Instantiate(escPanel, this.transform);//实例化返回面板
        go.name = "ESC面板";

        Button backButton = go.transform.Find("返回角色选择按钮").GetComponent<Button>();
        Button returnButton = go.transform.Find("返回游戏按钮").GetComponent<Button>();
        Button quitButton = go.transform.Find("退出游戏按钮").GetComponent<Button>();

        backButton.onClick.AddListener(OnClickBackToChooseCharacter);
        returnButton.onClick.AddListener(OnClickReturnGame);
        quitButton.onClick.AddListener(OnClickQuitGame);
    }
	
	void Update ()
    {
        if (UnityEngine.SceneManagement.SceneManager.GetActiveScene().name == "CharacterChoose") return;
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            EscPanel();
        }

        //面板关闭情况下才允许点击 鼠标光标隐藏
        if (Input.GetMouseButtonDown(0) && !escPanelState)
        {
            Cursor.visible = false; //隐藏光标
        }

        //Alt解锁鼠标
        if (Input.GetKeyDown(KeyCode.LeftAlt))
        {
            Cursor.visible = true; 
        }
    }

    void UpdateAvatar()
    {
        myNameandLevel.text = User.Instance.CurrentCharacter.Name +"  "+ User.Instance.CurrentCharacter.Level.ToString()+"级";
    }

    public void EscPanel()
    {
        escPanelState = !escPanelState;
        if (escPanelState == true)
        {
            if (SoundManager.Instance.audioClipPlay.clip!=null&& SoundManager.Instance.audioClipPlay.isPlaying)
            {
                SoundManager.Instance.audioClipPlay.Stop();
            }
            Cursor.visible = true;
        }
        else
        {
            Cursor.visible = false;
            if(SoundManager.Instance.audioClipPlay.clip != null)
            {
                SoundManager.Instance.audioClipPlay.Play();
            }
        }
        go.SetActive(escPanelState);
    }

    //返回选择角色的页面
    public void OnClickBackToChooseCharacter()
    {
        escPanelState = !escPanelState;
        go.SetActive(escPanelState);
        StopMainCityMusic(); 
        SceneManager.Instance.LoadScene("CharacterChoose");
        Services.UserService.Instance.SendGameLeave();
    }

    //返回游戏
    public void OnClickReturnGame()
    {
        EscPanel();
    }

    //退出游戏
    public void OnClickQuitGame()
    {
        UIMessageBox msgBox = MessageBox.Show("确认要退出游戏吗？", "退出游戏", MessageBoxType.Confirm, "确认", "取消");
        msgBox.OnYes = () =>
        {
            StopMainCityMusic();
            Application.Quit();
        };
    }

    private void StopMainCityMusic()
    {
        if (SoundManager.Instance.bgmaudioClipPlay.isPlaying)
        {
            SoundManager.Instance.bgmaudioClipPlay.Stop();  // 停止背景音乐
        }
    }
}
