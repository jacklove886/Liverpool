using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoolManager : MonoBehaviour
{
    Transform m_PoolParent;

    //对象池字典
    Dictionary<string, PoolBase> m_Pools = new Dictionary<string, PoolBase>();

    private void Awake()
    {
        m_PoolParent = this.transform.parent.Find("Pool");
    }
}
