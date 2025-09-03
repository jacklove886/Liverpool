using Entities;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIWorldElementManager : MonoSingleton<UIWorldElementManager> {

    public GameObject namePrefab;//角色头上姓名的预制体

    private Dictionary<Transform, GameObject> elements = new Dictionary<Transform, GameObject>();
    

    void Start ()
    {
        
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
        this.elements[owner] = goNameBar;
    }

    public void RemoveCharacterNameBar(Transform owner)
    {
        if (this.elements.ContainsKey(owner))
        {
            Destroy(this.elements[owner]);
            this.elements.Remove(owner);
        }
    }

}
