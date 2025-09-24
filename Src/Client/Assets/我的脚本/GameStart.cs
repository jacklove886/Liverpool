using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameStart : MonoBehaviour
{
    public GameMode GameMode;

    private void Start()
    {
        AppConst.GameMode = this.GameMode;
        DontDestroyOnLoad(this);

        Manager.Resource.ParseVersionFile();
        //初始化Lua虚拟机
        Manager.Lua.Init(
            ()=>//回调函数采用匿名方法
            {
                Manager.Lua.StartLua("Main");//执行require Main
                XLua.LuaFunction func = Manager.Lua.LuaEnv.Global.Get<XLua.LuaFunction>("Main");
                func.Call();//执行Main里的方法
            }
            );      
        
    }
}
