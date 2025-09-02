using Models;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIMainCity : MonoBehaviour {

    public Text myName;
    public Text myLevel;

    void Start () {
        UpdataAvatar();
	}
	
	void Update () {
		
	}

    void UpdataAvatar()
    {
        myName.text = User.Instance.CurrentCharacter.Name;
        myLevel.text = User.Instance.CurrentCharacter.Level.ToString() ;
    }
}
