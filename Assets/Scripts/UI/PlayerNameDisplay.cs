using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerNameDisplay : MonoBehaviour
{
    Text text;
    Image[] images;

    Color transparent = new Color(1f, 1f, 1f, 0f);

    void Start()
    {
        text = GetComponentInChildren<Text>();
        text.text = "";

        text.color = transparent;

        images = GetComponentsInChildren<Image>();

        foreach (Image image in images)
            image.color = transparent;
    }

    public void StartDisplayAnimation(LittleGuyInformation info)
	{
        string chars = info.FullName;
        text.color = transparent;
        foreach (Image image in images)
            image.color = transparent;

        text.text = "";

        StartCoroutine(DisplayAnimation(chars));
	}

    private IEnumerator DisplayAnimation(string info)
	{
        text.DOColor(Color.white, 0.3f);
        foreach (Image image in images)
            image.DOColor(Color.white, 0.3f);

        yield return new WaitForSeconds(0.1f);

        foreach (var c in info)
		{
            text.text += c;
            yield return new WaitForSeconds(0.05f);
		}

        yield return new WaitForSeconds(2f);

        text.DOColor(transparent, 0.8f);
        foreach (Image image in images)
            image.DOColor(transparent, 0.3f);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
