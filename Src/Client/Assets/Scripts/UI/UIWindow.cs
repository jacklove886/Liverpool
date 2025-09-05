using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class UIWindow : MonoBehaviour {

    public delegate void CloseHandler(UIWindow sender, WindowResult result);
    public event CloseHandler OnClose;

    public virtual System.Type Type { get { return this.GetType(); } }

    public enum WindowResult
    {
        None=0,
        Yes,
        No
    }
	
    public void Close(WindowResult result = WindowResult.None)
    {
        UIManager.Instance.Close(Type);
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
}
