using Managers;
using Models;
using Services;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Common.Data;

public class UISystemConfig : UIWindow {

    [Header("Music")]
    public Image BackGroundMusic;
    public Slider SliderMusic;
    public Toggle ToggleMusic;
    public Text MusicText;

    [Header("UI")]
    public Image BackGroundUI;
    public Slider SliderUI;
    public Toggle ToggleUI;
    public Text UIText;

    [Header("Character")]
    public Image BackGroundCharacter;
    public Slider SliderCharacter;
    public Toggle ToggleCharacter;
    public Text CharacterText;

    private void Start()
    {
        //从配置里读取音量
        ToggleMusic.isOn = Config.MusicOn;
        SliderMusic.value = Config.MusicVolume;
        MusicText.text = (Config.MusicVolume+"%").ToString();

        ToggleUI.isOn = Config.UIOn;
        SliderUI.value = Config.UIVolume;
        UIText.text = (Config.UIVolume+"%").ToString();

        ToggleCharacter.isOn = Config.CharacterOn;
        SliderCharacter.value = Config.CharacterVolume;
        CharacterText.text = (Config.CharacterVolume + "%").ToString();
    }

    public override void OnYesClick()
    {
        SoundManager.Instance.PlayUI(SoundDefine.Click);
        PlayerPrefs.Save();
        base.OnYesClick();
    }

    public void MusicToogle(bool on)//绑定Toogle组件控制
    {
        BackGroundMusic.enabled = !on;
        Config.MusicOn = on;
        SoundManager.Instance.PlayUI(SoundDefine.Click);
    }

    public void UIToogle(bool on)
    {
        BackGroundUI.enabled = !on;
        Config.UIOn = on;
        SoundManager.Instance.PlayUI(SoundDefine.Click);
    }

    public void CharacterToogle(bool on)//绑定Toogle组件控制
    {
        BackGroundCharacter.enabled = !on;
        Config.CharacterOn = on;
        SoundManager.Instance.PlayUI(SoundDefine.Click);
    }

    public void MusicVolume(float volume)//绑定Slider组件控制
    {
        Config.MusicVolume = (int)volume;
        MusicText.text = (volume+"%").ToString();
        PlaySound();
    }

    public void UIVolume(float volume)
    {
        Config.UIVolume = (int)volume;
        UIText.text = (volume+"%").ToString();
        PlaySound();
    }

    public void CharacterVolume(float volume)
    {
        Config.CharacterVolume = (int)volume;
        CharacterText.text = (volume+"%").ToString();
        PlaySound();
    }

    float lastPlay = 0;
    private void PlaySound()
    {
        if (Time.realtimeSinceStartup - lastPlay > 0.1)
        {
            lastPlay = Time.realtimeSinceStartup;
            SoundManager.Instance.PlayUI(SoundDefine.Accept);
        }
    }

}
