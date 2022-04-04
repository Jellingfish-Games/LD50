using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossAttackHitbox : MonoBehaviour
{
	public float damageMultiplier = 1;
	public float knockBackStrength = 0f;
	private void OnTriggerEnter(Collider other)
	{
		other.gameObject.GetComponent<LittleGuyController>()?.TakeDamage(BattleManager.instance.player.baseProperties.properties.damage * damageMultiplier);

		if (knockBackStrength != 0f)
		{
			other.gameObject.GetComponent<LittleGuyController>()?.ai.ApplyKnockback((other.gameObject.transform.position - transform.position).normalized * knockBackStrength);
		}
	}
}
