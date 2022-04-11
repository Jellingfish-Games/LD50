using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BombKnocker : MonoBehaviour
{
	private void OnTriggerEnter(Collider other)
	{
		var bomb = other.gameObject.GetComponent<BombObject>();

		if (bomb != null)
		{
			bomb.velocity = (bomb.transform.position - BattleManager.instance.player.transform.position).normalized * 6f;

			SFX.BossPunchHit.Play();
		}
	}
}
