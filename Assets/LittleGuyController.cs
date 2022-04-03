using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;
using DG.Tweening;

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

        //BattleManager.instance.SpawnLittleGuyHealthBar(this);

        SetColors();
        SetHat();
        SetWeapon();
        //StartAI();

        var RotConst = GetComponentInChildren<RotationConstraint>();
        ConstraintSource source = new ConstraintSource();
        source.sourceTransform = CameraManager.i.cameraBrain.transform;
        source.weight = 1;
        RotConst.SetSource(0, source);
        RotConst.rotationOffset = Vector3.zero;
        RotConst.rotationAtRest = new Vector3(45, 0, 0);

        //StartCoroutine(EnterCoroutine());

    }

    public IEnumerator EnterCoroutine()
    {
        //navMeshAgent.enabled = false;
        var animator = GetComponentInChildren<Animator>();
        animator.Play("Guy_Run");
        yield return transform.DOMove(BattleManager.instance.littleGuySpawnPosition2.position, 3).WaitForCompletion();
        animator.Play("Guy_Intro");
        yield return new WaitForSeconds(4);
        //TITLE NAME OF GUY
        BattleManager.instance.LittleGuyQuote(this, information.enterQuotes);
        animator.Play("Guy_Run");
        yield return transform.DOMove(BattleManager.instance.littleGuySpawnPosition3.position, 3).WaitForCompletion();
       
        //yield return new WaitForSeconds(4f);
        //yield return MoveToDirection(-transform.forward * 8);
        //yield return MoveToDirection(BattleManager.instance.littleGuySpawnPosition2.position);
        StartAI();
        BattleManager.instance.SpawnLittleGuyHealthBar(this);
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
        if (information.BattleStats.HP <= 0)
        {
            return;
        }

        information.BattleStats.HP -= damage;

        if (information.BattleStats.HP <= 0)
        {
            ai.Die();
            BattleManager.instance.LittleGuyDie(this);
        } else
        {
            ai.Hurt();
        }
	}
}
