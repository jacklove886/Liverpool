using Candlelight;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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

        //文件信息列表 记录依赖关系
        List<string> bundleInfos = new List<string>();

        //参数:路径 匹配字符串 文件
        string[] files = Directory.GetFiles(PathUtil.BundleResourcesPath, "*", SearchOption.AllDirectories);
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
            string bundleName = fileName.Replace(PathUtil.BundleResourcesPath, "").ToLower();
            assetBundle.assetBundleName = bundleName + ".ab";

            assetBundleBuilds.Add(assetBundle);//加进列表

            //添加文件和依赖信息
            List<string> dependenceInfo = GetDependence(assetName);
            string bundleInfo = assetName + "|" + bundleName;

            if (dependenceInfo.Count > 0)
            {
                bundleInfo = bundleInfo + "|" + string.Join("|", dependenceInfo.ToArray());
            }
            bundleInfos.Add(bundleInfo);
        }        
        if (Directory.Exists(PathUtil.BundleOutPath))
        {
            Directory.Delete(PathUtil.BundleOutPath, true);//删除所有文件
        }
        Directory.CreateDirectory(PathUtil.BundleOutPath);//创建目录
        BuildPipeline.BuildAssetBundles(PathUtil.BundleOutPath, assetBundleBuilds.ToArray(), BuildAssetBundleOptions.None, target);
        File.WriteAllLines(PathUtil.BundleOutPath + "/" + AppConst.FileListName, bundleInfos.ToArray());// 将文件信息写入文本文件

        AssetDatabase.Refresh();// 刷新Unity资源数据库
    }


    //获取依赖列表
    static List<string> GetDependence(string curFile)
    {
        List<string> dependence = new List<string>();
        string[] files = AssetDatabase.GetDependencies(curFile);//获取文件的所有依赖
        dependence = files.Where(file => !file.EndsWith(".cs") && !file.Equals(curFile)).ToList();//不要脚本文件和自身
        return dependence;
    }
}
