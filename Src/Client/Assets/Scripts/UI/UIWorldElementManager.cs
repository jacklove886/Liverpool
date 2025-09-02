using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIWorldElementManager : MonoBehaviour {

    public GameObject escPanel;//返回选择角色界面的按钮
    public bool escPanelState=false;//开启状态 按ESC进行开启关闭
    public bool mouseState = false;  //鼠标状态  false为锁定 true为None


    void Start ()
    {
		
	}
	
	
	void Update ()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            EscPanel();
        }
        if (!mouseState)
        {
            if (Input.GetMouseButtonDown(0) && escPanelState == false)
            {
                Cursor.lockState = CursorLockMode.Locked;
                mouseState = false;
            }
        }
        
    }

    public void EscPanel()
    {
        escPanelState = (escPanelState == true) ? false : true;
        if (escPanelState ==true) { Cursor.lockState = CursorLockMode.None; mouseState = true; }
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
