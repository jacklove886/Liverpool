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
        npcStatusPrefab.SetActive(false);
    }


    void Update ()
    {
        
    }

    public void AddCharacterNameBar(Transform owner,Character character)
    {
        GameObject goNameBar = Instantiate(namePrefab, this.transform);
        goNameBar.name = character.Name;

        UIWorldElement worldElement = goNameBar.GetComponent<UIWorldElement>();
        worldElement.owner = owner;
        worldElement.Camera = Camera.main.transform;
        worldElement.SetHeight(character);

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
            GameObject goNpcBar = Instantiate(npcStatusPrefab, this.transform);
            goNpcBar.name = owner.name;

            UIWorldElement worldElement = goNpcBar.GetComponent<UIWorldElement>();
            worldElement.owner = owner;

            goNpcBar.GetComponent<UIQuestStatus>().SetQuestStatus(status);
            goNpcBar.SetActive(true);
            this.elementsStatus[owner] = goNpcBar;
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
