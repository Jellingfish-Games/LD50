using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossAttackHitbox : MonoBehaviour
{
	public float damageMultiplier = 1;
	private void OnTriggerEnter(Collider other)
	{
		other.gameObject.GetComponent<LittleGuyController>()?.TakeDamage(BattleManager.instance.player.baseProperties.properties.damage * damageMultiplier);
	}
}
