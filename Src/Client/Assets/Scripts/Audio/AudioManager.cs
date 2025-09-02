using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoSingleton<AudioManager>
{

    [Header("音效播放器")]
    public AudioSource audioClipPlay;//总的声音播放器
    public AudioSource jumpaudioClipPlay;//专门的跳跃声音播放器

    [Header("打开游戏音效")]
    public AudioClip openGameClip;                             

    [Header("角色创建音效")]
    public AudioClip[] characterAudioClip1;                      

    [Header("角色选择音效")]
    public AudioClip[] characterAudioClip2;                     

    [Header("角色进入游戏音效")]
    public AudioClip[] characterAudioClip3;

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
