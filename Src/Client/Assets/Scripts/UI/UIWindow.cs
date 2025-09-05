using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class UIWindow : MonoBehaviour {

    //sender是UIWindow的实例
    public delegate void CloseHandler(UIWindow sender, WindowResult result);//WindowResult是枚举类型
    public event CloseHandler OnClose;

    public virtual System.Type Type { get { return this.GetType(); } }

    public enum WindowResult
    {
        None=0,
        Yes,
        No
    }
	
    public void Close(WindowResult result = WindowResult.None)//不传参数默认是None 可以传参Yes 或者No
    {
        UIManager.Instance.Close(Type);//关闭预制体
        if (OnClose != null)
        {
            OnClose(this, result);
        }
        OnClose = null;
    }
    
    public virtual void OnCloseClick()
    {
        Close();
    }

    public virtual void OnYesClick()
    {
        Close(WindowResult.Yes);
    }

    private void OnMouseDown()
    {
        Debug.LogFormat(name + "已点击");
    }
}
