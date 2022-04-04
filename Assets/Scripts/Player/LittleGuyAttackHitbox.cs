using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LittleGuyAttackHitbox : MonoBehaviour
{
	public LittleGuyInformation attacker;
	public float damageScale = 1f;
	public float flatDamage = 0f;
	private void Start()
	{
		var newAttacker = GetComponentInParent<LittleGuyInformation>();

		if (newAttacker != null)
			attacker = newAttacker;
	}

	private void OnTriggerEnter(Collider other)
	{
		other.gameObject.GetComponent<BossCharacter>()?.TakeDamage(flatDamage + attacker.BattleStats.Damage * damageScale);
	}
}
