using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;
using UObject = UnityEngine.Object;

public class ResourceManager : MonoBehaviour
{
    internal class BundleInfo
    {
        public string AssetsName;// Unity中的资源路径：Assets/XLua热更框架/BuildResources/UI/Prefab/Test.prefab
        public string BundleName;// Bundle文件名：ui/prefab/test.prefab.ab
        public List<string> Dependences;//依赖资源列表
    }

    //存放Bundle信息的集合
    private Dictionary<string, BundleInfo> m_BundleInfos = new Dictionary<string, BundleInfo>();


    //解析版本文件
    private void ParseVersionFile()
    {
        //版本文件的路径
        string url = Path.Combine(PathUtil.BundleResourcePath, AppConst.FileListName);
        string[] data = File.ReadAllLines(url);//读取所有行到数组中

        //解析文件信息
        for(int i = 0; i < data.Length; i++)
        {
            BundleInfo bundleInfo = new BundleInfo();
            string[] info = data[i].Split('|');//通过竖线分割
            bundleInfo.AssetsName = info[0];
            bundleInfo.BundleName = info[1];
            //list特性:本质是数组 但是可以动态扩容
            bundleInfo.Dependences = new List<string>(info.Length - 2);
            //第三部分开始是依赖资源
            for(int j = 2; j < info.Length; j++)
            {
                bundleInfo.Dependences.Add(info[j]);
            }
            m_BundleInfos.Add(bundleInfo.AssetsName, bundleInfo);
        }
    }


    //异步加载资源  递归加载依赖资源
    IEnumerator LoadBundleAsync(string assetName,Action<UObject>action=null)
    {
        string bundleName = m_BundleInfos[assetName].BundleName;
        string bundlePath = Path.Combine(PathUtil.BundleResourcePath, bundleName);//路径拼接
        List<string> dependences = m_BundleInfos[assetName].Dependences;
        if (dependences != null && dependences.Count > 0)
        {
            for(int i = 0; i < dependences.Count; i++)
            {
                yield return LoadBundleAsync(dependences[i]);//递归
            }
        }
        // 异步加载Bundle文件
        AssetBundleCreateRequest request = AssetBundle.LoadFromFileAsync(bundlePath);
        yield return request;

        // 从Bundle中异步加载具体资源
        AssetBundleRequest bundleRequest = request.assetBundle.LoadAssetAsync(assetName);
        yield return bundleRequest;
        Debug.Log("This is LoadBundleAsync模式");

        //执行回调函数
        if (action != null && bundleRequest != null)
        {
            action.Invoke(bundleRequest.asset);
        }
    }


    //编辑器环境下使用
    void EditorLoadAsset(string assetName,Action<UObject>action=null)
    {
        Debug.Log("This is EditorLoadAsset模式");
        UObject obj = UnityEditor.AssetDatabase.LoadAssetAtPath(assetName, typeof(UObject));
        if (obj == null)
        {
            Debug.LogError("assets name is not exist" + assetName);
        }
        if (action != null)
        {
            action.Invoke(obj);
        }
    }

    private void LoadAsset(string assetName, Action<UObject> action)
    {
        if (AppConst.GameMode == GameMode.EditorMode)
            EditorLoadAsset(assetName, action);
        else
        StartCoroutine(LoadBundleAsync(assetName, action));
    }

    //资源接口
    public void LoadUI(string assetName, Action<UObject> action)
    {
        LoadAsset(PathUtil.GetUIPath(assetName), action);
    }

    public void LoadMusic(string assetName, Action<UObject> action)
    {
        LoadAsset(PathUtil.GetMusicPath(assetName), action);
    }

    public void LoadSound(string assetName, Action<UObject> action)
    {
        LoadAsset(PathUtil.GetSoundPath(assetName), action);
    }

    public void LoadEffect(string assetName, Action<UObject> action)
    {
        LoadAsset(PathUtil.GetEffectPath(assetName), action);
    }

    public void LoadScene(string assetName, Action<UObject> action)
    {
        LoadAsset(PathUtil.GetScenePath(assetName), action);
    }

    //Tag卸载暂时不做

    void Start()
    {
        ParseVersionFile();
        LoadUI("Test", OnComplete);
    }

    private void OnComplete(UObject obj)
    {
        GameObject go = Instantiate(obj) as GameObject;
        go.transform.SetParent(this.transform);
        go.SetActive(true);
        go.transform.localPosition = Vector3.zero;
    }
}
