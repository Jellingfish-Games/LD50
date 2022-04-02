using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossCharacter : MonoBehaviour
{
    public BossPropertyData baseProperties;

    // When the run starts and maybe on phase change, the boss's properties get modified via ApplyPropertyModifier
    private BossProperties modifiedProperties;

    private BossAttack primaryAttack;
    private BossAttack secondaryAttack;

    void Awake()
    {
        modifiedProperties = baseProperties.properties;
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
}
