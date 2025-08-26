using SkillBridge.Message;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UICharacterView : MonoBehaviour {

    public SkillBridge.Message.NCharacterInfo info;
    public Transform displayRoot;           // 放模型的父节点
    public GameObject warriorPrefab;        // 战士预制体
    public GameObject wizardPrefab;         // 法师预制体
    public GameObject archerPrefab;         // 游侠预制体
    private GameObject currentInstance;
    public Image[] images;
    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}


    public void UpdateCharacter(NCharacterInfo info)
    {
        if (currentInstance != null)
        {
            Destroy(currentInstance);
            currentInstance = null;
        }
        GameObject prefab = null;
        switch (info.Class)
        {
            case CharacterClass.Warrior: prefab = warriorPrefab; { images[0].gameObject.SetActive(true); images[1].gameObject.SetActive(false); images[2].gameObject.SetActive(false); break; }
            case CharacterClass.Wizard: prefab = wizardPrefab; { images[0].gameObject.SetActive(false); images[1].gameObject.SetActive(true); images[2].gameObject.SetActive(false); break; }
            case CharacterClass.Archer: prefab = archerPrefab; { images[0].gameObject.SetActive(false); images[1].gameObject.SetActive(false); images[2].gameObject.SetActive(true); break; }
        }
        // 3. 实例化到展示点
        warriorPrefab.SetActive(false);
        currentInstance = Instantiate(prefab, displayRoot, false);
        currentInstance.SetActive(true);
        currentInstance.transform.localPosition = Vector3.zero;
        currentInstance.transform.localRotation = Quaternion.identity;
        currentInstance.transform.localScale = Vector3.one;
    }
}
