using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathUtil
{
    //根目录
    public static readonly string AssetsPath = Application.dataPath;

    //需要打Bundle的目录
    public static readonly string BuildResourcesPath = AssetsPath + "/XLua热更框架/BuildResources/";

    //Bundle输出目录
    public static readonly string BundleOutPath = Application.streamingAssetsPath;

    //bundle的资源路径
    public static string BundleResourcePath
    {
        get { return Application.streamingAssetsPath; }
    }


    /*
     * 获取Unity的相对目录
     * 假设完整路径是"E:/Unity_code/mmorpg/mmorpg/Src/Client/Assets/XLua热更框架/BuildResources/UI/Res/file.png";
     * 返回路径Assets/XLua热更框架/BuildResources/UI/Res/file.png
    */
    public static string GetUnityPath(string path)
    {
        
        return path.Substring(path.IndexOf("Assets"));
    }

    //获取标准路径
    public static string GetStandardPath(string path)
    {
        if (string.IsNullOrEmpty(path))
        {
            return string.Empty;
        }
        return path.Trim().Replace("\\", "/");//将反斜杠替换为正斜杠
    }
}
