using Cinemachine;
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

    private static BattleManager _instance;

    public static BattleManager instance { 
        get
        {
            if (_instance == null)
			{
                _instance = new GameObject().AddComponent<BattleManager>();
			}

            return _instance;
        }
        private set
        {
            _instance = value;
        }
    }

    public static BattleState state = BattleState.Intro;

    public AllBossAttacks attacks;

    public BossCharacter player;

    public LittleGuyHealthBar littleGuyHealthBarPrefab;
    public Transform littleGuyHealthBarRoot;

    public CanvasGroup attackSelectionGroup;
    public Transform attackSelectionGridRoot;

    private List<BossAttackUIElement> targets = new List<BossAttackUIElement>();

    public LittleGuyController littleGuyPrefab;
    public Transform littleGuySpawnPosition;

    public BossAttackUIElement attackElement;

    public BossAttackUIElement primaryAttackSlot;
    public BossAttackUIElement secondaryAttackSlot;

    public CinemachineTargetGroup targetGroup;

    private int numLittleGuysKilled;

    private List<LittleGuyController> littleGuys = new List<LittleGuyController>();

    void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        Init();
    }

    private void Init()
	{
        SwitchToNewState(BattleState.Battle);
    }

    public void SpawnLittleGuyHealthBar(LittleGuyController controller)
    {
        LittleGuyHealthBar healthbar = Instantiate(littleGuyHealthBarPrefab, littleGuyHealthBarRoot);
        healthbar.guyRef = controller;
        littleGuys.Add(controller);
        targetGroup.AddMember(controller.transform, 1, 1);
    }

    public void LittleGuyDie(LittleGuyController controller)
    {
        littleGuys.Remove(controller);
        targetGroup.RemoveMember(controller.transform);
        numLittleGuysKilled++;
    }

    // For the "multiplayer" game mode
    private IEnumerator SpawnNewLittleGuys()
    {
        while (player.hp > 0)
        {
            yield return new WaitForSeconds(Random.Range(1f, 5f));
            if (littleGuys.Count < Mathf.Clamp((Mathf.Pow(numLittleGuysKilled, 0.5f) * 0.1f), 1, 50)) {
                SpawnLittleGuy();
            }
        }
    }

    private void SpawnLittleGuy()
    {
        Instantiate(littleGuyPrefab, littleGuySpawnPosition.position, Quaternion.identity);
        
    }

    public void ConfirmAttackSelection()
    {
        BossAttackUIElement primaryElement = primaryAttackSlot.GetComponent<BossAttackUIElement>();
        BossAttackUIElement secondaryElement = secondaryAttackSlot.GetComponent<BossAttackUIElement>();

        if (primaryAttackSlot.attack != null && secondaryAttackSlot.attack != null)
        {
            player.ReplaceAttackInSlot(primaryAttackSlot.attack, false);
            player.ReplaceAttackInSlot(secondaryAttackSlot.attack, false);
            SwitchToNewState(BattleState.Battle);
        }
    }

    public static void ToggleChosenAttack(BossAttack attack)
    {
        if (instance.primaryAttackSlot.attack == null && instance.secondaryAttackSlot.attack != attack)
        {
            instance.primaryAttackSlot.SetAttack(attack);

            UpdateButtonActiveStates();
        } else if (instance.secondaryAttackSlot.attack == null && instance.primaryAttackSlot.attack != attack)
        {
            instance.secondaryAttackSlot.SetAttack(attack);

            UpdateButtonActiveStates();
        } else
        {
            RemoveChosenAttack(attack);
        }
    }

    private static void UpdateButtonActiveStates()
    {
        foreach (BossAttackUIElement t in instance.targets)
        {
            t.SetActive((t.attack == instance.primaryAttackSlot.attack) || (t.attack == instance.secondaryAttackSlot.attack));
        }
    }

    public static void RemoveChosenAttack(BossAttack attack)
    {
        if (instance.secondaryAttackSlot.attack == attack)
        {
            instance.secondaryAttackSlot.SetAttack(null);
            UpdateButtonActiveStates();

        } else if (instance.primaryAttackSlot.attack == attack)
        {
            instance.primaryAttackSlot.SetAttack(null);

            UpdateButtonActiveStates();
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
                instance.targets.Clear();
                foreach (var attackElem in instance.attacks.attacks)
                {
                    if (attackElem.Unlocked())
                    {
                        BossAttackUIElement uiInst = Instantiate(instance.attackElement, instance.attackSelectionGridRoot);
                        uiInst.SetAttack(attackElem.attack);
                        uiInst.SetActive(false);
                        instance.targets.Add(uiInst);
                    }
                }
                instance.attackSelectionGroup.gameObject.SetActive(true);
                break;
            case BattleState.Battle:
                instance.StartCoroutine(instance.SpawnNewLittleGuys());
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
