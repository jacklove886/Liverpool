using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;

using SkillBridge.Message;
using ProtoBuf;
using Services;
using Managers;

public class LoadingManager : MonoBehaviour {

    public GameObject UITips;
    public GameObject UILoading;
    public GameObject UILogin;

    public Slider progressBar;
    public Text progressText;
    public Text progressNumber;
    public AudioSource audioSource;

    IEnumerator Start()
    {
        audioSource.Play();
        log4net.Config.XmlConfigurator.ConfigureAndWatch(new System.IO.FileInfo("log4net.xml"));
        UnityLogger.Init();
        Common.Log.Init("Unity");
        Common.Log.Info("LoadingManager start");

        UITips.SetActive(true);
        UILoading.SetActive(false);
        UILogin.SetActive(false);
        yield return new WaitForSeconds(2f);
        print("加载完毕1");
        UILoading.SetActive(true);
        yield return new WaitForSeconds(2.5f);
        UITips.SetActive(false);
        print("加载完毕2");
        yield return DataManager.Instance.LoadData();//携程加载数据库

        //初始化服务器
        MapService.Instance.Init();
        UserService.Instance.Init();
        StatusService.Instance.Init();
        FriendService.Instance.Init();

        //初始化管理器
        ShopManager.Instance.Init();


        for (float i = 50; i < 100;)//从50-100开始模拟加载
        {
            i += Random.Range(0.1f, 1.5f);//随机加载0.1-1.5个进度条
            progressBar.value = i;
            yield return new WaitForEndOfFrame();//等待UI更新
        }

        UILoading.SetActive(false);
        UILogin.SetActive(true);//显示登录界面
        yield return null;
    }


    void Update () {

    }
}
