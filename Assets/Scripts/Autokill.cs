using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Autokill : MonoBehaviour
{
    public float killAfter = 3f;
    public bool killOnPillarContact = true;
    private void FixedUpdate()
    {
        killAfter -= Time.fixedDeltaTime;
        if (killAfter < 0)
        {
            Destroy(gameObject);
        }
    }


    private void OnTriggerEnter(Collider other)
    {
        if (killOnPillarContact && other.gameObject.CompareTag("Pillar"))
        {
            Destroy(gameObject);
        }
    }
}
