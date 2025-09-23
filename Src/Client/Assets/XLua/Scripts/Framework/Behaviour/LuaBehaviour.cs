using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using XLua;

public class LuaBehaviour : MonoBehaviour
{
    private LuaEnv m_LuaEnv = Manager.Lua.LuaEnv;
    protected LuaTable m_ScriptEnv;

    private Action m_LuaInit;
    private Action m_LuaUpdate;
    private Action m_LuaOnDestroy;

    public string luaName;
    private void Awake()
    {
        //为每个脚本设置一个独立的环境,可一定程度上防止脚本间全局变量、函数冲突
        m_ScriptEnv = m_LuaEnv.NewTable();
        
        LuaTable meta = m_LuaEnv.NewTable();//创建元表
        meta.Set("__index", m_LuaEnv.Global);
        m_ScriptEnv.SetMetaTable(meta);
        meta.Dispose();//meta释放引用

        m_ScriptEnv.Set("self", this);//Lua脚本中可以通过self访问这个Unity组件    
    }

    public virtual void Init(string luaName)
    {
        m_LuaEnv.DoString(Manager.Lua.GetLuaScript(luaName), "LuaTestScript", m_ScriptEnv);
        m_ScriptEnv.Get("OnInit", out m_LuaInit);
        m_ScriptEnv.Get("Update", out m_LuaUpdate);
        m_ScriptEnv.Get("OnDestroy", out m_LuaOnDestroy);
        if (m_LuaInit != null)
        {
            m_LuaInit.Invoke();
        }
    }

    private void Update()
    {
        if (m_LuaUpdate != null)
        {
            m_LuaUpdate.Invoke();
        }
    }

    protected virtual void Clear()
    {
        m_LuaInit = null;
        m_LuaUpdate = null;
        m_LuaOnDestroy = null;
        if (m_ScriptEnv != null)
        {
            m_ScriptEnv.Dispose();
        }
        m_ScriptEnv = null;
    }

    private void OnDestroy()
    {
        if (m_LuaOnDestroy != null)
        {
            m_LuaOnDestroy.Invoke();
        }
        Clear();
    }

    private void OnApplicationQuit()
    {
        Clear();
    }
}
