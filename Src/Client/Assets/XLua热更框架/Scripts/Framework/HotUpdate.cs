using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;

public class HotUpdate : MonoBehaviour
{
    byte[] m_ReadPathFileListData;
    byte[] m_ServerFileListData;
    //封装文件的下载信息
    internal class DownFileInfo
    {
        public string url;
        public string fileName;
        public DownloadHandler fileData;//文件数据
    }


    //下载单个文件
    IEnumerator DownLoadFile(DownFileInfo info,Action<DownFileInfo> Complete)
    {
        //请求下载
        UnityWebRequest webRequest = UnityWebRequest.Get(info.url);
        //发送请求并等待响应
        yield return webRequest.SendWebRequest();
        if (webRequest.isHttpError || webRequest.isNetworkError)
        {
            Debug.LogError("下载文件出错" + info.url);
            yield break;
        }
        info.fileData = webRequest.downloadHandler;
        if (Complete != null)
        {
            Complete.Invoke(info);
        }
        webRequest.Dispose();//释放资源
    }

    //下载多个文件
    IEnumerator DownLoadFile(List<DownFileInfo> infos, Action<DownFileInfo> Complete,Action DownLoadAllComplete)
    {
        foreach(var info in infos)
        {
            yield return DownLoadFile(info, Complete);
        }
        if (DownLoadAllComplete != null)
        {
            DownLoadAllComplete.Invoke();
        }
    }


    //获取文件信息
    private List<DownFileInfo> GetFileList(string fileData,string path)
    {
        string content = fileData.Trim().Replace("\r", "");
        string[] files = content.Split('\n');
        List<DownFileInfo> downFileInfos = new List<DownFileInfo>(files.Length);
        for(int i = 0; i < files.Length; i++)
        {
            string[] info = files[i].Split('|');
            DownFileInfo fileInfo = new DownFileInfo();
            fileInfo.fileName = info[1];
            fileInfo.url = Path.Combine(path, info[1]);
            downFileInfos.Add(fileInfo);
        }
        return downFileInfos;
    }

    private void Start()
    {
        if (IsFirstInstall())
        {
            ReleaseResources();
        }
        else
        {
            CheckUpdate();
        }
    }

    private bool IsFirstInstall()
    {
        //判断只读目录是否存在版本文件
        bool isExistReadPath = FileUtil.IsExists(Path.Combine(PathUtil.ReadPath, AppConst.FileListName));

        //判断可读写目录是否存在版本文件
        bool isExistReadWritePath = FileUtil.IsExists(Path.Combine(PathUtil.ReadWritePath, AppConst.FileListName));

        return isExistReadPath && !isExistReadWritePath;
    }

    //释放资源到可读写目录
    private void ReleaseResources()
    {
        string url = Path.Combine(PathUtil.ReadPath, AppConst.FileListName);
        DownFileInfo info = new DownFileInfo();
        info.url = url;
        StartCoroutine(DownLoadFile(info, OnDownLoadReadPathFileListComplete));
    }

    private void OnDownLoadReadPathFileListComplete(DownFileInfo file)
    {
        m_ReadPathFileListData = file.fileData.data;
        //获取文件
        List<DownFileInfo> fileInfos = GetFileList(file.fileData.text, PathUtil.ReadPath);
        StartCoroutine(DownLoadFile(fileInfos, OnReleaseFileComplete, OnReleaseAllFileComplete));
    }
    private void OnReleaseFileComplete(DownFileInfo file)
    {
        string writeFile = Path.Combine(PathUtil.ReadWritePath, file.fileName);
        FileUtil.WriteFile(writeFile, file.fileData.data);
    }

    private void OnReleaseAllFileComplete()
    {
        FileUtil.WriteFile(Path.Combine(PathUtil.ReadWritePath, AppConst.FileListName), m_ReadPathFileListData);
        CheckUpdate();
    }  

    //检测更新
    private void CheckUpdate()
    {
        string url = Path.Combine(AppConst.ResoucresUrl, AppConst.FileListName);
        DownFileInfo info = new DownFileInfo();
        info.url = url;
        StartCoroutine(DownLoadFile(info, OnDownLoadServerListComplete));
    }

    //下载服务器回调方法
    private void OnDownLoadServerListComplete(DownFileInfo file)
    {
        m_ServerFileListData = file.fileData.data;
        List<DownFileInfo> fileInfos = GetFileList(file.fileData.text, AppConst.ResoucresUrl);//资源服务器上的信息
        List<DownFileInfo> downListFiles = new List<DownFileInfo>();//需要下载的文件列表

        for(int i = 0; i < fileInfos.Count; i++)
        {
            string localFile = Path.Combine(PathUtil.ReadWritePath, fileInfos[i].fileName);
            if (!FileUtil.IsExists(localFile))//判断本地资源是否存在
            {
                fileInfos[i].url = Path.Combine(AppConst.ResoucresUrl, fileInfos[i].fileName);
                downListFiles.Add(fileInfos[i]);
            }
        }
        if (downListFiles.Count > 0)
        {
            StartCoroutine(DownLoadFile(fileInfos, OnUpdateFileComplete, OnUpdateAllFileComplete));
        }
        else
        {
            EnterGame();
        }
    }

    private void OnUpdateFileComplete(DownFileInfo file)
    {
        string writeFile = Path.Combine(PathUtil.ReadWritePath, file.fileName);
        FileUtil.WriteFile(writeFile, file.fileData.data);
    }

    private void OnUpdateAllFileComplete()
    {
        FileUtil.WriteFile(Path.Combine(PathUtil.ReadWritePath, AppConst.FileListName), m_ServerFileListData);
        EnterGame();
    }

    private void EnterGame()
    {
        
    }
}
