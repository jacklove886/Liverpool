using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class UIWindow : MonoBehaviour {//abstarct意义是防止直接实力化UIWindow 但是子类可以直接使用方法

    //uIDialog.OnClose += UIDialog_OnClose;//订阅UIWindow的事件
    //private void UIDialog_OnClose(UIWindow sender, UIWindow.WindowResult result)
    public event System.Action<UIWindow, WindowResult> OnClose;//发布关闭委托

    public virtual System.Type Type { get { return this.GetType(); } }//每个子类返回自己的具体类型

    public GameObject Root;

    public enum WindowResult
    {
        None=0,
        Yes,
        No
    }
	
    public void Close(WindowResult result = WindowResult.None)//不传参数默认是None 可以传参Yes 或者No
    {
        SoundManager.Instance.PlayUI(SoundDefine.Show);
        UIManager.Instance.Close(Type);//关闭预制体
        if (OnClose != null)
        {
            OnClose(this, result);
        }
        OnClose = null;//清空订阅事件
    }
    
    public virtual void OnCloseClick()
    {
        Close();
    }

    public virtual void OnYesClick()//虚方法 子类可以通过按钮绑定:yesButton.onClick.AddListener(OnYesClick);
    {
        Close(WindowResult.Yes);
    }

    public virtual void OnNoClick()
    {
        Close(WindowResult.No);
    }

}
