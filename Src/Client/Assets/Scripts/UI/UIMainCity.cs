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
        AudioManager.Instance.bgmaudioClipPlay.clip = AudioManager.Instance.bgmInMainCityClip;
        AudioManager.Instance.bgmaudioClipPlay.Play();
        Cursor.lockState = CursorLockMode.Locked;  // 游戏开始时锁定鼠标
        UpdataAvatar();
	}
	
	void Update ()
    {

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            EscPanel();
        }

        //面板关闭情况下才允许点击 改变鼠标变成锁定
        if (Input.GetMouseButtonDown(0) && !escPanelState)
        {
            Cursor.lockState = CursorLockMode.Locked;
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
            Cursor.lockState = CursorLockMode.None;
        }

        escPanel.SetActive(escPanelState);
    }

    public void OnClickBackToChooseCharacter()
    {
        StopMainCityMusic();
        //返回选择角色的页面
        SceneManager.Instance.LoadScene("CharacterChoose");
    }

    public void OnClickReturnGame()
    {
        EscPanel();
    }
    public void OnClickQuitGame()
    {
        StopMainCityMusic();
        //退出游戏
        Application.Quit();
    }

    private void StopMainCityMusic()
    {
        if (AudioManager.Instance != null &&
            AudioManager.Instance.bgmaudioClipPlay != null &&
            AudioManager.Instance.bgmaudioClipPlay.isPlaying)
        {
            AudioManager.Instance.bgmaudioClipPlay.Stop();  // 停止背景音乐
        }
    }
}
