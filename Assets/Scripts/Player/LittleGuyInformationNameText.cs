using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LittleGuyInformationNameText : MonoBehaviour
{
    public LittleGuyInformation source;

    private Text text;

    void Start()
    {
        text = GetComponent<Text>();
    }

    // Update is called once per frame
    void Update()
    {
        text.text = source.FullName;
    }
}
