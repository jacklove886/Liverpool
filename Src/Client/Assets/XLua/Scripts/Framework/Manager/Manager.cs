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

    private static LuaEntityManager _entity;
    public static LuaEntityManager Entity//处理UI相关的Lua逻辑
    {
        get
        {
            return _entity;
        }
    }

    private static LuaSceneManager _scene;
    public static LuaSceneManager Scene//处理场景相关的Lua逻辑
    {
        get
        {
            return _scene;
        }
    }

    private static LuaAudioManager _sound;
    public static LuaAudioManager Sound//处理声音相关的Lua逻辑
    {
        get
        {
            return _sound;
        }
    }

    private static LuaEventManager _event;
    public static LuaEventManager Event//处理声音相关的Lua逻辑
    {
        get
        {
            return _event;
        }
    }

    private void Awake()//统一生命周期
    {
        _resource = this.gameObject.AddComponent<ResourceManager>();
        _lua = this.gameObject.AddComponent<LuaManager>();
        _ui = this.gameObject.AddComponent<LuaUIManager>();
        _entity = this.gameObject.AddComponent<LuaEntityManager>();
        _scene = this.gameObject.AddComponent<LuaSceneManager>();
        _sound = this.gameObject.AddComponent<LuaAudioManager>();
        _event = this.gameObject.AddComponent<LuaEventManager>();
    }


}
