using System;  
using System.Collections;  
using System.Collections.Generic;  
using UnityEngine;  
using UnityEngine.Events;  

public class SceneManager : MonoSingleton<SceneManager>
{
    UnityAction<float> onProgress = null; 

    protected override void OnStart()
    {
        DontDestroyOnLoad(gameObject);
    }


    public void LoadScene(string name)
    {
        Debug.LogFormat("进入的地图是: {0}", name);
        StartCoroutine(LoadLevel(name));
    }

    IEnumerator LoadLevel(string name)
    {
        AsyncOperation async = UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(name);
        async.allowSceneActivation = true;
        async.completed += LevelLoadCompleted;
        while (!async.isDone)
        {
            if (onProgress != null)
                onProgress(async.progress);
            yield return null;
        }
    }

    private void LevelLoadCompleted(AsyncOperation obj)
    {
        if (onProgress != null)  
            onProgress(1f);  
    }
}