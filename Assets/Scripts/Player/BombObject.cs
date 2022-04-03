using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;

public class BombObject : MonoBehaviour
{
    public BombExplosion explosion;

	[HideInInspector]
	public Vector3 velocity;

	[HideInInspector]
	public LittleGuyInformation info;

	private void Start()
	{
		ConstraintSource src = new ConstraintSource();
		src.sourceTransform = Camera.main.transform;
		src.weight = 1f;

		GetComponentInChildren<RotationConstraint>().AddSource(src);
	}

	public void Explode()
	{
		CameraManager.i.Shake(1f, 2);
		BombExplosion bomb = Instantiate(explosion, transform.position, Quaternion.identity);
		bomb.info = info;
		Destroy(gameObject);
	}

	private void Update()
	{
		transform.position += velocity * Time.deltaTime;
	}

	private void FixedUpdate()
	{
		velocity *= 0.98f;
	}
}
