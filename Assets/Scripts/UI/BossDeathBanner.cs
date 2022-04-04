using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BossDeathBanner : MonoBehaviour
{
    Image bg;
    Image fg;

    Color transparent = new Color(1f, 1f, 1f, 0f);

    void Start()
    {
        bg = GetComponent<Image>();
        fg = transform.Find("TextImage").GetComponent<Image>();
        bg.color = transparent;
        fg.color = transparent;
    }

    public void Show()
    {
        StartCoroutine(ShowCoroutine());
    }

    private IEnumerator ShowCoroutine()
    {
        fg.DOColor(Color.white, 0.7f);
        bg.DOColor(Color.white, 0.4f);

        yield return new WaitForSeconds(0.7f);
    }

    public void Hide()
    {
        StartCoroutine(HideCoroutine());
    }

    private IEnumerator HideCoroutine()
    {
        fg.DOColor(transparent, 0.7f);
        bg.DOColor(transparent, 0.4f);

        yield return new WaitForSeconds(0.7f);
    }
}
