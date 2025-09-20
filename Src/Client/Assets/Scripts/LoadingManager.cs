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
    private UILoading ui;
    public  bool isLoading = false;

    protected override void OnStart()
    {
        ui = UIManager.Instance.Show<UILoading>();
        ui.gameObject.transform.SetParent(this.transform);
        ui.gameObject.SetActive(false);
    }

    public void ShowLoading()
    {
        ui.gameObject.SetActive(true);
        ui.SetBackground();
        isLoading = true;
    }

    public void HideLoading()
    {
        isLoading = false;
        ui.gameObject.SetActive(false);
    }

    public void UpdateProgress(float progress)
    {
        ui.SetProgress(progress);
    }

}
