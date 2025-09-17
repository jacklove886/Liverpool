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

    public AudioSource musicAudioSource;

    public AudioSource uiAudioSource;

    public AudioSource characterAudioSource;

    public AudioSource characterJumpAudioSource;

    const string MusicPath = "Music/";
    const string UIPath = "Sound/UI/";
    const string CharacterPath = "Sound/Character/";

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

    private int musicVolume;
    public int MusicVolume
    {
        get { return musicVolume; }
        set
        {
            if (musicVolume != value)
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

    private void Start()
    {
        MusicVolume = Config.MusicVolume;
        UIVolume = Config.UIVolume;
        MusicOn = Config.MusicOn;
        UIOn = Config.UIOn;
    }

    private void MusicMute(bool mute)
    {
        SetVolume("MusicVolume", mute ? 0 : musicVolume);
    }

    private void SoundMute(bool mute)
    {
        SetVolume("UIVolume", mute ? 0 : uiVolume);
    }

    private void SetVolume(string name, int value)
    {
        float volume = value * 0.5f - 50f;
        this.audioMixer.SetFloat(name, volume);
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
        AudioClip clip = Resloader.Load<AudioClip>(MusicPath + name);
        if (clip == null)
        {
            Debug.LogWarningFormat("音乐:{0}不存在", name);
            return;
        }
        uiAudioSource.PlayOneShot(clip);
    }

    public void PlayClipOnAudioSource(AudioSource audioSource,AudioClip clip,bool isloop)
    {

    }
}
