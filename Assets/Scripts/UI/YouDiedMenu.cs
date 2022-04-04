using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class YouDiedMenu : MonoBehaviour
{
    public Transform youDiedTransform;
    public RectTransform container;
    public CanvasGroup infoContainer;

    public LittleGuyUIController uiControllerPrefab;

    public LittleGuyUIController killerThumbnail;
    public Text guyNameText;
    public Text guyClassLevelText;
    public RectTransform guyAggressiveSlider;
    public RectTransform guyAwarenessSlider;
    public RectTransform guyDodgeSlider;
    public RectTransform guyStubbornSlider;
    public Transform uiControllerSpawnRoot;

    public IEnumerator DoSequence(LittleGuyInformation killer, List<LittleGuyInformation> kills)
    {
        infoContainer.alpha = 0;
        killerThumbnail.information = killer;
        killerThumbnail.UpdateInfo();

        guyNameText.text = killer.FullName;
        guyClassLevelText.text = $"{killer.Class} LVL {killer.BattleStats.Levelups + 1}";
        guyAggressiveSlider.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, Mathf.Clamp01(killer.BattleStats.Aggressiveness * 58));
        guyAwarenessSlider.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, Mathf.Clamp01(killer.BattleStats.Awareness * 58));
        guyDodgeSlider.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, Mathf.Clamp01(killer.BattleStats.DodgeSkill * 58));
        guyStubbornSlider.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, Mathf.Clamp01(killer.MetaStats.Stubborness * 58));

        foreach (var killed in kills)
        {
            var inst = Instantiate(uiControllerPrefab, uiControllerSpawnRoot);
            inst.information = killed;
        }

        youDiedTransform.localScale = Vector3.one * 0.5f;

        while (youDiedTransform.localScale.magnitude < Vector3.one.magnitude * 1.5f)
        {
            youDiedTransform.localScale += Vector3.one * Time.deltaTime;
            yield return null;
        }

        container.DOSizeDelta(Vector2.up * 256, 2).SetEase(Ease.OutQuad);
        yield return new WaitForSecondsRealtime(1);
        infoContainer.DOFade(1, 1).SetEase(Ease.OutQuad);
    }
}
