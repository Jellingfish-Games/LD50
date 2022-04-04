using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossAttackHitbox : MonoBehaviour
{
	public float damageMultiplier = 1;
	public float knockBackStrength = 0f;

	public bool playOnHit = false;
	public SFX soundOnHit;
	private void OnTriggerEnter(Collider other)
	{
		var guy = other.gameObject.GetComponent<LittleGuyController>();
		
		if (guy != null)
		{
			guy.TakeDamage(BattleManager.instance.player.baseProperties.properties.damage * damageMultiplier);

			if (playOnHit)
				soundOnHit.Play();

			if (knockBackStrength != 0f)
			{
				guy.ai.ApplyKnockback((other.gameObject.transform.position - transform.position).normalized * knockBackStrength);
			}
		}
	}
}
