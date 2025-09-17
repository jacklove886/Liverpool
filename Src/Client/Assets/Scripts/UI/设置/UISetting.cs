using Common.Data;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UISetting : UIWindow {

    //返回选择角色的页面
    public void OnClickBackToChooseCharacter()
    {
        SceneManager.Instance.LoadScene("CharacterChoose");
        SoundManager.Instance.PlayMusic(SoundDefine.Music_Select);
        Services.UserService.Instance.SendGameLeave();
        Close();
    }

    //退出游戏
    public void OnClickQuitGame()
    {
        UIMessageBox msgBox = MessageBox.Show("确认要退出游戏吗？", "退出游戏", MessageBoxType.Confirm, "确认", "取消");
        msgBox.OnYes = () =>
        {
            //退出游戏的功能
            Services.UserService.Instance.SendGameLeave(true);
        };
    }

    //退出游戏
    public void OnClickSystemConfig()
    {
        //退出游戏的功能
        UIManager.Instance.Show<UISystemConfig>();
        Close();
    }

}
