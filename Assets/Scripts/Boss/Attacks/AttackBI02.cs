using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(menuName = "Data/Boss/AttackBI02", fileName = "AttackBI02")]
public class AttackBI02 : BossAttack
{
    public GameObject fireBall;
    public GameObject spinner;
    public override IEnumerator PerformAttack(BossCharacter self)
    {
        GameObject attackHitboxes = Instantiate(this.attackHitboxes, self.transform);
        BossAttackHitboxList hitboxList = attackHitboxes.GetComponent<BossAttackHitboxList>();

        self.RestrictControls();
        self.LockInPlace();

        hitboxList.SetCurrentHitbox(0);

        RaycastHit info;
        Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out info, 100, BattleManager.instance.groundMask);
        Vector3 target = info.point;
        target.y = 0;

        self.state = BossCharacter.BossState.Windup;


        yield return self.WaitForAnim("Boss_Cast");

        self.state = BossCharacter.BossState.Attack;

        SFX.BossFireballHit.Play();

        CameraManager.i.Shake(.75f, .5f);

        var spinnerObj = Instantiate(spinner).GetComponent<Spinner>();
        spinnerObj.spinSprite = false;
        spinnerObj.spinSpeed = 0f;
        spinnerObj.GetComponent<Autokill>().killAfter = 1f;
        spinnerObj.transform.position = self.transform.position;

        var setDirection = target - self.transform.position;
        spinnerObj.transform.localRotation = Quaternion.AngleAxis(Mathf.Atan2(-setDirection.z, setDirection.x) * Mathf.Rad2Deg - 45, Vector3.up);

        var fireball = Instantiate(fireBall, spinnerObj.transform).GetComponent<Rigidbody>();

        fireball.GetComponentInChildren<BossAttackHitbox>().damageMultiplier = 2f;

        fireball.transform.localPosition = Vector3.zero;

        yield return new WaitForSeconds(.5f);

        SFX.BossSlashSwing.Play();
        spinnerObj.spinSpeed = 17f;

        CameraManager.i.Shake(.5f, 1);

        yield return self.WaitForAnim("Boss_Roar");

        //yield return new WaitForSeconds(backswingTime);
        if (BattleManager.state == BattleManager.BattleState.Battle) self.UnlockPlace();

        Destroy(attackHitboxes);
        self.state = BossCharacter.BossState.Idle;
        if (BattleManager.state == BattleManager.BattleState.Battle) self.EnableControls();
    }
}
