using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ButtonSounds : MonoBehaviour, IPointerEnterHandler, IPointerClickHandler
{
    private Button btn;

    void Start()
    {
        btn = GetComponent<Button>();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (btn.enabled && btn.interactable)
        {
            SFX.UIConfirm.Play();
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (btn.enabled && btn.interactable)
        {
            SFX.UISelectHover.Play();
        }
    }
}
