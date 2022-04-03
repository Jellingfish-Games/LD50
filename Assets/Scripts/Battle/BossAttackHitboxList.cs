using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BossAttackHitboxList : MonoBehaviour
{
    public List<GameObject> attackHitboxesInOrder;

    // Whether to rotate the hitbox stack in the direction of movement, or just horizontal flip
    public bool enableDirectionality;
	public Vector3 setDirection;

	private BossCharacter character;

    void Start()
	{
		if (attackHitboxesInOrder == null)
			attackHitboxesInOrder = new List<GameObject>();

		character = BattleManager.instance.player;
	}

	private void Update()
	{
		if (enableDirectionality)
		{
			if (setDirection != Vector3.zero)
			{
				transform.localRotation = Quaternion.AngleAxis(Mathf.Atan2(-setDirection.z, setDirection.x) * Mathf.Rad2Deg, Vector3.up);

			}
			else
			{
				transform.localRotation = Quaternion.AngleAxis(Mathf.Atan2(-character.velocity.z, character.velocity.x) * Mathf.Rad2Deg, Vector3.up);
			}
		}
		else
		{
			transform.localScale = character.flip ? new Vector3(-1, 1, 1) : Vector3.one;
		}
	}

	public void SetCurrentHitbox(params int[] index)
	{
        for (int i = 0; i < attackHitboxesInOrder.Count; i++)
		{
            if (index.Contains(i))
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
