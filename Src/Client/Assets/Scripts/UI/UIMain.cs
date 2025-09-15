using Managers;
using Models;
using Services;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIMain : MonoSingleton<UIMain> {

    public Text myNameandLevel;
    public UITeam UITeam;

    protected override void OnStart ()
    {
        SoundManager.Instance.bgmaudioClipPlay.clip = SoundManager.Instance.bgmInMainCityClip;
        SoundManager.Instance.bgmaudioClipPlay.Play();
        UpdateAvatar();

        SoundManager.Instance.uiClipPlay.clip = null;
    }
	
	void Update ()
    {
        if (UnityEngine.SceneManagement.SceneManager.GetActiveScene().name == "CharacterChoose") return;
    }

    void UpdateAvatar()
    {
        myNameandLevel.text = User.Instance.CurrentCharacter.Name +"  "+ User.Instance.CurrentCharacter.Level.ToString()+"级";
    }
      
    public void OnClickBag()
    {
        UIManager.Instance.Show<UIBag>();//背包
    }

    public void OnClickCharEquip()
    {
        UIManager.Instance.Show<UICharEquip>();//商店
    }

    public void OnClickQuest()
    {
        UIManager.Instance.Show<UIQuestSystem>();//任务
    }

    public void OnClickFriend()
    {
        UIManager.Instance.Show<UIFriend>();//好友
    }

    public void ShowTeamUI(bool show)
    {
        UITeam.ShowTeam(show);//组队
    }

    public void OnClickGuild(bool show)//公会
    {
        GuildManager.Instance.ShowGuild();
    }

    public void OnClickRide()//坐骑
    {
        
    }

    public void OnClickSetting()//设置
    {
        UISetting ui= UIManager.Instance.Show<UISetting>();
        ui.transform.SetParent(this.transform,false);
    }

    public void OnClickSkill()//技能
    {

    }
}
