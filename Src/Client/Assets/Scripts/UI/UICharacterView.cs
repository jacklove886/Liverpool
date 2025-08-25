using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UICharacterView : MonoBehaviour {
    public GameObject[] characters;
    private int currentCharacter = 0;
    public int CurrentCharacter
    {
        get
        {
            return currentCharacter;
        }
        set
        {
            currentCharacter = value;
            UpdateCharacter();
        }
    }

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    void UpdateCharacter()
    {
        for(int i = 0; i < 3; i++)
        {
            //当前角色类型的索引和开启状态对应
            characters[i].SetActive(i == this.currentCharacter);
        }
    }
}
