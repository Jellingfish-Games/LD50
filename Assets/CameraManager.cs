using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    public static CameraManager i;


    public GameObject cameraObj;
    public GameObject cameraBrain;

    void Awake()
    {
        if (i == null)
        {
            i = this;
        } else
        {
            Destroy(gameObject);
        }
    }
}
