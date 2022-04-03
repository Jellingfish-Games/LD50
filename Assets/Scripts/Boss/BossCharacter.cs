using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Animations;

public class BossCharacter : MonoBehaviour
{
    public enum BossState { Idle,Moving, Windup, Attack, Backswing }


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

    public void RestrictControls()
	{
        restrictControls = true;
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
                velocity = new Vector3(movementInput.x, 0, movementInput.y) * modifiedProperties.movementSpeed;
        }
        else
		{
            velocity *= 0.999f;
		}

        rb.velocity = velocity;

        if (velocity.magnitude < .1f)
        {
            state = BossState.Idle;
        } else
        {
            state = BossState.Moving;
        }
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
        if (!restrictControls)
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
                StartCoroutine(primaryAttack.PerformAttack(this));
			}
        }
    }

    public void Input_SecondaryAttack(InputAction.CallbackContext context)
    {
        if (!restrictControls && context.ReadValue<float>() > 0.5f)
        {
            if (secondaryAttack != null)
            {
                StartCoroutine(secondaryAttack.PerformAttack(this));
            }
        }
    }

    public void UpdateAnimations()
    {
        switch (state)
        {
            case BossState.Moving:
                if (!animator.GetCurrentAnimatorStateInfo(0).IsName("Boss_Run"))
                {
                    animator.Play("Boss_Run");
                }
                break;
            default:
                if (!animator.GetCurrentAnimatorStateInfo(0).IsName("Boss_Idle"))
                {
                    animator.Play("Boss_Idle");
                }
                break;
        }
    }
}
