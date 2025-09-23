using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using XLua;

public class UILogic : LuaBehaviour
{
    private Action m_LuaOpen;
    private Action m_LuaClose;

    public override void Init(string luaName)
    {
        base.Init(luaName);
        m_ScriptEnv.Get("OnOpen", out m_LuaOpen);
        m_ScriptEnv.Get("OnClose", out m_LuaClose);
    }

    public void OnOpen()
    {
        if (m_LuaOpen!= null)
        {
            m_LuaOpen.Invoke();
        }
    }

    public void OnClose()
    {
        if (m_LuaClose != null)
        {
            m_LuaClose.Invoke();
        }
    }

    protected override void Clear()
    {
        base.Clear();
        m_LuaOpen = null;
        m_LuaClose = null;
    }
}
