using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Autokill : MonoBehaviour
{
    public float killAfter = 3f;
    public bool killOnPillarContact = true;
    float timeAlive = 0;

    private void FixedUpdate()
    {
        timeAlive += Time.fixedDeltaTime;

        if (timeAlive > killAfter)
        {
            Destroy(gameObject);
        }
    }


    private void OnTriggerEnter(Collider other)
    {
        var mounga = GetComponent<Rigidbody>();
        var vec1 = new Vector2(mounga.velocity.normalized.x, mounga.velocity.normalized.z);
        var vec2 = new Vector2((transform.position - other.transform.position).normalized.x, (transform.position - other.transform.position).normalized.z);
        if (Vector2.Angle(vec1, vec2) > 130 && killOnPillarContact && other.gameObject.CompareTag("Pillar"))
        {
            Destroy(gameObject);
        }
    }
}
