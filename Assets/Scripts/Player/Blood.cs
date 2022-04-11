using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Blood : MonoBehaviour
{
    public List<Sprite> bloodSprites;

    private SpriteRenderer spriteRenderer;

	void Start()
	{
		spriteRenderer = GetComponent<SpriteRenderer>();

		spriteRenderer.sprite = bloodSprites[Random.Range(0, bloodSprites.Count)];

		transform.rotation = Quaternion.AngleAxis(90f, Vector3.right);
	}
}
