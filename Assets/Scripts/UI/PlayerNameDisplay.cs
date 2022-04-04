using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerNameDisplay : MonoBehaviour
{
    Text text;
    Text levelText;
    Image[] images;

    Color transparent = new Color(1f, 1f, 1f, 0f);

    void Start()
    {
        text = transform.Find("MainLine").Find("Text").GetComponent<Text>();
        text.text = "";

        text.color = transparent;

        levelText = transform.Find("LevelText").GetComponent<Text>();
        levelText.text = "";
        levelText.color = transparent;


        images = GetComponentsInChildren<Image>();

        foreach (Image image in images)
            image.color = transparent;
    }

    public void StartDisplayAnimation(LittleGuyInformation info)
	{
        string chars = info.FullName;
        text.color = transparent;
        levelText.color = transparent;
        foreach (Image image in images)
            image.color = transparent;

        text.text = "";
        levelText.text = "";

        StartCoroutine(DisplayAnimation(chars, info.BattleStats.Levelups + 1));
	}

    private IEnumerator DisplayAnimation(string info, int level)
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
        levelText.DOColor(Color.white, 0.2f);
        string lvl = $"lvl. {level}";

        foreach (var c in lvl)
		{
            levelText.text += c;
            yield return new WaitForSeconds(0.02f);
        }

        yield return new WaitForSeconds(2f);

        text.DOColor(transparent, 0.8f);
        levelText.DOColor(transparent, 0.8f);
        foreach (Image image in images)
            image.DOColor(transparent, 0.3f);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
