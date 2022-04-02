using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackHitboxList : MonoBehaviour
{
    public List<GameObject> attackHitboxesInOrder;

    // Whether to rotate the hitbox stack in the direction of movement, or just horizontal flip
    public bool enableDirectionality;

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
			transform.localRotation = Quaternion.AngleAxis(Mathf.Atan2(character.velocity.z, character.velocity.x), Vector3.up);
		}
		else
		{
			transform.localScale = character.flip ? new Vector3(-1, 1, 1) : Vector3.one;
		}
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
