using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LittleGuyHealthBar : MonoBehaviour
{

    public LittleGuyController guyRef;

    public Color healthyColor;
    public Color midColor;
    public Color unhealthyColor;

    public float fullWidth = 11;

    public RectTransform bar;
    public UnityEngine.UI.Image barImage;

    private float _lastHP;

    // Update is called once per frame
    void Update()
    {
        if (guyRef != null)
        {
            if (guyRef.info.BattleStats.HP <=0)
            {
                Destroy(gameObject);
                return;
            }
            if (guyRef.info.BattleStats.HP < _lastHP)
            {
                float severity = Mathf.Clamp((_lastHP - guyRef.info.BattleStats.HP) * 0.005f, 0.1f, 2f);
                transform.DOShakePosition(0.5f, severity * (Vector3.right + Vector3.up), 12);
            }

            _lastHP = guyRef.info.BattleStats.HP;

            float percentageHP = (guyRef.info.BattleStats.HP / guyRef.info.BattleStats.MaxHP);
            bar.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, fullWidth * percentageHP);

            if (percentageHP > 0.5f)
            {
                barImage.color = healthyColor;
            } else if (percentageHP > 0.25f)
            {
                barImage.color = midColor;
            } else
            {
                barImage.color = unhealthyColor;
            }

            transform.position = Camera.main.WorldToScreenPoint(guyRef.transform.position + 1 * Vector3.up);
        }
    }
}
