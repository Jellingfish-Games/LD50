using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossWarningHitbox : MonoBehaviour
{
	private void OnTriggerEnter(Collider other)
	{
		other.gameObject.GetComponent<LittleGuyAI>()?.Warn(transform.position);
	}
}
