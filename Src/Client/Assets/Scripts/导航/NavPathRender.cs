using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Entities;
using SkillBridge.Message;
using Models;
using Managers;
using Services;
using UnityEngine.AI;

public class NavPathRender : MonoSingleton<NavPathRender>
{
    private LineRenderer pathRenderer;
    private NavMeshPath path;

    private void Start()
    {
        pathRenderer = GetComponent<LineRenderer>();
        pathRenderer.enabled = false;
    }

    public void SetPath(NavMeshPath path,Vector3 target)
    {
        this.path = path;
        if (path == null)
        {
            pathRenderer.enabled = false;
            pathRenderer.positionCount = 0;
        }
        else
        {
            pathRenderer.enabled = true;
            pathRenderer.positionCount = path.corners.Length+1;//corner不包含终点
            pathRenderer.SetPositions(path.corners);//设置所有点
            pathRenderer.SetPosition(pathRenderer.positionCount - 1, target);//设置每个点的位置
            for (int i=0;i< pathRenderer.positionCount; i++)
            {
                pathRenderer.SetPosition(i,pathRenderer.GetPosition(i)+Vector3.up*0.2f);//给每个点加一点偏移 浮在空中
            }
        }
    }
}
