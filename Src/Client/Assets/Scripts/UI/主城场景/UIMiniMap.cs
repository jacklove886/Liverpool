using Models;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIMiniMap : MonoBehaviour {

    public Image minimap;
    public Image arrow;
    public Text mapName;

    void Start ()
    {
        mapName.text = User.Instance.CurrentMapData.Name;
	}
	
	
	void Update ()
    {
		
	}
}
