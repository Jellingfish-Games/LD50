using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class BossCharacter : MonoBehaviour
{
    public BossPropertyData baseProperties;

    public Animator animator;

    // When the run starts and maybe on phase change, the boss's properties get modified via ApplyPropertyModifier
    private BossProperties modifiedProperties;

    public BossAttack primaryAttack;
    public BossAttack secondaryAttack;

    private Vector2 movementInput;

    private Rigidbody rb;

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
        BattleManager.instance.player = this;
    }

    private void Update()
    {
        Move();
        UpdateDirection();
    }
    
    private void UpdateDirection()
	{
        if (movementInput.x > 0)
		{
            spriteRenderer.flipX = true;
		}
        else if (movementInput.x < 0)
		{
            spriteRenderer.flipX = false;
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
            velocity *= 0.95f;
		}

        rb.velocity = velocity;
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
}
