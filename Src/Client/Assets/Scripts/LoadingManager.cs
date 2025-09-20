using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;

using SkillBridge.Message;
using ProtoBuf;
using Services;
using Managers;
using Common.Data;

public class LoadingManager : MonoSingleton<LoadingManager>
{

    public GameObject UILoadingPrefab;
    private GameObject UILoading;
    private Slider progressBar;
    private Image Bg;

    public  bool isLoading = false;

    protected override void OnStart()
    {
        UILoading= Instantiate(UILoadingPrefab, this.transform);
        Bg= UILoading.GetComponentInChildren<Image>();
        progressBar = Bg.gameObject.GetComponentInChildren<Slider>();
        UILoading.SetActive(false);
    }

    public void ShowLoading()
    {        
        int x = Random.Range(0, SpriteManager.Instance.loadingBg.Length);
        Bg.overrideSprite = SpriteManager.Instance.loadingBg[x];
        UILoading.SetActive(true);
        isLoading = true;
    }

    public void HideLoading()
    {
        UILoading.SetActive(false);
        isLoading = false;
    }

    public void UpdateProgress(float progress)
    {
        progressBar.value = progress;
    }

}
