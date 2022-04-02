using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LittleGuyController : MonoBehaviour
{
    LittleGuyInformation information;
    SpriteRenderer hat;
    SpriteRenderer weapon;

    // Start is called before the first frame update
    void Start()
    {
        information = GetComponent<LittleGuyInformation>();


        SetColors();
    }

    void SetColors()
    {
        foreach (var i in GetComponentsInChildren<SpriteRenderer>())
        {
            i.material.SetColor("MainColor", information.MetaStats.mainColor);
            i.material.SetColor("SkinColor", information.MetaStats.skinColor);
        }
    }

    void SetHat()
    {

    }

}
