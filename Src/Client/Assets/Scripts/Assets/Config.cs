using Managers;
using Models;
using Services;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Config//音乐存储
{
    public static bool MusicOn
    {
        get
        {
            return PlayerPrefs.GetInt("Music", 1) == 1;//读取配置 找不到默认为1
        }
        set
        {
            PlayerPrefs.SetInt("Music", value ? 1 : 0);//设置开关
            SoundManager.Instance.MusicOn = value;
        }
    }

    public static bool UIOn
    {
        get
        {
            return PlayerPrefs.GetInt("UI", 1) == 1;
        }
        set
        {
            PlayerPrefs.SetInt("UI", value ? 1 : 0);
            SoundManager.Instance.UIOn = value;
        }
    }

    public static bool CharacterOn
    {
        get
        {
            return PlayerPrefs.GetInt("Character", 1) == 1;
        }
        set
        {
            PlayerPrefs.SetInt("Character", value ? 1 : 0);
            SoundManager.Instance.CharacterOn = value;
        }
    }

    public static int MusicVolume
    {
        get { return PlayerPrefs.GetInt("MusicVolume", 100); }
        set
        {
            PlayerPrefs.SetInt("MusicVolume", value);
            SoundManager.Instance.MusicVolume = value;
        }
    }

    public static int UIVolume
    {
        get { return PlayerPrefs.GetInt("UIVolume", 100); }
        set
        {
            PlayerPrefs.SetInt("UIVolume", value);
            SoundManager.Instance.UIVolume = value;
        }
    }

    public static int CharacterVolume
    {
        get { return PlayerPrefs.GetInt("CharacterVolume", 100); }
        set
        {
            PlayerPrefs.SetInt("CharacterVolume", value);
            SoundManager.Instance.CharacterVolume = value;
        }
    }

    ~Config()
    {
        PlayerPrefs.Save();
    }

}
