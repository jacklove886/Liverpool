using Models;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIMainCity : MonoBehaviour {

    public Text myNameandLevel;
    public GameObject escPanel;//返回选择角色界面的按钮
    public bool escPanelState = false;//开启状态 按ESC进行开启关闭

    void Start ()
    {
        Time.timeScale = 1;
        AudioManager.Instance.bgmaudioClipPlay.clip = AudioManager.Instance.bgmInMainCityClip;
        AudioManager.Instance.bgmaudioClipPlay.Play();
        Cursor.visible = false; 
        UpdataAvatar();
	}
	
	void Update ()
    {

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

    void UpdataAvatar()
    {
        myNameandLevel.text = User.Instance.CurrentCharacter.Name +"  "+ User.Instance.CurrentCharacter.Level.ToString()+"级";
    }

    public void EscPanel()
    {
        escPanelState = !escPanelState;
        if (escPanelState == true)
        {
            Time.timeScale = 0;
            Cursor.visible = true;
        }
        else
        {
            Cursor.visible = false;
            Time.timeScale = 1;
        }

        escPanel.SetActive(escPanelState);
    }

    public void OnClickBackToChooseCharacter()
    {
        StopMainCityMusic();
        //返回选择角色的页面
        SceneManager.Instance.LoadScene("CharacterChoose");
        Services.UserService.Instance.SendGameLeave();
    }

    public void OnClickReturnGame()
    {
        EscPanel();
    }
    public void OnClickQuitGame()
    {
        StopMainCityMusic();
        Services.UserService.Instance.SendGameLeave();//如果出bug 把这一行注释掉
        //退出游戏
        Application.Quit();
    }

    private void StopMainCityMusic()
    {
        if (AudioManager.Instance.bgmaudioClipPlay.isPlaying)
        {
            AudioManager.Instance.bgmaudioClipPlay.Stop();  // 停止背景音乐
        }
    }
}
