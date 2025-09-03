using System;  
using System.Collections;  
using System.Collections.Generic;  
using UnityEngine;  
using UnityEngine.Events;  

public class SceneManager : MonoSingleton<SceneManager>
{
    UnityAction<float> onProgress = null; 

    private readonly string[] largeScenes = { "MainCity"};

    protected void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }

    void Update()
    {
        
    }

    public void LoadScene(string name)
    {
        Debug.LogFormat("进入的地图是: {0}", name);
        if (IsLargeScene(name))
        {
            Debug.Log("检测到大场景，使用异步加载: " + name);
            StartCoroutine(LoadLevelAsync(name));  
        }
        else
        {
            Debug.Log("检测到小场景，使用同步加载: " + name);
            UnityEngine.SceneManagement.SceneManager.LoadScene(name);  
        }
    }

    private bool IsLargeScene(string sceneName)
    {
        foreach (string largeSceneKeyword in largeScenes)
        {
            if (sceneName==largeSceneKeyword)  
            {
                return true;  // 大场景
            }
        }
        return false;  // 小场景
    }

    IEnumerator LoadLevelAsync(string name)
    {
        AsyncOperation async = UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(name);  
        async.allowSceneActivation = false;  // 先不激活场景，避免重复初始化问题

        // 等待加载到90%
        while (async.progress < 0.9f)
        {
            if (onProgress != null)  
                onProgress(async.progress);  // 更新加载进度
            yield return null;  
        }

        yield return new WaitForSeconds(0.5f);

        async.allowSceneActivation = true;  

        // 等待场景完全激活
        while (!async.isDone)
        {
            if (onProgress != null)  
                onProgress(1f);  // 设置进度为100%
            yield return null;  
        }

        Debug.Log("场景激活完成: " + name);
    }

    internal void LoadScene(int v)
    {
        
    }

    private void LevelLoadCompleted(AsyncOperation obj)
    {
        if (onProgress != null)  
            onProgress(1f);  
    }
}