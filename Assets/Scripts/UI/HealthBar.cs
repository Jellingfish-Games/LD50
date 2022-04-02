using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class HealthBar : MonoBehaviour
{
    private float _lastHP;

    private BossCharacter player;

    public RectTransform redBarPart;
    public RectTransform whiteBarPart;

    public float width = 211;
    private float _startHP;
    private float redBarWidth = 0;
    private float whiteBarWidth = 0;
    private float _whiteBarLag = 0;

    // Update is called once per frame
    void Update()
    {
        if (player == null)
        {
            player = BattleManager.instance.player;
            _lastHP = player.hp;
            _startHP = player.hp;
            whiteBarPart.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, whiteBarWidth);
        }

        if (player != null)
        {
            if (player.hp < _lastHP)
            {
                float severity = Mathf.Clamp((_lastHP - player.hp) * 0.005f, 0.1f, 2f);
                transform.DOShakePosition(0.5f, severity * (Vector3.right + Vector3.up), 12);
                _whiteBarLag = severity * 1;
            }

            _lastHP = player.hp;

            float rightTarget = 211 * (player.hp / _startHP);
            redBarWidth = Mathf.Min(redBarWidth + _startHP * 0.02f * Time.deltaTime, rightTarget);
            redBarPart.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, redBarWidth);

            _whiteBarLag = _whiteBarLag - Time.deltaTime;
            if (_whiteBarLag < 0)
            {
                whiteBarWidth = Mathf.Max(whiteBarWidth - 0.01f * _startHP * Time.deltaTime, redBarWidth);
                whiteBarPart.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, whiteBarWidth);
            }
        }
    }
}
