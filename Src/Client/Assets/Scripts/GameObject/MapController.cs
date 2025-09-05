using Managers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapController : MonoBehaviour {

    public Collider boxBouding;

	void Start ()
    {
        MinimapManager.Instance.UpdateMinimap(boxBouding);
	}
	
}
