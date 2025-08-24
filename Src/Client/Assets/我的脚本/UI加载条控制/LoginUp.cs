using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoginUp : MonoBehaviour {

    public Slider slider;
    public Text text;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        Updatejindu();
	}
    void Updatejindu()
    {
        text.text = slider.value + "%";
    }
}
