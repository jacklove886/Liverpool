using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LuaSceneManager : MonoBehaviour
{
    private string m_LogicName="[SceneLogic]";

    private void Awake()
    {
        UnityEngine.SceneManagement.SceneManager.activeSceneChanged += OnActiveSceneChanged;
    }


    //场景切换的回调
    private void OnActiveSceneChanged(Scene s1, Scene s2)
    {
        if (!s1.isLoaded || !s2.isLoaded)
            return;
        SceneLogic logic1 = GetSceneLogic(s1);
        SceneLogic logic2 = GetSceneLogic(s2);

        if (logic1 != null)
        {
            logic1.OnInActive();
        }

        if (logic2 != null)
        {
            logic2.OnActive();
        }
    }

    //激活场景
    public void SetSceneActive(string sceneName)
    {
        Scene scene = UnityEngine.SceneManagement.SceneManager.GetSceneByName(sceneName);
        UnityEngine.SceneManagement.SceneManager.SetActiveScene(scene);
    }


    public void LoadScene(string sceneName,string luaName)
    {
        Manager.Resource.LoadScene(sceneName, (UnityEngine.Object obj) =>
         {
             //叠加的加载方式  加载下一个场景 但保留上一个场景
             StartCoroutine(StartLoadScene(sceneName, luaName, LoadSceneMode.Additive));
         });
    }

    public void ChangeScene(string sceneName, string luaName)
    {
        Manager.Resource.LoadScene(sceneName, (UnityEngine.Object obj) =>
        {
            //Single模式 加载下一个场景会卸载上一个场景
            StartCoroutine(StartLoadScene(sceneName, luaName, LoadSceneMode.Single));
        });
    }

    private IEnumerator StartLoadScene(string sceneName,string luaName,LoadSceneMode mode)
    {
        if (IsLoadScene(sceneName))//检测场景是否已经加载
            yield break;

        //异步加载
        AsyncOperation async = UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(sceneName, mode);
        async.allowSceneActivation = true;
        yield return async;

        Scene scene = UnityEngine.SceneManagement.SceneManager.GetSceneByName(sceneName);
        GameObject go = new GameObject(m_LogicName);

        UnityEngine.SceneManagement.SceneManager.MoveGameObjectToScene(go, scene);

        SceneLogic logic = go.AddComponent<SceneLogic>();
        logic.Init(luaName);
        logic.SceneName = sceneName;
        logic.OnEnter();
    }

    private bool IsLoadScene(string sceneName)
    {
        Scene scene = UnityEngine.SceneManagement.SceneManager.GetSceneByName(sceneName);
        return scene.isLoaded;
    }

    //卸载场景
    public void UnLoadSceneAsync(string sceneName)
    {
        StartCoroutine(UnLoadScene(sceneName));
    }

    private IEnumerator UnLoadScene(string sceneName)
    {
        Scene scene= UnityEngine.SceneManagement.SceneManager.GetSceneByName(sceneName);
        if (!scene.isLoaded)
        {
            Debug.LogError("scene not is loaded");
            yield break;
        }
        SceneLogic logic = GetSceneLogic(scene);
        if (logic != null)
        {
            logic.OnQuit();
        }      
        AsyncOperation async = UnityEngine.SceneManagement.SceneManager.UnloadSceneAsync(scene);
        yield return async;
    }

    private SceneLogic GetSceneLogic(Scene scene)
    {
        //获取场景中所有根级别的GameObject  即没有父节点的GameObject
        GameObject[] gameObjects = scene.GetRootGameObjects();
        foreach(GameObject go in gameObjects)
        {
            if (go.name==m_LogicName)
            {
                SceneLogic logic = go.GetComponent<SceneLogic>();
                return logic;
            }
        }
        return null;
    }
}
