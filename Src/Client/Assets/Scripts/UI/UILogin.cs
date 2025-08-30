using Services;// 服务器命名空间
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using SkillBridge.Message;

public class UILogin : MonoBehaviour {
    public InputField username;
    public InputField password;
    public Button buttonJizhuAccount;
    public Button buttonYuedu;
    public Button buttonRegister;
    public Button buttonLogin;

    // Use this for initialization
    private void Start()
    {
        DataManager.Instance.Load();//如果启动了LoadingManager脚本 就不需要这句话 如果没启动 就要加上这句话来加载数据库
        UserService.Instance.OnLogin += OnLogin;
        string savedUsername = PlayerPrefs.GetString("SavedUsername", "");
        if (!string.IsNullOrEmpty(savedUsername))
        {
            username.text = savedUsername;
        }

    }
    private void OnDestroy()
    {
        UserService.Instance.OnLogin -= OnLogin;
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    public void OnClickLogin()
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
        else if (buttonYuedu.gameObject.activeInHierarchy != true)
        {
            MessageBox.Show("请勾选用户协议");
            return;
        }
        UserService.Instance.SendLogin(username.text, password.text);
    }
    // 登录结果回调
    void OnLogin(Result result, string msg)
    {
        if (result == Result.Success)
        {
            // 保存账号到本地
            if (buttonJizhuAccount.gameObject.activeInHierarchy)
            {
                PlayerPrefs.SetString("SavedUsername", username.text);
                PlayerPrefs.Save();
            }
            else
            {
                // 没勾选记住账号就把之前保存的删掉
                PlayerPrefs.DeleteKey("SavedUsername");
            }
            PlayerPrefs.Save();

            // 登录成功，跳转到选择角色场景
            UnityEngine.SceneManagement.SceneManager.LoadScene("CharacterChoose");
        }
        else
        {
            // 登录失败，显示错误信息
            MessageBox.Show(msg);
        }
    }
}
