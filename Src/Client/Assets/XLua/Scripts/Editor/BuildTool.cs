using Candlelight;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

//将资源打包成AB包
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
        //存储AB包的信息   BuildPipeline使用  用来生成.ab文件
        List<AssetBundleBuild> assetBundleBuilds = new List<AssetBundleBuild>();

        //存储文件信息和依赖关系  运行的ResourceManager使用 用来生成txt文本
        List<string> bundleInfos = new List<string>();

        //参数:路径 匹配字符串 文件
        string[] files = Directory.GetFiles(PathUtil.BuildResourcesPath, "*", SearchOption.AllDirectories);
        //排除meta文件
        for(int i = 0; i < files.Length; i++)
        {
            if (files[i].EndsWith(".meta"))
                continue;
            
            AssetBundleBuild assetBundle = new AssetBundleBuild();

            string fileName= PathUtil.GetStandardPath(files[i]);//获取标准路径
            Debug.Log("file" + fileName);

            string assetName = PathUtil.GetUnityPath(fileName);//获取Unity路径
            assetBundle.assetNames = new string[] {assetName};
            string bundleName = fileName.Replace(PathUtil.BuildResourcesPath, "").ToLower();//获取bundle路径并小写
            assetBundle.assetBundleName = bundleName + ".ab";

            assetBundleBuilds.Add(assetBundle);//加进列表

            //添加文件和依赖信息
            List<string> dependenceInfo = GetDependence(assetName);
            string bundleInfo = assetName + "|" + bundleName + ".ab";

            if (dependenceInfo.Count > 0)
            {
                //dependenceInfo是列表 要用string.Join
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
