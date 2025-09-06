using Models;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NpcDistanceManager : MonoSingleton<NpcDistanceManager> {

    private List<NpcController> allNpc = new List<NpcController>();

    private float checkInterval = 0.1f;//检测时间0,1秒
    private float lastCheakTime;//上次检测的时间

    public void RegisterNpc(NpcController npc)
    {
        allNpc.Add(npc);//把NPC添加到list列表中
    }

    private void Update()
    {
        if(Time.time-lastCheakTime>= checkInterval)
        {
            if (User.Instance.CurrentCharacterObject != null)
            {
                Vector3 playerPosition = User.Instance.CurrentCharacterObject.transform.position;
                foreach(var npc in allNpc)
                {
                    float sqrDistance = (playerPosition - npc.transform.position).sqrMagnitude;//玩家与npc的距离的平方
                    npc.SetCanInteractive(sqrDistance <= npc.interactiveDistance * npc.interactiveDistance);
                }
            }
            lastCheakTime = Time.time;
        }
    }

}
