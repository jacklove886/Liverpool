using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : Singleton<UIManager> {

    class UIElement
    {
        public string Resources;//预制体的路径
        public bool Cache; //控制UI关闭时是销毁还是隐藏  true隐藏 false销毁
        public GameObject Instance;
    }

    //Type填写类名 UIElement里填上面的三个变量
    private Dictionary<Type, UIElement> UIResources = new Dictionary<Type, UIElement>();

    public UIManager()
    {
        //每有一个UI类就在UIResources字典里注册一个  
        /*new UIElement 等价于:
        UIElement element = new UIElement();
        element.Resources = "UI/UIDialog";
        element.Cache = true;*/
        UIResources.Add(typeof(UIDialog), new UIElement() { Resources = "UI/UIDialog", Cache = true });//只隐藏不销毁
        }

    ~UIManager()
    {

    }

    public T Show<T>()
    {
        //SoundManager.Instance.PlaySound("ui_open");
        Type type = typeof(T);
        if (UIResources.ContainsKey(type))
        {
            UIElement info = UIResources[type];//实例化一个UIElement对象 可以调用三个变量
            if (info.Instance != null)
            {
                info.Instance.SetActive(true);//如果有实例 直接开启
            }
            else
            {
                UnityEngine.Object prefab = Resources.Load(info.Resources);//因为加载的资源可能是任何类型的 不一定是GameObject
                if (prefab == null)
                {
                    return default(T);//返回T类型的默认值 比如string返回null int返回0 bool返回false
                }
                info.Instance = (GameObject)GameObject.Instantiate(prefab);
            }
            return info.Instance.GetComponent<T>();//返回T组件
        }
        return default(T);
    }

    public void Close(Type type)
    {
        //SoundManager.Instance.PlaySound("ui_close");
        if (UIResources.ContainsKey(type))
        {
            UIElement info = UIResources[type];
            if (info.Cache)
            {
                info.Instance.SetActive(false);//关闭
            }
            else
            {
                GameObject.Destroy(info.Instance);//销毁
                info.Instance = null;
            }
        }
    }
}
