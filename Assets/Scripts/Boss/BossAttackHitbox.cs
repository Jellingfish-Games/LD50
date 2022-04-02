using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossAttackHitbox : MonoBehaviour
{
	private void OnTriggerEnter(Collider other)
	{
		other.gameObject.GetComponent<LittleGuyController>()?.TakeDamage(BattleManager.instance.player.baseProperties.properties.damage);
	}
}
