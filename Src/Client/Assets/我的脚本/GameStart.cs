using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameStart : MonoBehaviour
{
    public GameMode GameMode;

    private void Start()
    {
        Manager.Event.Subscribe(10000, OnLuaInit);
        AppConst.GameMode = this.GameMode;
        DontDestroyOnLoad(this);

        Manager.Resource.ParseVersionFile();
        //初始化Lua虚拟机
        Manager.Lua.Init();
    }

    void OnLuaInit(object args)
    {
        Manager.Lua.StartLua("Main");//执行require Main
        XLua.LuaFunction func = Manager.Lua.LuaEnv.Global.Get<XLua.LuaFunction>("Main");
        func.Call();//执行Main里的方法

        Manager.Pool.CreateGameObjectPool("UI", 10);
        Manager.Pool.CreateGameObjectPool("Monster",120);
        Manager.Pool.CreateGameObjectPool("Effect", 120);
        Manager.Pool.CreateAssetPool("AssetBundle", 5);
    }

    private void OnApplicationQuit()
    {
        Manager.Event.UnSubscribe(10000, OnLuaInit);
    }
}
