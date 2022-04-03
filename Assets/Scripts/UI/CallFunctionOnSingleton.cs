using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CallFunctionOnSingleton : MonoBehaviour
{
    public void GlobalManagerStartSingeplayer()
    {
        GlobalManager.instance.StartSingleplayer();
    }
    public void GlobalManagerStartMultiplayer()
    {
        GlobalManager.instance.StartMultiplayer();
    }
}
