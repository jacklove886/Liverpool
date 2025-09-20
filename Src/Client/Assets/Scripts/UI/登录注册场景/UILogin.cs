
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

        log4net.Config.XmlConfigurator.ConfigureAndWatch(new System.IO.FileInfo("log4net.xml"));
        UnityLogger.Init();

        MapService.Instance.Init();//初始化服务器

        UserService.Instance.Init();//初始化服务器

        DataManager.Instance.Load();//同步加载(测试用)  LoadingManager用的是携程分帧加载

        ShopManager.Instance.Init();//初始化商店管理器

        NpcTestManager.Instance.Init();//初始化商店管理器

        StatusService.Instance.Init();

        FriendService.Instance.Init();

        TeamService.Instance.Init();

        GuildService.Instance.Init();

        ChatService.Instance.Init();

        SoundManager.Instance.PlayMusic(SoundDefine.Music_Login);
        #endregion

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
        SoundManager.Instance.PlayUI(SoundDefine.Click);
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
            SoundManager.Instance.PlayMusic(SoundDefine.Music_Select);
            UnityEngine.SceneManagement.SceneManager.LoadScene("CharacterChoose");
        }
        else
        {
            // 登录失败，显示错误信息
            MessageBox.Show(msg);
        }
    }
}
