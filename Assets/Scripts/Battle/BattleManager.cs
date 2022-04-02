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

    void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        SwitchToNewState(BattleState.Intro);
    }

    public static void SwitchToNewState(BattleState newState)
    {
        CleanupState();
        state = BattleState.Intro;
        switch (state)
        {
            case BattleState.Intro:
                // Show elements visible during intro.
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
            // etc
        }
    }
}
