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

    public Action InitOK;

    public void Init(Action Init)//初始化
    {
        InitOK += Init;
        LuaEnv = new LuaEnv();
        LuaEnv.AddLoader(Loader);
        m_LuaScripts = new Dictionary<string, byte[]>();
        if (AppConst.GameMode != GameMode.EditorMode)
        {
            LoadLuaScript();
        }       
        
#if UNITY_EDITOR
        else
        {
            EditorLoadLuaScript();
        }
#endif
    }

    public void StartLua(string name)
    {
        LuaEnv.DoString(string.Format("require '{0}'",name));
    }

    byte[] Loader(ref string name)
    {
        return GetLuaScript(name);
    }

    //从缓存中获取lua脚本名
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

    //AssetBundle加载
    void LoadLuaScript()
    {
        foreach(string name in LuaNames)
        {
            Manager.Resource.LoadLua(name,(UnityEngine.Object obj)=>
            {
                AddLuaScript(name, (obj as TextAsset).bytes); //将obj转换为TextAsset类型
                if (m_LuaScripts.Count >= LuaNames.Count)
                {
                    //所有lua文件加载完成的时候
                    if (InitOK != null)
                    {
                        InitOK.Invoke();
                    }                  
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
        //搜索路径下的所有含有*.bytes的文件
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
            LuaEnv.Tick();//处理垃圾回收
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
