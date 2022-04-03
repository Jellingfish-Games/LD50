using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LittleGuyController : MonoBehaviour
{
    LittleGuyInformation information;
    [SerializeField] SpriteRenderer hat;
    [SerializeField] SpriteRenderer weapon;

    public LittleGuyAI ai;
    Animator animator;

    public LittleGuyInformation info => information;

    // Start is called before the first frame update
    void Start()
    {
        information = GetComponent<LittleGuyInformation>();
        animator = GetComponentInChildren<Animator>();

        BattleManager.instance.SpawnLittleGuyHealthBar(this);

        SetColors();
        SetHat();
        SetWeapon();
        StartAI();
    }

    public void StartAI()
    {
        ai.StartAIRoutine();
    }

    void SetColors()
    {
        foreach (var i in GetComponentsInChildren<SpriteRenderer>())
        {
            i.material.SetColor("MainColor1", LittleGuyManager.i.palette[information.MetaStats.mainColorID * 5 + information.MetaStats.mainColorValue]);
            i.material.SetColor("MainColor2", (LittleGuyManager.i.palette[information.MetaStats.mainColorID * 5 + information.MetaStats.mainColorValue + 2] + LittleGuyManager.i.palette[information.MetaStats.mainColorID * 5 + information.MetaStats.mainColorValue])/2);
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

    public void TakeDamage(float damage)
	{
        // do something other than log

        information.BattleStats.HP -= damage;

        ai.Hurt();
	}
}
