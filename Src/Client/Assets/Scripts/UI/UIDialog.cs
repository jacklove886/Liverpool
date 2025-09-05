using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIDialog : UIWindow{

    public Text title;
	
	void Start () {
		
	}
	
	
	void Update () {
		
	}

    public void SetTitle(string title)
    {
        this.title.text = title;
    }
}
