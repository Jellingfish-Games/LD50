using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;

public class BombExplosion : MonoBehaviour
{
	[HideInInspector]
	public LittleGuyInformation info;

	private void Start()
	{
		ConstraintSource src = new ConstraintSource();
		src.sourceTransform = Camera.main.transform;
		src.weight = 1f;

		GetComponentInChildren<RotationConstraint>().AddSource(src);

		transform.GetComponentInChildren<LittleGuyAttackHitbox>(true).attacker = info;
	}
	public void Finish()
	{
		Destroy(gameObject);
	}
}
