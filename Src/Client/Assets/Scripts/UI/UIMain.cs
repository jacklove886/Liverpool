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
    public Image characterImage;

    protected override void OnStart ()
    {
        UpdateAvatar();
        characterImage.overrideSprite = SpriteManager.Instance.characterIcons[(int)User.Instance.CurrentCharacter.Class-1];
    }
	

    void UpdateAvatar()
    {
        myNameandLevel.text = User.Instance.CurrentCharacter.Name +"  "+ User.Instance.CurrentCharacter.Level.ToString()+"级";
    }
      
    public void OnClickBag()
    {
        SoundManager.Instance.PlayUI(SoundDefine.Click);
        UIManager.Instance.Show<UIBag>();//背包
    }

    public void OnClickCharEquip()
    {
        SoundManager.Instance.PlayUI(SoundDefine.Click);
        UIManager.Instance.Show<UICharEquip>();//商店
    }

    public void OnClickQuest()
    {
        SoundManager.Instance.PlayUI(SoundDefine.Click);
        UIManager.Instance.Show<UIQuestSystem>();//任务
    }

    public void OnClickFriend()
    {
        SoundManager.Instance.PlayUI(SoundDefine.Click);
        UIManager.Instance.Show<UIFriend>();//好友
    }

    public void ShowTeamUI(bool show)
    {
        SoundManager.Instance.PlayUI(SoundDefine.Click);
        UITeam.ShowTeam(show);//组队
    }

    public void OnClickGuild(bool show)//公会
    {
        SoundManager.Instance.PlayUI(SoundDefine.Click);
        GuildManager.Instance.ShowGuild();
    }

    public void OnClickRide()//坐骑
    {
        SoundManager.Instance.PlayUI(SoundDefine.Click);
        UIManager.Instance.Show<UIRide>();
    }

    public void OnClickSetting()//设置
    {
        SoundManager.Instance.PlayUI(SoundDefine.Click);
        UIManager.Instance.Show<UISetting>();
    }

    public void OnClickSkill()//技能
    {

    }
}
