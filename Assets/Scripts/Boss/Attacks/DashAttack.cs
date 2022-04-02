using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(menuName = "Data/Boss/DashAttack", fileName = "DashAttack")]
public class DashAttack : BossAttack
{
    public override IEnumerator PerformAttack(BossCharacter self)
    {
        GameObject attackHitboxes = Instantiate(this.attackHitboxes, self.transform);
        AttackHitboxList hitboxList = attackHitboxes.GetComponent<AttackHitboxList>();

        self.RestrictControls();
        self.LockInPlace();

        self.state = BossCharacter.BossState.Windup;

        // play dash anim here
        yield return new WaitForSeconds(windupTime);

        RaycastHit info;

        self.state = BossCharacter.BossState.Attack;

        Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out info);

        Vector3 dashTarget = info.point;

        dashTarget.y = 0;

        Vector3 delta = dashTarget - self.transform.position;
        delta.y = 0;

        delta = delta.normalized * 1f;

        self.UnlockPlace();

        hitboxList.SetCurrentHitbox(0);

        self.velocity = delta;

        yield return new WaitForSeconds(0.5f);

        // play stop anim here

        self.state = BossCharacter.BossState.Backswing;

        float timeLeft = backswingTime;

        while (timeLeft > 0f)
		{
            self.velocity *= 0.95f;

            timeLeft -= Time.fixedDeltaTime;
            yield return new WaitForFixedUpdate();
		}

        Destroy(attackHitboxes);

        self.state = BossCharacter.BossState.Moving;

        self.EnableControls();
    }
}
