using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UISetting : UIWindow {

    //返回选择角色的页面
    public void OnClickBackToChooseCharacter()
    {
        StopMainCityMusic();
        SceneManager.Instance.LoadScene("CharacterChoose");
        Services.UserService.Instance.SendGameLeave();
        Close();
    }

    //退出游戏
    public void OnClickQuitGame()
    {
        UIMessageBox msgBox = MessageBox.Show("确认要退出游戏吗？", "退出游戏", MessageBoxType.Confirm, "确认", "取消");
        msgBox.OnYes = () =>
        {
            StopMainCityMusic();
            //退出游戏的功能
            Services.UserService.Instance.SendGameLeave(true);
        };
    }

    private void StopMainCityMusic()
    {
        if (SoundManager.Instance.bgmaudioClipPlay.isPlaying)
        {
            SoundManager.Instance.bgmaudioClipPlay.Stop();  // 停止背景音乐
        }
        Close();
    }
}
