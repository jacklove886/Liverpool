using UnityEngine;

class MessageBox
{
    static UnityEngine.Object cacheObject = null;

    //静态方法创建消息框
    public static UIMessageBox Show(string message, string title="", MessageBoxType type = MessageBoxType.Information, string btnOK = "", string btnCancel = "")
    {
        if (cacheObject==null)
        {
            cacheObject = Resloader.Load<Object>("UI/UIMessageBox");//缓存机制 只用加载第一次
        }

        GameObject go = (GameObject)GameObject.Instantiate(cacheObject);
        Canvas canvas = go.GetComponent<Canvas>();
        canvas.sortingOrder = 10;//设置层级 在最前面显示
        UIMessageBox msgbox = go.GetComponent<UIMessageBox>();
        msgbox.Init(title, message, type, btnOK, btnCancel);//初始化
        return msgbox;
    }
}

public enum MessageBoxType
{
    /// <summary>
    /// Information Dialog with OK button
    /// </summary>
    Information = 1,

    /// <summary>
    /// Confirm Dialog whit OK and Cancel buttons
    /// </summary>
    Confirm = 2,

    /// <summary>
    /// Error Dialog with OK buttons
    /// </summary>
    Error = 3
}