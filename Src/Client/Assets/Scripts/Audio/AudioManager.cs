using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoSingleton<AudioManager>
{

    [Header("音效播放器")]
    public AudioSource audioClipPlay;//总的声音播放器
    public AudioSource bgmaudioClipPlay;//背景音乐播放器
    public AudioSource jumpaudioClipPlay;//角色跳跃声音

    [Header("打开游戏音效")]
    public AudioClip openGameClip;

    [Header("打开角色创建音效")]
    public AudioClip openChooseCharacterClip;

    [Header("主城背景音乐")]
    public AudioClip bgmInMainCityClip;

    [Header("角色创建音效")]
    public AudioClip[] characterAudioClip1;                      

    [Header("角色选择音效")]
    public AudioClip[] characterAudioClip2;                     

    [Header("角色跳跃音效")]
    public AudioClip[] jumpAudioClip;

    [Header("角色走路音效")]
    public AudioClip[] walkAudioClip;

    [Header("角色跑步音效")]
    public AudioClip[] runAudioClip;

    protected void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }

    void Start () {
		
	}
	
	
	void Update () {
		
	}

}
