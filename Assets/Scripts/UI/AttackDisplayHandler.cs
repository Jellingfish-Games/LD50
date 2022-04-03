using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackDisplayHandler : MonoBehaviour
{
    public GameObject rightMouseButtonImg;
    public GameObject leftMouseButtonImg;

    // Update is called once per frame
    void Update()
    {
        leftMouseButtonImg.SetActive(Input.GetMouseButton(0));
        rightMouseButtonImg.SetActive(Input.GetMouseButton(1));
    }
}
