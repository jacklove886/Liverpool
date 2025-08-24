using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIRegister : MonoBehaviour {
    public InputField username;
    public InputField password;
    public InputField confirmpassword;
    public Button buttonRegister;

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void OnClickRegister()
    {
        print("已点击注册");
        print("账号是" + username.text);
        print("密码是" + password.text);
        print("确认密码是" + confirmpassword.text);
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
        else
        {
            MessageBox.Show("注册成功!");
            return;
        }
    }
}
