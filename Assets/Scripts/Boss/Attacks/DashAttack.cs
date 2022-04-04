using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(menuName = "Data/Boss/DashAttack", fileName = "DashAttack")]
public class DashAttack : BossAttack
{
    public override IEnumerator PerformAttack(BossCharacter self)
    {
        GameObject attackHitboxes = Instantiate(this.attackHitboxes, self.transform);
        BossAttackHitboxList hitboxList = attackHitboxes.GetComponent<BossAttackHitboxList>();

        self.RestrictControls();
        self.LockInPlace();

        RaycastHit info;
        Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out info, 100, BattleManager.instance.groundMask);
        Vector3 dashTarget = info.point;
        dashTarget.y = 0;

        hitboxList.setDirection = dashTarget - self.transform.position;
        hitboxList.SetCurrentHitbox(1);

        self.state = BossCharacter.BossState.Windup;
        yield return self.WaitForAnim("Boss_Windup");
        yield return new WaitForSeconds(windupTime);


        self.state = BossCharacter.BossState.Attack;

        hitboxList.setDirection = Vector3.zero;
        hitboxList.SetCurrentHitbox(0);
        Vector3 delta = dashTarget - self.transform.position;
        delta.y = 0;
        delta = delta.normalized * 12f;
        if (BattleManager.state == BattleManager.BattleState.Battle) self.UnlockPlace();
        self.velocity = delta;

        CameraManager.i.Shake(1, 1);

        yield return self.WaitForAnim("Boss_Dash");
        //yield return new WaitForSeconds(1);

        Destroy(attackHitboxes);

        self.state = BossCharacter.BossState.Idle;
        if (BattleManager.state == BattleManager.BattleState.Battle) self.EnableControls();
    }
}
