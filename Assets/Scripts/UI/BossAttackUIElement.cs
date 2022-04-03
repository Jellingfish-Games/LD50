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
            nameText.text = attack.name;
            descriptionText.text = attack.description;
        } else
        {
            image.sprite = defaultSprite;
        }

        button.onClick.RemoveAllListeners();
        if (isSlotOnBottom)
        {
            button.onClick.AddListener(() => BattleManager.RemoveChosenAttack(attack));
        } else
        {
            button.onClick.AddListener(() => BattleManager.ToggleChosenAttack(attack));
        }
    }

    public void SetActive(bool active)
    {
        activeImg.gameObject.SetActive(active);
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
