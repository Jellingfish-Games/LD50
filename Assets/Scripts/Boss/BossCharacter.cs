using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Animations;

public class BossCharacter : MonoBehaviour
{
    public enum BossState { Idle, Windup, Attack, Backswing, Dead }


    public BossState state = BossState.Idle;
    public BossPropertyData baseProperties;

    public Animator animator;

    // When the run starts and maybe on phase change, the boss's properties get modified via ApplyPropertyModifier
    private BossProperties modifiedProperties;

    public BossAttack primaryAttack;
    public BossAttack secondaryAttack;

    public float hp;
    public float maxHP => modifiedProperties.maxHP;

    private Vector2 movementInput;

    private Rigidbody rb;

    public bool flip;

    private SpriteRenderer spriteRenderer;

    private bool restrictControls;

    private bool lockInPlace;

    public Vector3 velocity; // won't get modified by engine, easier to slow down on attack gradually

    public int phaseCount = 3;

    public int currentPhase = 1;

    private Coroutine attackCoroutine;

    void Awake()
    {
        modifiedProperties = baseProperties.properties;
        rb = GetComponent<Rigidbody>();
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
    }

    private void Start()
    {
        hp = modifiedProperties.maxHP;
        BattleManager.instance.player = this;

        var RotConst = GetComponentInChildren<RotationConstraint>();
        ConstraintSource source = new ConstraintSource();
        source.sourceTransform = CameraManager.i.cameraBrain.transform;
        source.weight = 1;
        RotConst.SetSource(0, source);
        RotConst.rotationOffset = Vector3.zero;
        RotConst.rotationAtRest = new Vector3(45, 0, 0);
    }

    private void FixedUpdate()
    {
        Move();
    }

    private void Update()
    {
        UpdateDirection();
        UpdateAnimations();
    }

    private void UpdateDirection()
    {
        if (!lockInPlace)
        {
            if (velocity.x > 0)
            {
                spriteRenderer.flipX = false;
                flip = false;
            }
            else if (velocity.x < 0)
            {
                spriteRenderer.flipX = true;
                flip = true;
            }
        }
    }

    public void UpdateDirection(bool dir)
    {
        spriteRenderer.flipX = dir;
        flip = dir;
    }

    public void RestrictControls()
    {
        restrictControls = true;
    }

    public IEnumerator WaitForAnim(string animName)
    {
        if (!animator.GetCurrentAnimatorStateInfo(0).IsName("Boss_Dead"))
        {
            animator.Play(animName);
            yield return null;
            while (animator.GetCurrentAnimatorStateInfo(0).IsName(animName) && animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1)
            {
                yield return null;
            }
        }
    }

    public void PlayAnim(string animName)
    {
        animator.Play(animName);
    }

    public void EnableControls()
    {
        restrictControls = false;
        lockInPlace = false;
    }

    public void LockInPlace()
    {
        lockInPlace = true;
    }

    public void UnlockPlace()
    {
        lockInPlace = false;
    }

    private void Move()
    {
        if (!lockInPlace)
        {
            if (!restrictControls)
            {
                velocity += new Vector3(movementInput.x, 0, movementInput.y) * modifiedProperties.movementSpeed * 0.15f;

                if (velocity.magnitude > modifiedProperties.movementSpeed)
                    velocity = velocity.normalized * modifiedProperties.movementSpeed;

                velocity *= 0.93f;
            }
        }
        else
        {
            velocity *= 0.92f;
        }

        rb.velocity = velocity;

        //if (velocity.magnitude < .5f)
        //{
        //    state = BossState.Idle;
        //} else
        //{
        //    state = BossState.Moving;
        //}
    }

    public void ApplyPropertyModifier(BossProperties addedProps)
    {
        modifiedProperties.Merge(addedProps);
    }

    public void ReplaceAttackInSlot(BossAttack attack, bool isSecondaryAttack)
    {
        if (isSecondaryAttack)
        {
            secondaryAttack = attack;
        }
        else
        {
            primaryAttack = attack;
        }
    }

    public void Input_Move(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            movementInput = context.ReadValue<Vector2>();
        }
        else
        {
            movementInput = Vector2.zero;
        }
    }

    public void Input_PrimaryAttack(InputAction.CallbackContext context)
    {
        if (!restrictControls && context.ReadValue<float>() > 0.5f)
        {
            if (primaryAttack != null)
            {
                attackCoroutine = StartCoroutine(primaryAttack.PerformAttack(this));
            }
        }
    }

    public void Input_SecondaryAttack(InputAction.CallbackContext context)
    {
        if (!restrictControls && context.ReadValue<float>() > 0.5f)
        {
            if (secondaryAttack != null)
            {
                attackCoroutine = StartCoroutine(secondaryAttack.PerformAttack(this));
            }
        }
    }

    public void UpdateAnimations()
    {
        //switch (state)
        //{
        //    case BossState.Moving:
        //        if (!animator.GetCurrentAnimatorStateInfo(0).IsName("Boss_Run"))
        //        {
        //            animator.Play("Boss_Run");
        //        }
        //        break;
        //    default:
        //        if (!animator.GetCurrentAnimatorStateInfo(0).IsName("Boss_Idle"))
        //        {
        //            animator.Play("Boss_Idle");
        //        }
        //        break;
        //}

        if (state == BossState.Dead) return;

        if (state == BossState.Idle && !animator.GetCurrentAnimatorStateInfo(0).IsName("Boss_Hurt") && !animator.GetCurrentAnimatorStateInfo(0).IsName("Boss_Dead"))
        {
            if (velocity.magnitude < .5f)
            {
                if (!animator.GetCurrentAnimatorStateInfo(0).IsName("Boss_Idle"))
                {
                    animator.Play("Boss_Idle");
                }
            }
            else
            {
                if (!animator.GetCurrentAnimatorStateInfo(0).IsName("Boss_Run"))
                {
                    animator.Play("Boss_Run");
                }
            }
        }

    }

    public void TakeDamage(float damage, LittleGuyInformation damageDealer)
    {
        if (hp <= 0)
        {
            return;
        }
        // TODO: Animation

        animator.Play("Boss_Hurt");

        if (hp - damage <= 0 && hp > 0)
        {
            hp -= damage;
            Die(damageDealer);

            SFX.BossDie.Play();

            return;
        }
        else
		{
            SFX.BossGetHit.Play();
		}

        hp -= damage;

        if (hp < (maxHP / phaseCount * (phaseCount - currentPhase)))
		{
            PhaseTransition();

            SFX.BossBarrage.Play();

            currentPhase += 1;
		}
    }

    public void PhaseTransition()
	{
        BattleManager.SwitchToNewState(BattleManager.BattleState.PhaseTransition);
	}

    public void Die(LittleGuyInformation damageDealer)
	{
        // TODO: Play animation

        //state = BossState.Dead;

        //animator.Play("Boss_Death");

        //LockInPlace();
        //RestrictControls();

        //BattleManager.instance.BossDie(damageDealer);
        StartCoroutine(DieCoroutine(damageDealer));
	}

    IEnumerator DieCoroutine(LittleGuyInformation damageDealer)
    {
        LockInPlace();
        RestrictControls();

        StopCoroutine(attackCoroutine);

        foreach (var c in GetComponentsInChildren<BossAttackHitboxList>())
		{
            c.SetCurrentHitbox(-1);
            Destroy(c.gameObject);
		}

        state = BossState.Dead;

        animator.Play("Boss_Death");

        CameraManager.i.Shake(2, 3);

        yield return new WaitForSeconds(3f);

        BattleManager.instance.BossDie(damageDealer);

    }
}
