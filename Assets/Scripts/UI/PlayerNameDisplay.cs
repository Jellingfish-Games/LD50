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

    List<LittleGuyInformation> nameQueue = new List<LittleGuyInformation> ();

    Coroutine mainCoroutine;


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

        StartCoroutine(MainCoroutine());
    }

    public void StartDisplayAnimation(LittleGuyInformation info)
	{
        nameQueue.Add(info);
    }

    private IEnumerator MainCoroutine()
	{
        while (true)
		{
            if (nameQueue.Count > 0)
			{
                LittleGuyInformation info = nameQueue[0];
                nameQueue.Remove(info);

                string chars = info.FullName;
                text.color = transparent;
                levelText.color = transparent;
                foreach (Image image in images)
                    image.color = transparent;

                text.text = "";
                levelText.text = "";


                yield return DisplayAnimation(chars, info.Class.ToString(), info.BattleStats.Levelups + 1, nameQueue.Count > 1);
            }
            yield return null;
		}
	}

    private IEnumerator DisplayAnimation(string info, string guyClass, int level, bool quick)
	{
        text.DOColor(Color.white, quick ? 0.2f : 0.3f);
        foreach (Image image in images)
            image.DOColor(Color.white, quick ? 0.2f : 0.3f);

        yield return new WaitForSeconds(0.1f);

        int letterCount = quick ? 3 : 1;

        for (int i = 0; i < info.Length; i++)
		{
            text.text += info[i];
            letterCount--;

            if (letterCount == 0)
                yield return new WaitForSeconds(quick ? 0f : 0.05f);

            letterCount = quick ? 3 : 1;
		}
        levelText.DOColor(Color.white, 0.2f);
        string lvl = $"{guyClass} lvl. {level}";

        foreach (var c in lvl)
		{
            levelText.text += c;
            yield return new WaitForSeconds(0.02f);
        }

        yield return new WaitForSeconds(quick ? 0.8f : 2f);

        text.DOColor(transparent, quick ? 0.4f : 0.8f);
        levelText.DOColor(transparent, quick ? 0.4f : 0.8f);
        foreach (Image image in images)
            image.DOColor(transparent, quick ? 0.2f : 0.3f);

        yield return new WaitForSeconds(quick ? 0.4f : 0.8f);
        text.text = "";
        levelText.text = "";
        yield return null;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
