using Models;
using System;  
using System.Collections;  
using System.Collections.Generic;  
using UnityEngine;  
using UnityEngine.Events;  

public class SceneManager : MonoSingleton<SceneManager>
{
    [SerializeField]
    private List<string> fieldScenes = new List<string>
    {
        "Map01", "Map02", "Map03",
    };
    public void LoadTargetScene(string name)//加载场景
    {
        Debug.Log("进入的场景是"+name);
        bool isField = fieldScenes.Contains(name);
        if (!LoadingManager.Instance.isLoading)
        {
            var player = User.Instance.CurrentCharacterPlayerInput;
            if (player != null)
            player.rb.gameObject.SetActive(false);
            StartCoroutine(LoadLevel(name, isField, player));
        }        
    }

    IEnumerator LoadLevel(string name,bool isField,PlayerInputController player)
    {
        if (isField)
        {
            LoadingManager.Instance.ShowLoading();
        }

        //虚假加载 0%-90%
        if (isField)
        {
            for (float fakeProgress = 0; fakeProgress < 90; fakeProgress += UnityEngine.Random.Range(2f, 4f))
            {
                LoadingManager.Instance.UpdateProgress(fakeProgress);
                yield return new WaitForSeconds(0.05f); // 每0.05秒涨一点
            }
        }

        //真实加载
        AsyncOperation async = UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(name);
        async.allowSceneActivation = false;
        while (async.progress < 0.9f)
        {
            yield return null;
        }

        //虚假加载 90%-100%
        if (isField)
        {
            for (float fakeProgress = 90; fakeProgress < 100; fakeProgress += UnityEngine.Random.Range(0.6f, 1.2f))
            {
                LoadingManager.Instance.UpdateProgress(fakeProgress);
                yield return new WaitForSeconds(0.05f);
            }
        }

        async.allowSceneActivation = true;//先激活场景        

        while (!async.isDone)
        {
            yield return null;
        }

        yield return null;
        yield return null;

        if (isField)
        {
            LoadingManager.Instance.HideLoading();           
            if (player != null)
            {
                player.rb.gameObject.SetActive(true);
            }
        }
        
    }
}