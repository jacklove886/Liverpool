using Managers;
using Models;
using Services;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Common.Data;

public class UISystemConfig : UIWindow {

    public Image BackGroundMusic;
    public Image BackGroundSound;

    public Slider SliderMusic;
    public Slider SliderSound;

    public Toggle ToggleMusic;
    public Toggle ToggleSound;

    private void Start()
    {
        ToggleMusic.isOn = Config.MusicOn;
        ToggleSound.isOn = Config.MusicOn;
        SliderMusic.value = Config.MusicVolume;
        SliderSound.value = Config.UIVolume;
    }

    public override void OnYesClick()
    {
        SoundManager.Instance.PlayUI(SoundDefine.Click);
        PlayerPrefs.Save();
        base.OnYesClick();
    }

    public void MusicToogle(bool on)
    {
        BackGroundMusic.enabled = !on;
        Config.MusicOn = on;
        SoundManager.Instance.PlayUI(SoundDefine.Click);
    }

    public void SoundToogle(bool on)
    {
        BackGroundSound.enabled = !on;
        Config.UIOn = on;
        SoundManager.Instance.PlayUI(SoundDefine.Click);
    }

    public void MusicVolume(float volume)
    {
        Config.MusicVolume = (int)volume;
        PlaySound();
    }

    public void SoundVolume(float volume)
    {
        Config.UIVolume = (int)volume;
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
