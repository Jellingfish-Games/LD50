using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(menuName = "Data/Boss/DashAttack", fileName = "DashAttack")]
public class DashAttack : BossAttack
{
    public override IEnumerator PerformAttack(BossCharacter self)
    {
        self.RestrictControls();

        Debug.Log("MARIO");

        // play dash anim here
        yield return new WaitForSeconds(windupTime);

        Vector3 dashTarget = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        dashTarget.y = 0;

        Vector3 delta = dashTarget - self.transform.position;

        delta = delta.normalized * 500f;

        self.velocity = delta;

        yield return new WaitForSeconds(1f);

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
