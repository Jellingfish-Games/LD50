using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackReplacementElement : MonoBehaviour
{
    public BossAttackUIElement leftElem;
    public BossAttackUIElement rightElem;

    public void Initialize(BossAttack oldAttack, BossAttack newAttack)
    {
        leftElem.attack = oldAttack;
        rightElem.attack = newAttack;
    }
    public void Select()
    {
        BattleManager.instance.ReplaceAttackSlot(leftElem.attack, rightElem.attack);
    }
}
