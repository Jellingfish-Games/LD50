using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuCameraLerp : MonoBehaviour
{
    public float endPos;
    public float duration;

    // Start is called before the first frame update
    void Start()
    {
        transform.DOMoveY(endPos, duration).SetEase(Ease.OutQuad);
    }
}
