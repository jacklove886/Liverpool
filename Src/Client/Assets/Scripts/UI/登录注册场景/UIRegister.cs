using Services;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIRegister : MonoBehaviour {
    public InputField username;
    public InputField password;
    public InputField confirmpassword;
    public Button buttonYuedu;
    public Button buttonRegister;

    // Use this for initialization
    private void Start () {
        UserService.Instance.OnRegister += OnRegister;

	}
    private void OnDestroy()
    {
        UserService.Instance.OnRegister -= OnRegister;
    }
    void OnRegister(SkillBridge.Message.Result result,string msg)
    {
        MessageBox.Show("注册成功");
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    public void OnClickRegister()
    {
        if (string.IsNullOrEmpty(username.text))
        {
            MessageBox.Show("请输入账号");
            return;
        }
        else if (string.IsNullOrEmpty(password.text))
        {
            MessageBox.Show("请输入密码");
            return;
        }
        else if (string.IsNullOrEmpty(confirmpassword.text))
        {
            MessageBox.Show("请输入确认密码");
            return;
        }
        else if (password.text != confirmpassword.text)
        {
            MessageBox.Show("两次输入的密码不一致");
            return;
        }
        else if (buttonYuedu.gameObject.activeInHierarchy!=true)
        {
            MessageBox.Show("请勾选用户协议");
            return;
        }
        UserService.Instance.SendRegister(username.text, password.text);
    }
}
