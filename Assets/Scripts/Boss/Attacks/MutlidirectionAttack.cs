using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

[CreateAssetMenu(menuName = "Data/Boss/MutlidirectionAttack", fileName = "MutlidirectionAttack")]
public class MutlidirectionAttack : BossAttack
{
    public GameObject fireBall;
    public GameObject spinner;
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

        SFX.BossRoar.Play();
        yield return self.WaitForAnim("Boss_Cast");
        yield return new WaitForSeconds(windupTime);
        SFX.BossFire.Play();

        self.state = BossCharacter.BossState.Attack;
        Vector3 delta = target - self.transform.position;
        delta.y = 0;
        delta = delta.normalized * 10f;

        CameraManager.i.Shake(.5f, 1);

        var spinnerObj = Instantiate(spinner).GetComponent<Spinner>();
        spinnerObj.transform.position = self.transform.position;

        for (int i = 0; i<= 8; i++)
        {
            var fireball = Instantiate(fireBall, spinnerObj.transform).GetComponent<Rigidbody>();
            var setDirection = target - self.transform.position;
            //fireball.transform.localRotation = Quaternion.AngleAxis(Mathf.Atan2(-setDirection.z, setDirection.x) * Mathf.Rad2Deg + 90 + i * 10, Vector3.up);

            fireball.transform.localPosition = (new Vector3(Mathf.Sin(Mathf.Deg2Rad * i * 45), 0, Mathf.Cos(Mathf.Deg2Rad * i * 45))).normalized * 2.5f;

            //fireball.velocity = (new Vector3(Mathf.Sin(Mathf.Deg2Rad * i * 45), 0, Mathf.Cos(Mathf.Deg2Rad * i * 45))).normalized * 4f;

            fireball.GetComponentInChildren<BossAttackHitbox>().damageMultiplier = .3f;

            fireball.GetComponentInChildren<Autokill>().killOnPillarContact = false;


            fireball.transform.localScale *= .35f;

            fireball.transform.parent = spinnerObj.transform;
        }

        yield return self.WaitForAnim("Boss_Roar");


        //yield return new WaitForSeconds(backswingTime);
        if (BattleManager.state == BattleManager.BattleState.Battle) self.UnlockPlace();

        Destroy(attackHitboxes);
        self.state = BossCharacter.BossState.Idle;
        if (BattleManager.state == BattleManager.BattleState.Battle) self.EnableControls();
    }
}
