using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CameraManager : MonoBehaviour
{
    public static CameraManager i;

    public GameObject cameraObj;
    public GameObject cameraBrain;
    public CinemachineTargetGroup targetGroup;

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
