using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using XLua;

public class LuaEntityManager : MonoBehaviour
{
    Dictionary<string, GameObject> m_Entities = new Dictionary<string, GameObject>();
    Dictionary<string, Transform> m_Groups = new Dictionary<string, Transform>();

    private Transform m_EntityParent;

    private void Awake()
    {
        m_EntityParent = this.transform.parent.Find("Entity");
    }

    public void SetEntityGroup()
}
