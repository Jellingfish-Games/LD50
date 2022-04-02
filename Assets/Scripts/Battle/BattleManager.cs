using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleManager : MonoBehaviour
{
    public enum BattleState
    {
        Intro,
        AttackSelection,
        Cutscene,
        Battle,
        PhaseTransition,
        Victory,
        Loss
    }

    public static BattleManager instance;

    public static BattleState state = BattleState.Intro;

    public AllBossAttacks attacks;

    public BossCharacter player;

    public CanvasGroup attackSelectionGroup;
    public Transform attackSelectionGridRoot;

    public BossAttackUIElement attackElement;
    public Transform draggableElementParentPrefab;

    public DropTarget primaryAttackSlot;
    public DropTarget secondaryAttackSlot;

    void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        SwitchToNewState(BattleState.AttackSelection);
    }

    public void ConfirmAttackSelection()
    {
        BossAttackUIElement primaryElement = primaryAttackSlot.GetComponent<BossAttackUIElement>();
        BossAttackUIElement secondaryElement = secondaryAttackSlot.GetComponent<BossAttackUIElement>();

        if (primaryElement != null && secondaryElement != null)
        {
            player.ReplaceAttackInSlot(primaryElement.attack, false);
            player.ReplaceAttackInSlot(secondaryElement.attack, false);
            SwitchToNewState(BattleState.Battle);
        }
    }

    public static void SwitchToNewState(BattleState newState)
    {
        CleanupState();
        state = newState;
        switch (state)
        {
            case BattleState.Intro:
                // Show elements visible during intro.
                break;
            case BattleState.AttackSelection:
                foreach(var attackElem in instance.attacks.attacks)
                {
                    if (attackElem.Unlocked())
                    {
                        Transform parent = Instantiate(instance.draggableElementParentPrefab, instance.attackSelectionGridRoot);
                        BossAttackUIElement uiInst = Instantiate(instance.attackElement, parent);
                        uiInst.attack = attackElem.attack;
                    }
                }
                instance.attackSelectionGroup.gameObject.SetActive(true);
                break;
        }
    }

    private static void CleanupState()
    {
        switch (state)
        {
            case BattleState.Intro:
                // Hide elements visible during intro.
                break;
            case BattleState.AttackSelection:
                instance.attackSelectionGroup.gameObject.SetActive(false);
                break;
                // etc
        }
    }
}
