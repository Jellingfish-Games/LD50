using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(menuName = "Data/Boss/DashAttack", fileName = "DashAttack")]
public class DashAttack : BossAttack
{
    public override IEnumerator PerformAttack(BossCharacter self)
    {
        self.RestrictControls();
        self.LockInPlace();

        Debug.Log("MARIO");

        // play dash anim here
        yield return new WaitForSeconds(windupTime);

        RaycastHit info;

        Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out info);

        Vector3 dashTarget = info.point;

        dashTarget.y = 0;

        Vector3 delta = dashTarget - self.transform.position;
        delta.y = 0;

        delta = delta.normalized * 50f;

        self.UnlockPlace();

        self.velocity = delta;

        yield return new WaitForSeconds(0.05f);

        // play stop anim here

        float timeLeft = backswingTime;

        while (timeLeft > 0f)
		{
            self.velocity *= 0.95f;

            timeLeft -= Time.fixedDeltaTime;
            yield return new WaitForFixedUpdate();
		}

        self.EnableControls();
    }
}
