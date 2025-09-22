using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameStart : MonoBehaviour
{
    public GameMode GameMode;

    private void Start()
    {
        AppConst.GameMode = this.GameMode;
    }
}
