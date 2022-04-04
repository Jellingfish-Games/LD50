using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spinner : MonoBehaviour
{
    public float spinSpeed = 10f;
    private void FixedUpdate()
    {
        foreach (var i in GetComponentsInChildren<CapsuleCollider>())
        {
            i.transform.RotateAround(BattleManager.instance.player.transform.position, Vector3.up, spinSpeed);
            i.gameObject.GetComponentInChildren<SpriteRenderer>().transform.rotation = Quaternion.Euler(45, 0, 0);
        }
    }
}
