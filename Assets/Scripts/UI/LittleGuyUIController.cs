using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LittleGuyUIController : MonoBehaviour
{
    public LittleGuyInformation information;
    [SerializeField] Image hat;
    [SerializeField] Image weapon;

    public LittleGuyInformation info => information;

    private void Awake()
    {
        information = GetComponent<LittleGuyInformation>();
    }

    // Start is called before the first frame update
    void Start()
    {
        SetColors();
        SetHat();
        SetWeapon();

    }

    public void UpdateInfo()
    {
        SetColors();
        SetHat();
        SetWeapon();
    }

    void SetColors()
    {
        foreach (var i in GetComponentsInChildren<Image>())
        {
            i.material = Instantiate(i.material);
            i.material.SetColor("MainColor1", LittleGuyManager.i.palette[information.MetaStats.mainColorID * 5 + information.MetaStats.mainColorValue]);
            i.material.SetColor("MainColor2", (LittleGuyManager.i.palette[information.MetaStats.mainColorID * 5 + information.MetaStats.mainColorValue + 2] + LittleGuyManager.i.palette[information.MetaStats.mainColorID * 5 + information.MetaStats.mainColorValue]) / 2);
            i.material.SetColor("MainColor3", LittleGuyManager.i.palette[information.MetaStats.mainColorID * 5 + information.MetaStats.mainColorValue + 2]);
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
