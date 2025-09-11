using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

//[ExecuteInEditMode]在编辑模式下也能运行
[ExecuteInEditMode]
public class SpawnPoint: MonoBehaviour
{
    private Mesh mesh = null;
    public int ID;

    void Start()
    {
        this.mesh = GetComponent<MeshFilter>().sharedMesh;
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        Vector3 pos=this.transform.position+Vector3.up*this.transform.localScale.y*.5f;//以原来圆的顶点为半径
        Gizmos.color = Color.red;
        if (mesh != null)
        {
            Gizmos.DrawWireMesh(mesh, pos, transform.rotation, transform.localScale);
        }
        UnityEditor.Handles.color = Color.red;
        UnityEditor.Handles.ArrowHandleCap(0, transform.position, transform.rotation, 1f, EventType.Repaint);
        UnityEditor.Handles.Label(pos, "SpawnPoint" + this.ID);

    }
#endif
}
