using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIWorldElementManager : MonoBehaviour {

    public GameObject escPanel;//返回选择角色界面的按钮
    public bool escPanelState=false;//开启状态 按ESC进行开启关闭
    public bool mouseState;  //鼠标状态  false为锁定 true为None


    void Start ()
    {
        Cursor.lockState = CursorLockMode.Locked;  // 游戏开始时锁定鼠标
        mouseState = false; 
    }
	
	
	void Update ()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            EscPanel();
        }

        //面板关闭情况下才允许点击 改变鼠标变成锁定
        if (Input.GetMouseButtonDown(0) && !escPanelState&&mouseState)
        {
            Cursor.lockState = CursorLockMode.Locked;
            mouseState = false;
        }
        
    }

    public void EscPanel()
    {
        escPanelState = !escPanelState;
        mouseState = true;
        if (escPanelState ==true)
        {
            Cursor.lockState = CursorLockMode.None;
        }

        escPanel.SetActive(escPanelState);
    }

    public void OnClickBackToChooseCharacter()
    {
        //返回选择角色的页面
        SceneManager.Instance.LoadScene("CharacterChoose");
    }

    public void OnClickQuitGame()
    {
        //退出游戏
        Application.Quit();
    }
}
