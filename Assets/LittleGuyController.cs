using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LittleGuyController : MonoBehaviour
{
    LittleGuyInformation information;
    [SerializeField] SpriteRenderer hat;
    [SerializeField] SpriteRenderer weapon;

    // Start is called before the first frame update
    void Start()
    {
        information = GetComponent<LittleGuyInformation>();

        SetColors();
        SetHat();
        SetWeapon();
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
        hat.sprite = LittleGuyManager.i.hats[information.MetaStats.hatID];
    }

    void SetWeapon()
    {
        weapon.sprite = LittleGuyManager.i.weapons[information.MetaStats.weaponID];
    }

}
