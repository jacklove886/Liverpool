using Managers;
using Models;
using Services;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Config
{
    public static bool MusicOn
    {
        get
        {
            return PlayerPrefs.GetInt("Music", 1) == 1;
        }
        set
        {
            PlayerPrefs.SetInt("Music", value ? 1 : 0);
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

    ~Config()
    {
        PlayerPrefs.Save();
    }

}
