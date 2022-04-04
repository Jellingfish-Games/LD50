using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(menuName = "Data/Boss/FireballAttack", fileName = "FireballAttack")]
public class FireballAttack : BossAttack
{
    public GameObject fireBall;
    public override IEnumerator PerformAttack(BossCharacter self)
    {
        self.RestrictControls();
        self.LockInPlace();

        RaycastHit info;
        Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out info, 100, BattleManager.instance.groundMask);
        Vector3 target = info.point;
        target.y = 0;

        self.state = BossCharacter.BossState.Windup;
        yield return self.WaitForAnim("Boss_Windup");
        yield return new WaitForSeconds(windupTime);

        self.state = BossCharacter.BossState.Attack;
        Vector3 delta = target - self.transform.position;
        delta.y = 0;
        delta = delta.normalized * 10f;
        self.UnlockPlace();
        var fireball = Instantiate(fireBall).GetComponent<Rigidbody>();
        var setDirection = target - self.transform.position;
        fireball.transform.localRotation = Quaternion.AngleAxis(Mathf.Atan2(-setDirection.z, setDirection.x) * Mathf.Rad2Deg, Vector3.up);

        fireball.transform.localScale = new Vector3(4, 4, 3);
        fireball.transform.position = self.transform.position + Vector3.up * .5f;
        fireball.velocity = delta;

        fireball.GetComponentInChildren<BossAttackHitbox>().damageMultiplier = 2.5f;

        CameraManager.i.Shake(.5f, 1);

        yield return new WaitForSeconds(backswingTime);
        //Destroy(attackHitboxes);
        self.state = BossCharacter.BossState.Idle;
        self.EnableControls();
    }
}
