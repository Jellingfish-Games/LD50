using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BossAttackUIElement : MonoBehaviour
{
    public BossAttack attack;

    public Image image;

    // Start is called before the first frame update
    void Start()
    {
        image.sprite = attack.icon;
    }
}
