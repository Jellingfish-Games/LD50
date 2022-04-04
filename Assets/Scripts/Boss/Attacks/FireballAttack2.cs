using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(menuName = "Data/Boss/FireballAttack2", fileName = "FireballAttack2")]
public class FireballAttack2 : BossAttack
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

        Vector3 delta = target - self.transform.position;

        self.state = BossCharacter.BossState.Windup;
        yield return self.WaitForAnim("Boss_Windup");
        yield return new WaitForSeconds(windupTime);

        self.state = BossCharacter.BossState.Attack;
        self.UnlockPlace();

        CameraManager.i.Shake(1f, 1);

        for (int i = 0; i < 9; i++)
        {
            var lDirection = new Vector3(Mathf.Sin(Mathf.Deg2Rad * (i - 4.5f) * 20), 0,  Mathf.Cos(Mathf.Deg2Rad * (i - 4.5f) * 20));
            var setDirection = lDirection - self.transform.position;

            var fireball = Instantiate(fireBall).GetComponent<Rigidbody>();

            fireball.transform.localRotation = Quaternion.AngleAxis(Mathf.Atan2(-setDirection.z, setDirection.x) * Mathf.Rad2Deg, Vector3.up);

            fireball.transform.localScale = new Vector3(1, 1, 1);
            fireball.transform.position = self.transform.position + Vector3.up * .5f;

            fireball.GetComponentInChildren<BossAttackHitbox>().damageMultiplier = .5f;

            fireball.velocity = (lDirection + delta).normalized * 4f;

            yield return new WaitForSeconds(.02f);
        }

        yield return new WaitForSeconds(backswingTime);
        //Destroy(attackHitboxes);
        self.state = BossCharacter.BossState.Idle;
        self.EnableControls();
    }
}
