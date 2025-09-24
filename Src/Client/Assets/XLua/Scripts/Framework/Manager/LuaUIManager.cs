using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using XLua;

public class LuaUIManager : MonoBehaviour 
{
    //缓存UI
    Dictionary<string, GameObject> m_UI = new Dictionary<string, GameObject>();

    //UI层级
    Dictionary<string, Transform> m_UIGroups = new Dictionary<string, Transform>();

    private Transform m_UIParent;

    private void Awake()
    {
        m_UIParent=this.transform.parent.Find("UI");
    }

    public void SetUIGroup(List<string>group)
    {
        for(int i = 0; i < group.Count; i++)
        {
            GameObject go = new GameObject("Group-" + group[i]);
            go.transform.SetParent(m_UIParent, false);
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
        if(m_UI.TryGetValue(uiName,out ui))
        {
            UILogic uiLogic = ui.GetComponent<UILogic>();
            uiLogic.OnOpen();
            return;
        }

        Manager.Resource.LoadUI(uiName, (UnityEngine.Object obj) =>
        {
            ui = Instantiate(obj) as GameObject;
            m_UI.Add(uiName,ui);

            Transform parent = GetUIGroup(group);
            ui.transform.SetParent(parent, false);
            UILogic uiLogic = ui.AddComponent<UILogic>();
            uiLogic.Init(luaName);//相当于awake
            uiLogic.OnOpen();//相当于start
        });

    }
}
