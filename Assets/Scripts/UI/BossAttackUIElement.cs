using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class BossAttackUIElement : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public BossAttack attack;

    public Button button;
    public Image image;

    public Sprite defaultSprite;

    public bool active = false;
    public bool isSlotOnBottom = false;

    public Text nameText;
    public Text descriptionText;

    public Transform activeImg;
    public CanvasGroup detailsFoldout;

    // Start is called before the first frame update
    void Start()
    {
        SetAttack(attack);
        SetActive(active);
    }

    public void SetButtonActive(bool active)
    {
        button.enabled = active;
    }

    public void SetAttack(BossAttack attack)
    {
        this.attack = attack;

        if (attack != null)
        {
            image.sprite = attack.icon;
            nameText.text = attack.attackName;
            var rangeStars = "";
            for (int i = 0; i< attack.Range; i++)
            {
                rangeStars += '★';
            }
            for (int i = 0; i < 5 - attack.Range; i++)
            {
                rangeStars += '☆';
            }

            var damageStars = "";
            for (int i = 0; i < attack.Power; i++)
            {
                damageStars += '★';
            }
            for (int i = 0; i < 5 - attack.Power; i++)
            {
                damageStars += '☆';
            }

            var speedStars = "";
            for (int i = 0; i < attack.Speed; i++)
            {
                speedStars += '★';
            }
            for (int i = 0; i < 5 - attack.Speed; i++)
            {
                speedStars += '☆';
            }


            descriptionText.text = attack.description + "\r\n\r\n • Power " + damageStars + "\r\n • Range " + rangeStars + "\r\n • Speed " + speedStars;
        } else
        {
            image.sprite = defaultSprite;
        }

        if (button != null)
        {
            button.onClick.RemoveAllListeners();
            if (isSlotOnBottom)
            {
                button.onClick.AddListener(() => BattleManager.RemoveChosenAttack(attack));
            }
            else
            {
                button.onClick.AddListener(() => BattleManager.ToggleChosenAttack(attack));
            }
        }
    }

    public void SetActive(bool active)
    {
        if (active != activeImg.gameObject.activeSelf)
        {
            activeImg.gameObject.SetActive(active);
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (attack != null)
        {
            detailsFoldout.gameObject.SetActive(true);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        detailsFoldout.gameObject.SetActive(false);
    }
}
