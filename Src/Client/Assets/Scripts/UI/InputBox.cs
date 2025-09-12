using UnityEngine;

class InputBox
{
    static UnityEngine.Object cacheObject = null;

    //静态方法创建消息框
    public static UIInputBox Show(string message, string title = "", MessageBoxType type = MessageBoxType.Information, string btnOK = "", string btnCancel = "")
    {
        if (cacheObject == null)
        {
            cacheObject = Resloader.Load<Object>("UI/UIInputBox");//缓存机制 只用加载第一次
        }

        GameObject go = (GameObject)GameObject.Instantiate(cacheObject);
        Canvas canvas = go.GetComponent<Canvas>();
        canvas.sortingOrder = 10;//设置层级 在最前面显示
        UIInputBox inputBox = go.GetComponent<UIInputBox>();
        inputBox.Init(title, message, type, btnOK, btnCancel);//初始化
        return inputBox;
    }
}