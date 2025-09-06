using Services;// 服务器命名空间
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using SkillBridge.Message;
using Managers;

public class UILogin : MonoBehaviour {
    public InputField username;
    public InputField password;
    public Button buttonJizhuAccount;
    public Button buttonYuedu;
    public Button buttonRegister;
    public Button buttonLogin;

    private void Start()
    {
        # region  如果启动了LoadingManager脚本 就不需要这些话 如果没启动 就要加上这些话来加载数据库
        MapService.Instance.Init();//初始化服务器
        UserService.Instance.Init();//初始化服务器
        DataManager.Instance.Load();//加载数据库
        NpcTestManager.Instance.Init();//初始化NPC管理器
        #endregion

        UserService.Instance.OnLogin += OnLogin;
        SoundManager.Instance.audioClipPlay.PlayOneShot(SoundManager.Instance.openGameClip);
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

    public void OnClickDialog()
    {
        UIDialog uIDialog = UIManager.Instance.Show<UIDialog>();//利用返回值获得组件
        uIDialog.title.text="Siu";//调用方法
        uIDialog.OnClose += UIDialog_OnClose;//订阅UIWindow的事件
    }

    //点击确认或者关闭后 调用UIDialog_OnClose方法
    private void UIDialog_OnClose(UIWindow sender, UIWindow.WindowResult result)
    {
        //as作用是将UIWindow类型的sender安全转换为UIDialog类型 如果失败就返回null 不会抛异常
        string DialogName = (sender as UIDialog).name;//获取的是GameObject的name
        //第一个是消息框,第二个是标题,第三个是指定MessageBoxType的类型是信息框(只显示一个确定按钮)
        MessageBox.Show("我们阿森纳是不可战胜的"+result, DialogName, MessageBoxType.Information);
    }
}
