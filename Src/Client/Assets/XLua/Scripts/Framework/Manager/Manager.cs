using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//管理器的统一入口
public class Manager : MonoBehaviour
{
    private static ResourceManager _resource;
    public static ResourceManager Resource//资源加载
    {
        get
        {
            return _resource;
        }
    }

    private static LuaManager _lua;
    public static LuaManager Lua//Lua虚拟机
    {
        get
        {
            return _lua;
        }
    }

    private static LuaUIManager _ui;
    public static LuaUIManager UI//处理UI相关的Lua逻辑
    { 
        get
        {
            return _ui;
        }
    }

    private void Awake()//统一生命周期
    {
        _resource = this.gameObject.AddComponent<ResourceManager>();
        _lua = this.gameObject.AddComponent<LuaManager>();
        _ui = this.gameObject.AddComponent<LuaUIManager>();
    }


}
