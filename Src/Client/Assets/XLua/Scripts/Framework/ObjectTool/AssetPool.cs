using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AssetPool : PoolBase
{
    public override Object Spwan(string name)
    {
        return base.Spwan(name);
    }

    public override void UnSpawn(string name, Object obj)
    {
        base.UnSpawn(name, obj);
    }

    public override void Release()
    {
        base.Release();
        foreach(PoolObject item in m_Objects)
        {
            if(System.DateTime.Now.Ticks-item.LastUseTime.Ticks>=m_ReleaseTime* 10000000)
            {
                Debug.Log("AssetPool  Release Time" + System.DateTime.Now);
                Manager.Resource.UnloadBundle(item.Name);
                m_Objects.Remove(item);
                Release();
                return;
            }
        }
    }

}
