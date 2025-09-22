using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public class BuildTool : Editor
{
    [MenuItem("XLuaTools/Build Windows Bundle")]
    static void BundleWindowsBuild()
    {
        Build(BuildTarget.StandaloneWindows);
    }

    [MenuItem("XLuaTools/Build Android Bundle")]
    static void BundleAndroidBuild()
    {
        Build(BuildTarget.Android);
    }

    [MenuItem("XLuaTools/Build iPhone Bundle")]
    static void BundleiPhoneBuild()
    {
        Build(BuildTarget.iOS);
    }

    static void Build(BuildTarget target)
    {
        //定义要打包的资源和Bundle名称
        List<AssetBundleBuild> assetBundleBuilds = new List<AssetBundleBuild>();
        //参数:路径 匹配字符串 文件
        string[] files = Directory.GetFiles(PathUtil.BuildResourcesPath, "*", SearchOption.AllDirectories);
        //排除meta文件
        for(int i = 0; i < files.Length; i++)
        {
            if (files[i].EndsWith(".meta"))
                continue;
            
            AssetBundleBuild assetBundle = new AssetBundleBuild();

            string fileName= PathUtil.GetStandardPath(files[i]);
            Debug.Log("file" + fileName);

            string assetName = PathUtil.GetUnityPath(fileName);
            assetBundle.assetNames = new string[] {assetName};//Unity相对目录
            string bundleName = fileName.Replace(PathUtil.BuildResourcesPath, "").ToLower();
            assetBundle.assetBundleName = bundleName + ".ab";

            assetBundleBuilds.Add(assetBundle);//加进列表
        }
        if (Directory.Exists(PathUtil.BundleOutPath))
        {
            Directory.Delete(PathUtil.BundleOutPath, true);//删除所有文件
        }
        Directory.CreateDirectory(PathUtil.BundleOutPath);//创建目录
        BuildPipeline.BuildAssetBundles(PathUtil.BundleOutPath, assetBundleBuilds.ToArray(), BuildAssetBundleOptions.None, target);
    }
}
