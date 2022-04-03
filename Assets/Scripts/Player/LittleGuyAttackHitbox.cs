using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LittleGuyAttackHitbox : MonoBehaviour
{
	public LittleGuyInformation attacker;

	private void Start()
	{
		attacker = GetComponentInParent<LittleGuyInformation>();
	}

	private void OnTriggerEnter(Collider other)
	{
		other.gameObject.GetComponent<BossCharacter>()?.TakeDamage(attacker.BattleStats.Damage);
	}
}
