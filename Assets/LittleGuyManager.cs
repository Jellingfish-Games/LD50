using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LittleGuyManager : MonoBehaviour
{
    public static LittleGuyManager i;
    public List<Color> palette;
    public List<Sprite> hats;
    public List<Sprite> weapons;

    private void Awake()
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
