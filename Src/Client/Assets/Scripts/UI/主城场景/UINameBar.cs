using Entities;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UINameBar : MonoBehaviour {

    public Text characterName;
    public Image image;
    public Character character;	
	
	void Update ()
    {
        UpdateInfo();
    }

    void UpdateInfo()
    {
        if (this.character != null)
        {
            string name = character.Name + "  " + character.Info.Level+"级";
            if(name!= characterName.text&& characterName.text!=null)
            {
                characterName.text = name;
            }
        }
    }
}
