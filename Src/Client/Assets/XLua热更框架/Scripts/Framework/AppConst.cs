using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public enum GameMode
{
    EditorMode,
    PackBundle,
    UpdateMode,
}


public class AppConst
{
    public const string BundleExtension = ".ab";//扩展名
    public const string FileListName = "filelist.txt";//文件列表的名字

    public static GameMode GameMode = GameMode.EditorMode;//游戏模式
    
    public const string ResoucresUrl = "http://127.0.0.1/AssetBundles";//热更资源的地址
}

