using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(menuName = "Data/Boss/TestAttack", fileName = "TestAttack")]
public class BossAttack_Test : BossAttack
{
    public override IEnumerator PerformAttack(BossCharacter self)
    {
        yield return null;
    }
}
