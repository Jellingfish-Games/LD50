using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LittleGuyAttackHitbox : MonoBehaviour
{
	public LittleGuyInformation attacker;
	public float damageScale = 1f;
	public float flatDamage = 0f;

	public bool playOnHit = false;
	public SFX soundOnHit;

	private void Start()
	{
		var newAttacker = GetComponentInParent<LittleGuyInformation>();

		if (newAttacker != null)
			attacker = newAttacker;
	}

	private void OnTriggerEnter(Collider other)
	{
		var boss = other.gameObject.GetComponent<BossCharacter>();

		if (boss != null)
		{
			boss.TakeDamage(flatDamage + attacker.BattleStats.Damage * damageScale, attacker);
			if (playOnHit)
				soundOnHit.Play();
		}
	}
}
