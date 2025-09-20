using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;

public class UILoading : UIWindow {

    public GameObject LoadingScene;
    public Slider progressBar;
    public Image Bg;

    public void SetProgress(float progress)
    {
        progressBar.value = progress;
    }

    public void SetBackground()
    {
        int x = Random.Range(0, SpriteManager.Instance.loadingBg.Length);
        Bg.overrideSprite = SpriteManager.Instance.loadingBg[x];
    }
}
