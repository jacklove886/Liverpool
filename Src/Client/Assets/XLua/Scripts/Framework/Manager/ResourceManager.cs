using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;

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

    //存放Bundle资源的集合
    private Dictionary<string, AssetBundle> m_AssetBundles = new Dictionary<string, AssetBundle>();

    //解析版本文件
    public void ParseVersionFile()
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
            m_BundleInfos[bundleInfo.AssetsName] = bundleInfo;

            if (info[0].IndexOf("LuaScripts") > 0)//证明是lua文件
            {
                Manager.Lua.LuaNames.Add(info[0]);//加进列表
            }
        }
    }

    //异步加载资源  递归加载依赖资源
    IEnumerator LoadBundleAsync(string assetName,Action<UnityEngine.Object> action=null)
    {
        string bundleName = m_BundleInfos[assetName].BundleName;
        string bundlePath = Path.Combine(PathUtil.BundleResourcePath, bundleName);//路径拼接
        List<string> dependences = m_BundleInfos[assetName].Dependences;

        AssetBundle bundle = GetBundle(bundleName);
        if (bundle == null)
        { 
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
            bundle = request.assetBundle;
            m_AssetBundles[bundleName] = bundle;
        }

        //如果加载是场景 直接返回回调
        if (assetName.EndsWith(".unity"))
            {
                if (action != null)
                {
                    action.Invoke(null);
                    yield break;
                }
            }
        // 从Bundle中异步加载具体资源
        AssetBundleRequest bundleRequest = bundle.LoadAssetAsync(assetName);
        yield return bundleRequest;                   
        Debug.Log("This is LoadBundleAsync模式");

        //执行回调函数
        if (action != null && bundleRequest != null)
        {
            action.Invoke(bundleRequest.asset);
        }
    }

    private AssetBundle GetBundle(string name)
    {
        AssetBundle bundle = null;
        if (m_AssetBundles.TryGetValue(name, out bundle))
        {
            return bundle;
        }
        return null;
    }


#if UNITY_EDITOR
    //编辑器环境下使用
    void EditorLoadAsset(string assetName,Action<UnityEngine.Object> action=null)
    {
        Debug.Log("This is EditorLoadAsset模式");
        UnityEngine.Object obj = UnityEditor.AssetDatabase.LoadAssetAtPath(assetName, typeof(UnityEngine.Object));
        if (obj == null)
        {
            Debug.LogError("assets name is not exist" + assetName);
        }
        if (action != null)
        {
            action.Invoke(obj);
        }
    }
#endif

    private void LoadAsset(string assetName, Action<UnityEngine.Object> action)
    {
        Debug.Log("当前模式" + AppConst.GameMode);
        if (AppConst.GameMode != GameMode.EditorMode)
        {
            StartCoroutine(LoadBundleAsync(assetName, action));
        }

#if UNITY_EDITOR
        else
        {
            EditorLoadAsset(assetName, action);
        }
#endif
    }

    //资源接口
    public void LoadUI(string assetName, Action<UnityEngine.Object> action = null)
    {
        LoadAsset(PathUtil.GetUIPath(assetName), action);
    }

    public void LoadMusic(string assetName, Action<UnityEngine.Object> action = null)
    {
        LoadAsset(PathUtil.GetMusicPath(assetName), action);
    }

    public void LoadSound(string assetName, Action<UnityEngine.Object> action = null)
    {
        LoadAsset(PathUtil.GetSoundPath(assetName), action);
    }

    public void LoadEffect(string assetName, Action<UnityEngine.Object> action = null)
    {
        LoadAsset(PathUtil.GetEffectPath(assetName), action);
    }

    public void LoadScene(string assetName, Action<UnityEngine.Object> action = null)
    {
        LoadAsset(PathUtil.GetScenePath(assetName), action);
    }

    public void LoadLua(string assetName, Action<UnityEngine.Object> action=null)
    {
        LoadAsset(assetName, action);
    }

    public void LoadPrefab(string assetName, Action<UnityEngine.Object> action = null)
    {
        LoadAsset(assetName, action);
    }


    //Tag卸载暂时不做
    public void UnloadBundle(string name)
    {
       
    }
}
