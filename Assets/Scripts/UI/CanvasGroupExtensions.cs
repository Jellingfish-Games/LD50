using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class CanvasGroupExtensions
{
    public static IEnumerator Hide(this CanvasGroup group, float duration = 1)
    {
        group.interactable = false;
        group.blocksRaycasts = false;

        if (duration > 0.05f)
        {
            group.alpha = 1;

            while (group.alpha > 0)
            {
                group.alpha -= Time.unscaledDeltaTime / duration;
                yield return new WaitForSecondsRealtime(0.01f);
            }
        }
        group.alpha = 0;
    }
    public static IEnumerator Show(this CanvasGroup group, float duration = 1)
    {
        if (duration > 0.05f) { 
            group.alpha = 0;

            while (group.alpha < 1)
            {
                group.alpha += Time.unscaledDeltaTime / duration;
                yield return new WaitForSecondsRealtime(0.01f);
            }
        }

        group.alpha = 1;
        group.interactable = true;
        group.blocksRaycasts = true;
    }
}
