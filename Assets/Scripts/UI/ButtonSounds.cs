using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ButtonSounds : MonoBehaviour, IPointerEnterHandler, IPointerClickHandler
{
    private Button btn;
    public bool special = false;

    void Start()
    {
        btn = GetComponent<Button>();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (btn.enabled && btn.interactable)
        {
            if (special)
            {
                AudioManager.ChangeMusic(BGM.BossIntro, BGM.BossLoop);
                SFX.UIStartGame.Play();
            } else
            {
                SFX.UIConfirm.Play();
            }
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
