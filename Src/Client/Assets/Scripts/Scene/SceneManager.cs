using System;  
using System.Collections;  
using System.Collections.Generic;  
using UnityEngine;  
using UnityEngine.Events;  

public class SceneManager : MonoSingleton<SceneManager>
{


    public void LoadTargetScene(string name)//加载场景
    {
        Debug.Log("进入的场景是"+name);
        if (!LoadingManager.Instance.isLoading)
        {
            StartCoroutine(LoadLevel(name));
        }      
    }

    IEnumerator LoadLevel(string name)
    {
        LoadingManager.Instance.ShowLoading();
        AsyncOperation async = UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(name);
        async.allowSceneActivation = false;
        while (async.progress < 0.9f)
        {
            float progress = async.progress / 0.9f;//算出真实进度
            LoadingManager.Instance.UpdateProgress(progress * 100);//更新进度条
            yield return null;
        }
        yield return new WaitForSeconds(0.5f);//加载到90%  等待0.5f
        LoadingManager.Instance.HideLoading();
        async.allowSceneActivation = true;
    }
}