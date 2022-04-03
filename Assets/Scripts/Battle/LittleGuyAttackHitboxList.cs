using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LittleGuyAttackHitboxList : MonoBehaviour
{
    public List<GameObject> attackHitboxesInOrder;

	private LittleGuyAI character;

    void Start()
	{
		if (attackHitboxesInOrder == null)
			attackHitboxesInOrder = new List<GameObject>();

		character = GetComponentInParent<LittleGuyAI>();
	}

	private void Update()
	{
		transform.localScale = character.flip ? new Vector3(-1, 1, 1) : Vector3.one;
	}

	public void SetCurrentHitbox(int index)
	{
        for (int i = 0; i < attackHitboxesInOrder.Count; i++)
		{
            if (i == index)
			{
                attackHitboxesInOrder[i].SetActive(true);
			}
            else
			{
                attackHitboxesInOrder[i].SetActive(false);
			}
		}
	}
}
