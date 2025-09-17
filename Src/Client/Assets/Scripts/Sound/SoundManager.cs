using Managers;
using Models;
using Services;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Common.Data;
using UnityEngine.Audio;
using System;

public class SoundManager : MonoSingleton<SoundManager>
{
    public AudioMixer audioMixer;

    [Header("背景音乐")]
    public AudioSource musicAudioSource;

    [Header("UI音效")]
    public AudioSource uiAudioSource;

    [Header("角色持续音效")]
    public AudioSource characterAudioSource;

    [Header("角色短暂音效")]
    public AudioSource characterOneShotAudioSource;

    const string MusicPath = "Music/";
    const string UIPath = "Sound/UI/";
    const string CharacterPath = "Sound/Character/";
    const string CharacterOneShotPath = "Sound/CharacterOneShot/";

    private bool musicOn;
    public bool MusicOn
    {
        get { return musicOn; }
        set
        {
            musicOn = value;
            MusicMute(!musicOn);//如果关了  设置静音
        }
    }

    private bool uiOn;
    public bool UIOn
    {
        get { return uiOn; }
        set
        {
            uiOn = value;
            SoundMute(!uiOn);//如果关了  设置静音
        }
    }

    private bool characterOn;
    public bool CharacterOn
    {
        get { return characterOn; }
        set
        {
            characterOn = value;
            CharacterMute(!characterOn);//如果关了  设置静音
        }
    }

    private int musicVolume;
    public int MusicVolume
    {
        get { return musicVolume; }
        set
        {
            if (musicVolume != value)//如果声音不一样
            {
                musicVolume = value;
                if (musicOn) this.SetVolume("MusicVolume", musicVolume);
            }
        }
    }

    private int uiVolume;
    public int UIVolume
    {
        get { return uiVolume; }
        set
        {
            if (uiVolume != value)
            {
                uiVolume = value;
                if (uiOn) this.SetVolume("UIVolume", uiVolume);
            }
        }
    }

    private int characterVolume;
    public int CharacterVolume
    {
        get { return characterVolume; }
        set
        {
            if (characterVolume != value)
            {
                characterVolume = value;
                if (characterOn) this.SetVolume("CharacterVolume", characterVolume);
            }
        }
    }

    private void Start()
    {
        MusicVolume = Config.MusicVolume;
        UIVolume = Config.UIVolume;
        CharacterVolume= Config.CharacterVolume;

        MusicOn = Config.MusicOn;
        UIOn = Config.UIOn;
        CharacterOn = Config.CharacterOn;
    }

    private void MusicMute(bool mute)
    {
        SetVolume("MusicVolume", mute ? 0 : musicVolume);
    }

    private void SoundMute(bool mute)
    {
        SetVolume("UIVolume", mute ? 0 : uiVolume);
    }

    private void CharacterMute(bool mute)
    {
        SetVolume("CharacterVolume", mute ? 0 : characterVolume);
    }

    private void SetVolume(string name, int value)
    {
        float volume = value * 0.5f - 50f;
        this.audioMixer.SetFloat(name, volume);//关键！！！设置混音器的声音大小
    }

    public void PlayMusic(string name)
    {
        AudioClip clip = Resloader.Load<AudioClip>(MusicPath + name);
        if (clip == null)
        {
            Debug.LogWarningFormat("音乐:{0}不存在", name);
            return;
        }
        if (musicAudioSource.isPlaying)//如果在播放 暂停
        {
            musicAudioSource.Stop();
        }
        musicAudioSource.clip = clip;
        musicAudioSource.Play();
    }

    public void PlayUI(string name)
    {
        AudioClip clip = Resloader.Load<AudioClip>(UIPath + name);
        if (clip == null)
        {
            Debug.LogWarningFormat("音乐:{0}不存在", name);
            return;
        }
        uiAudioSource.PlayOneShot(clip);
    }

    public void PlayCharacter(string name)
    {
        AudioClip clip = Resloader.Load<AudioClip>(CharacterPath + name);
        if (clip == null)
        {
            Debug.LogWarningFormat("音乐:{0}不存在", name);
            return;
        }
        characterAudioSource.clip = clip;
        characterAudioSource.Play();
    }

    public void StopCharacter()
    {
        if (characterAudioSource.clip != null)
        {
            characterAudioSource.clip = null;
        }
    }

    public void PlayCharacterClipOnAudioSource(string name)
    {
        AudioClip clip = Resloader.Load<AudioClip>(CharacterOneShotPath + name);
        if (clip == null)
        {
            Debug.LogWarningFormat("音乐:{0}不存在", name);
            return;
        }
        characterOneShotAudioSource.PlayOneShot(clip);
    }
}
