using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WizardProjectile : MonoBehaviour
{
	public GameObject particleFX;

	[HideInInspector]
	public Vector3 velocity;

	private void Update()
	{
		transform.position += velocity * Time.deltaTime;
	}
}
