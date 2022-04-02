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

    private BossAttack primaryAttack;
    private BossAttack secondaryAttack;

    private Vector2 movementInput;

    private Rigidbody rb;

    void Awake()
    {
        modifiedProperties = baseProperties.properties;
        rb = GetComponent<Rigidbody>();
    }

    private void Start()
    {
        BattleManager.instance.player = this;
    }

    private void Update()
    {
        Move();
    }

    private void Move()
    {
        rb.velocity = new Vector3(movementInput.x, 0, movementInput.y) * modifiedProperties.movementSpeed;
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
        movementInput = context.ReadValue<Vector2>();
    }

    public void Input_PrimaryAttack(InputAction.CallbackContext context)
    {
        if (context.ReadValue<float>() > 0.5f)
        {
            primaryAttack?.PerformAttack(this);
        }
    }

    public void Input_SecondaryAttack(InputAction.CallbackContext context)
    {
        if (context.ReadValue<float>() > 0.5f)
        {
            secondaryAttack?.PerformAttack(this);
        }
    }
}
