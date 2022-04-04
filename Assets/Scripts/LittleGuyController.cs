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
    private bool entering;

    public LittleGuyInformation info => information;
    private bool canSpeak = true;

    private void Awake()
	{
        information = GetComponent<LittleGuyInformation>();
    }

    // Start is called before the first frame update
    void Start()
    {
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
        yield return transform.DOMove(BattleManager.instance.littleGuySpawnPosition2.position, 3).SetEase(Ease.Linear).WaitForCompletion();
        animator.Play("Guy_Intro");
        yield return new WaitForSeconds(1f);
        BattleManager.instance.nameDisplay.StartDisplayAnimation(info);

        yield return new WaitForSeconds(2f);
        //TITLE NAME OF GUY
        BattleManager.instance.LittleGuyQuote(this, information.enterQuotes);

        StartCoroutine(SpeechCooldown());

        yield return new WaitForSeconds(3f);
        animator.Play("Guy_Run");
        yield return transform.DOMove(BattleManager.instance.littleGuySpawnPosition3.position, 1).SetEase(Ease.Linear).WaitForCompletion();
        animator.Play("Guy_Idle");
        yield return new WaitForSeconds(.5f);
        //yield return new WaitForSeconds(4f);
        //yield return MoveToDirection(-transform.forward * 8);
        //yield return MoveToDirection(BattleManager.instance.littleGuySpawnPosition2.position);
        StartAI();
        BattleManager.instance.SpawnLittleGuyHealthBar(this);
    }

    public IEnumerator ShortEnterCoroutine()
    {
        //navMeshAgent.enabled = false;
        var animator = GetComponentInChildren<Animator>();
        animator.Play("Guy_Run");
        yield return transform.DOMove(BattleManager.instance.littleGuySpawnPosition2.position, 2).SetEase(Ease.Linear).WaitForCompletion();
        animator.Play("Guy_Intro");
        yield return new WaitForSeconds(0.8f);
        BattleManager.instance.nameDisplay.StartDisplayAnimation(info);

        //yield return new WaitForSeconds(2f);
        //TITLE NAME OF GUY
        BattleManager.instance.LittleGuyQuote(this, information.comeBackQuotes);

        StartCoroutine(SpeechCooldown());

        yield return new WaitForSeconds(1f);
        animator.Play("Guy_Run");
        yield return transform.DOMove(BattleManager.instance.littleGuySpawnPosition3.position, 1).SetEase(Ease.Linear).WaitForCompletion();
        animator.Play("Guy_Idle");
        yield return new WaitForSeconds(.5f);
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

        if (entering)
		{
            return;
		}

        information.BattleStats.HP -= damage;

        if (information.BattleStats.HP <= 0)
        {
            ai.Die();
            BattleManager.instance.LittleGuyDie(this);

            if (BattleManager.instance.littleGuys.Count == 0)
                BattleManager.instance.littleGuyDeathBanner.Show();
        }
        else
        {
            if (Random.Range(0f, 1f) < 0.1f && canSpeak)
			{
				BattleManager.instance.LittleGuyQuote(this, information.hurtQuotes);
                StartCoroutine(SpeechCooldown());
			}
			ai.Hurt();
        }
	}

    IEnumerator SpeechCooldown()
    {
        canSpeak = false;

        float time = 5f;

        yield return new WaitForSeconds(time);

        canSpeak = true;

        yield return null;
    }
}
