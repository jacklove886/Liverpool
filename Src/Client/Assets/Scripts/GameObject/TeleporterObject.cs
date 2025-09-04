using Common.Data;
using Managers;
using Services;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeleporterObject : MonoBehaviour {

    public int ID;
    Mesh mesh = null;

	void Start ()
    {
        mesh = GetComponent<MeshFilter>().sharedMesh;
	}
	
	
	void Update () {
		
	}

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        if (mesh != null)
        {
            Gizmos.DrawWireMesh(mesh, transform.position + Vector3.up * transform.localScale.y * .5f, transform.rotation, transform.localScale);
        }
        UnityEditor.Handles.color = Color.red;
        UnityEditor.Handles.ArrowHandleCap(0, transform.position, transform.rotation, 1f, EventType.Repaint);

    }
#endif

    private void OnTriggerEnter(Collider other)
    {
        PlayerInputController playerController = other.GetComponent<PlayerInputController>();
        if(playerController!=null&& playerController.isActiveAndEnabled)
        {
            TeleporterDefine td = DataManager.Instance.Teleporters[ID];
            if (td == null)
            {
                Debug.LogErrorFormat("角色:{0}进入传送门:{1},但是TeleporterDefine不存在", playerController.character.Info.Name, ID);
                return;
            }
            Debug.LogFormat("角色:{0}进入传送门[{1}:{2}", playerController.character.Info.Name, td.ID, td.Name);
            if (td.LinkTo > 0)
            {
                if (DataManager.Instance.Teleporters.ContainsKey(td.LinkTo))
                {
                    MapService.Instance.SendMapTeleport(ID);
                }
                else
                {
                    Debug.LogErrorFormat("传送门ID:{0} LinkID:{1} 错误!", td.ID, td.LinkTo);
                }          
            }
        }
    }
}
