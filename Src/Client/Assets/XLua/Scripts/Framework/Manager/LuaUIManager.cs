using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using XLua;

public class LuaUIManager : MonoBehaviour 
{
    //UI层级
    Dictionary<string, Transform> m_UIGroups = new Dictionary<string, Transform>();

    private Transform m_UIParent;

    private void Awake()
    {
        m_UIParent=this.transform.parent.Find("UI");//找到Root根节点下的UI
    }

    public void SetUIGroup(List<string>group)
    {
        for(int i = 0; i < group.Count; i++)
        {
            GameObject go = new GameObject("Group-" + group[i]);//设置名字
            go.transform.SetParent(m_UIParent, false);//设置父亲为m_UIParent
            m_UIGroups.Add(group[i],go.transform);
        }
    }

    //获取分组
    Transform GetUIGroup(string group)
    {
        if (!m_UIGroups.ContainsKey(group))
        {
            Debug.LogError("group没找到" + group);
        }
        return m_UIGroups[group];
    }

    public void OpenUI(string uiName, string group,string luaName)
    {
        GameObject ui = null;
        Transform parent = GetUIGroup(group);
        string uiPath = PathUtil.GetUIPath(uiName);
        UnityEngine.Object uiObj = Manager.Pool.Spawn("UI", uiPath);

        if(uiObj!=null)//已经缓存
        {
            ui = uiObj as GameObject;
            ui.transform.SetParent(parent, false);
            UILogic uiLogic = ui.GetComponent<UILogic>();
            uiLogic.OnOpen();
            return;
        }

        Manager.Resource.LoadUI(uiName, (UnityEngine.Object obj) =>
        {
            ui = Instantiate(obj) as GameObject;
            ui.transform.SetParent(parent, false);
            UILogic uiLogic = ui.AddComponent<UILogic>();
            uiLogic.Init(luaName);//相当于awake
            uiLogic.AssetName = uiPath;
            uiLogic.OnOpen();//相当于start
        });

    }
}
