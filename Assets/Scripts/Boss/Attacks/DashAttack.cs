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
        hitboxList.SetCurrentHitbox(1);

        self.state = BossCharacter.BossState.Windup;
        yield return self.WaitForAnim("Boss_Windup");
        yield return new WaitForSeconds(windupTime);

        self.state = BossCharacter.BossState.Attack;
        hitboxList.SetCurrentHitbox(0);
        Vector3 delta = dashTarget - self.transform.position;
        delta.y = 0;
        delta = delta.normalized * 12f;
        self.UnlockPlace();
        self.velocity = delta;

        CameraManager.i.Shake(1, 1);

        yield return self.WaitForAnim("Boss_Dash");
        //yield return new WaitForSeconds(1);

        Destroy(attackHitboxes);

        self.state = BossCharacter.BossState.Idle;
        self.EnableControls();

        //       play dash anim here
        //      yield return new WaitForSeconds(windupTime);

        //      RaycastHit info;

        //      self.state = BossCharacter.BossState.Attack;

        //      Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out info, 100, BattleManager.instance.groundMask);

        //      Vector3 dashTarget = info.point;

        //      dashTarget.y = 0;

        //      Vector3 delta = dashTarget - self.transform.position;
        //      delta.y = 0;

        //      delta = delta.normalized * 40f;

        //      self.UnlockPlace();

        //      hitboxList.SetCurrentHitbox(0);

        //      self.velocity = delta;

        //      yield return new WaitForSeconds(0.03f);

        //       play stop anim here

        //      self.state = BossCharacter.BossState.Backswing;

        //      float timeLeft = backswingTime;

        //      while (timeLeft > 0f)
        //{
        //          self.velocity *= 0.95f;

        //          timeLeft -= Time.fixedDeltaTime;
        //          yield return new WaitForFixedUpdate();
        //}

        //      Destroy(attackHitboxes);

        //      self.state = BossCharacter.BossState.Moving;

        // self.EnableControls();
    }
}
