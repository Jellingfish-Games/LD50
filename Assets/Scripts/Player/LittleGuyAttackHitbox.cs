using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LittleGuyAttackHitbox : MonoBehaviour
{
	public LittleGuyInformation attacker;
	public float damageScale = 1f;
	private void Start()
	{
		var newAttacker = GetComponentInParent<LittleGuyInformation>();

		if (newAttacker != null)
			attacker = newAttacker;
	}

	private void OnTriggerEnter(Collider other)
	{
		other.gameObject.GetComponent<BossCharacter>()?.TakeDamage(attacker.BattleStats.Damage * damageScale);
	}
}
