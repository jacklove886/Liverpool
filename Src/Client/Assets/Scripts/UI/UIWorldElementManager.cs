using Entities;
using Managers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIWorldElementManager : MonoSingleton<UIWorldElementManager> {

    public GameObject namePrefab;//角色头上姓名的预制体
    public GameObject npcStatusPrefab;//NPC头上姓名的预制体

    private Dictionary<Transform, GameObject> elementsNames = new Dictionary<Transform, GameObject>();
    private Dictionary<Transform, GameObject> elementsStatus = new Dictionary<Transform, GameObject>();


    protected override void OnStart()
    {
        namePrefab.SetActive(false);
    }


    void Update ()
    {
        
    }

    public void AddCharacterNameBar(Transform owner,Character character)
    {
        GameObject goNameBar = Instantiate(namePrefab, this.transform);
        goNameBar.name = "角色" + character.Name;
        goNameBar.GetComponent<UINameBar>().owner = owner;
        goNameBar.GetComponent<UINameBar>().character = character;
        goNameBar.SetActive(true);
        this.elementsNames[owner] = goNameBar;
    }

    public void RemoveCharacterNameBar(Transform owner)
    {
        if (this.elementsNames.ContainsKey(owner))
        {
            Destroy(this.elementsNames[owner]);
            this.elementsNames.Remove(owner);
        }
    }

    public void AddNpcQuestStatus(Transform owner, NpcQuestStatus status)
    {
        if (this.elementsStatus.ContainsKey(owner))
        {
            elementsStatus[owner].GetComponent<UIQuestStatus>().SetQuestStatus(status);
        }
        else
        {
            GameObject go = Instantiate(npcStatusPrefab, this.transform);
            go.name = "NPC状态" + owner.name;
            go.GetComponent<UINameBar>().owner = owner;
            go.GetComponent<UIQuestStatus>().SetQuestStatus(status);
            go.SetActive(true);
            this.elementsStatus[owner] = go;
        }      
    }

    public void RemoveNpcQuestStatus(Transform owner)
    {
        if (this.elementsStatus.ContainsKey(owner))
        {
            Destroy(this.elementsStatus[owner]);
            this.elementsStatus.Remove(owner);
        }
    }

}
