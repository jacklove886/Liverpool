using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using XLua;

public class LuaManager : MonoBehaviour
{
    //所有的lua文件名
    public List<string> LuaNames = new List<string>();

    //缓存lua脚本内容
    private Dictionary<string,byte[]> m_LuaScripts;

    public LuaEnv LuaEnv;

    private void Awake()
    {
        LuaEnv = new LuaEnv();
        LuaEnv.AddLoader(Loader);
    }

    public void StartLua(string name)
    {
        LuaEnv.DoString(string.Format("require '{0}'",name));
    }

    byte[] Loader(ref string name)
    {
        return GetLuaScript(name);
    }

    private byte[] GetLuaScript(string name)
    {
        //require ui.login.register 替换为ui/login/register
        name = name.Replace(".", "/");
        string fileName = PathUtil.GetLuaPath(name);

        byte[] luaScript = null;
        if(!m_LuaScripts.TryGetValue(fileName,out luaScript))
        {
            Debug.LogError("lua script is not exist:" + fileName);
        }
        return luaScript;
    }

    void LoadLuaScript()
    {
        foreach(string name in LuaNames)
        {
            Manager.Resource.LoadLua(name,(UnityEngine.Object obj)=>
            {
                AddLuaScript(name, (obj as TextAsset).bytes);
                if (m_LuaScripts.Count >= LuaNames.Count)
                {
                    LuaNames.Clear();
                    LuaNames = null;
                }
            });
        }
    }

    private void AddLuaScript(string assetName, byte[] luaScript)
    {
        m_LuaScripts[assetName] = luaScript;
    }


#if UNITY_EDITOR
    void EditorLoadLuaScript()
    {
        //搜索路径下的所有文件夹
        string[] luaFiles = Directory.GetFiles(PathUtil.LuaPath, "*.bytes", SearchOption.AllDirectories);
        for(int i = 0; i < luaFiles.Length; i++)
        {
            string fileName = PathUtil.GetStandardPath(luaFiles[i]);
            byte[] file = File.ReadAllBytes(fileName);
            AddLuaScript(fileName, file);
        }
    }

#endif


    //Lua的内存回收
    private void Update()
    {
        if (LuaEnv != null)
        {
            LuaEnv.Tick();
        }
    }

    //销毁虚拟机
    private void OnDestroy()
    {
        if (LuaEnv != null)
        {
            LuaEnv.Dispose();
            LuaEnv = null;
        }
    }
}
