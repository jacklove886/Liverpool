using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class ButtonHighLight : MonoBehaviour {

    public Image button;
    public bool Selected
    {
        get { return button.IsActive(); }
        set
        {
            button.gameObject.SetActive(value);
        }
    }
    // Use this for initialization
    void Start () {
	}
	
	// Update is called once per frame
	void Update () {
		
	}

}
