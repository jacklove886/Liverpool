using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameObjectPool : PoolBase
{
    //取出对象
    public override Object Spwan(string name)
    {
        Object obj = base.Spwan(name);
        if (obj == null)
        {
            return null;
        }
        GameObject go = obj as GameObject;
        go.SetActive(true);//设置激活状态
        return obj;
    }

    //回收对象
    public override void UnSpawn(string name, Object obj)
    {
        GameObject go = obj as GameObject;
        go.SetActive(false);
        go.transform.SetParent(this.transform, false);
        base.UnSpawn(name, obj);
    }

    public override void Release()
    {
        base.Release();
        foreach(PoolObject item in m_Objects)
        {
            //时间到了没有使用就释放
            if(System.DateTime.Now.Ticks - item.LastUseTime.Ticks >= m_ReleaseTime * 10000000)
            {
                Debug.Log("GameObjectPool  Release Time"+ System.DateTime.Now);
                Destroy(item.Object);
                Manager.Resource.MinusBundleCount(item.Name);
                m_Objects.Remove(item);
                Release();//递归
                return;
            }
        }
    }
}
