using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;

public class FileUtil
{

    //检测文件是否存在
    public static bool IsExists(string path)
    {
        FileInfo file = new FileInfo(path);
        return file.Exists;
    }


    //写入文件
    public static void WriteFile(string path,byte[] data)
    {
        //获取标准路径
        path = PathUtil.GetStandardPath(path);
        /*
         *  文件夹的路径
         *  假设 path = "E:/Unity/StreamingAssets/ui/prefab/test.ab"
         *  dir = "E:/Unity/StreamingAssets/ui/prefab"
        */ 
        string dir = path.Substring(0, path.LastIndexOf("/"));
        if (!Directory.Exists(dir))
        {
            Directory.CreateDirectory(dir);
        }
        FileInfo file = new FileInfo(path);
        if (file.Exists)
        {
            file.Delete();
        }
        try
        {
            //写入文件数据
            using(FileStream fs= new FileStream(path, FileMode.Create, FileAccess.Write))
            {
                fs.Write(data, 0, data.Length);
                fs.Close();
            }
        }
        catch(IOException e)
        {
            Debug.LogError(e.Message);
        }
    }



}

