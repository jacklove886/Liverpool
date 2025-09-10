using System;
using System.Collections;
using System.Collections.Generic;
using Models;
using UnityEngine;
using UnityEngine.UI;

public class UIQuestInfo : MonoBehaviour {

    public Text title;
    public Text[] targets;
    public Text description;
    public UIBagItemIcon rewardItems;

    public Text rewardMoney;
    public Text rewardExp;


	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    internal void SetQuestInfo(Quest quest)
    {
        throw new NotImplementedException();
    }
}
