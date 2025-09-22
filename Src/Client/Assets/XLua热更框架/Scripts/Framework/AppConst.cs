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

    public static GameMode GameMode = GameMode.EditorMode;
}

