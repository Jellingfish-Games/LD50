using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Autokill : MonoBehaviour
{
    public float killAfter = 3f;

    private void FixedUpdate()
    {
        killAfter -= Time.fixedDeltaTime;
        if (killAfter < 0)
        {
            Destroy(gameObject);
        }
    }
}