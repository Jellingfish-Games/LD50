using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(menuName = "Data/Boss/FireballAttack", fileName = "FireballAttack")]
public class FireballAttack : BossAttack
{
    public GameObject fireBall;
    public override IEnumerator PerformAttack(BossCharacter self)
    {
        GameObject attackHitboxes = Instantiate(this.attackHitboxes, self.transform);
        BossAttackHitboxList hitboxList = attackHitboxes.GetComponent<BossAttackHitboxList>();

        self.RestrictControls();
        self.LockInPlace();

        RaycastHit info;
        Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out info, 100, BattleManager.instance.groundMask);
        Vector3 target = info.point;
        target.y = 0;

        hitboxList.setDirection = target - self.transform.position;
        hitboxList.SetCurrentHitbox(0);

        self.UpdateDirection(target.x < self.transform.position.x);

        self.state = BossCharacter.BossState.Windup;
        yield return self.WaitForAnim("Boss_Cast");
        yield return new WaitForSeconds(windupTime);

        self.state = BossCharacter.BossState.Attack;
        Vector3 delta = target - self.transform.position;
        delta.y = 0;
        delta = delta.normalized * 10f;
        var fireball = Instantiate(fireBall).GetComponent<Rigidbody>();
        var setDirection = target - self.transform.position;
        fireball.transform.localRotation = Quaternion.AngleAxis(Mathf.Atan2(-setDirection.z, setDirection.x) * Mathf.Rad2Deg, Vector3.up);

        fireball.transform.localScale = new Vector3(6, 6, 5);
        fireball.transform.position = self.transform.position + Vector3.up * .5f;
        fireball.velocity = delta;

        fireball.GetComponentInChildren<BossAttackHitbox>().damageMultiplier = 2.5f;

        CameraManager.i.Shake(.5f, 1);


        yield return self.WaitForAnim("Boss_Shoot_Backswing");

        //yield return new WaitForSeconds(backswingTime);
        if (BattleManager.state == BattleManager.BattleState.Battle) self.UnlockPlace();

        Destroy(attackHitboxes);

        self.state = BossCharacter.BossState.Idle;
        if (BattleManager.state == BattleManager.BattleState.Battle) self.EnableControls();
    }
}
