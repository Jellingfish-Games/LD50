using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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

    public LayerMask groundMask;

    public BossCharacter player;

    public LittleGuyHealthBar littleGuyHealthBarPrefab;
    public Transform littleGuyHealthBarRoot;

    public LittleGuySpeechBubble speechBubblePrefab;
    public Transform littleGuyQuoteRoot;

    public CanvasGroup attackSelectionGroup;
    public Transform attackSelectionGridRoot;

    public PlayerNameDisplay nameDisplay;

    private List<BossAttackUIElement> targets = new List<BossAttackUIElement>();

    public LittleGuyController littleGuyPrefab;
    public Transform littleGuySpawnPosition;
    public Transform littleGuySpawnPosition2;
    public Transform littleGuySpawnPosition3;

    public BossAttackUIElement attackElement;

    public BossAttackUIElement primaryAttackSlot;
    public BossAttackUIElement secondaryAttackSlot;

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
        //SwitchToNewState(BattleState.Battle);
        SwitchToNewState(BattleState.AttackSelection);
    }

    public void SpawnLittleGuyHealthBar(LittleGuyController controller)
    {
        LittleGuyHealthBar healthbar = Instantiate(littleGuyHealthBarPrefab, littleGuyHealthBarRoot);
        healthbar.guyRef = controller;
        littleGuys.Add(controller);
        CameraManager.i.targetGroup.AddMember(controller.transform, .15f, 1);
    }

    public void LittleGuyDie(LittleGuyController controller)
    {
        littleGuys.Remove(controller);
        CameraManager.i.targetGroup.RemoveMember(controller.transform);
        numLittleGuysKilled++;
    }

    // For the "multiplayer" game mode
    public IEnumerator SpawnNewLittleGuys()
    {
        while (player.hp > 0)
        {
            if (littleGuys.Count < Mathf.Clamp((Mathf.Pow(numLittleGuysKilled, 0.5f) * 0.1f), 1, 50)) {
                SpawnLittleGuy();
            }
            yield return new WaitForSeconds(Random.Range(1f, 5f));
        }
    }

    public LittleGuyController SpawnLittleGuy()
    {
        return Instantiate(littleGuyPrefab, littleGuySpawnPosition.position, Quaternion.identity); 
    }

    public void ConfirmAttackSelection()
    {
        BossAttackUIElement primaryElement = primaryAttackSlot.GetComponent<BossAttackUIElement>();
        BossAttackUIElement secondaryElement = secondaryAttackSlot.GetComponent<BossAttackUIElement>();

        if (primaryAttackSlot.attack != null && secondaryAttackSlot.attack != null)
        {
            player.ReplaceAttackInSlot(primaryAttackSlot.attack, false);
            player.ReplaceAttackInSlot(secondaryAttackSlot.attack, true);
            SwitchToNewState(BattleState.Cutscene);
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
                instance.primaryAttackSlot.SetButtonActive(true);
                instance.secondaryAttackSlot.SetButtonActive(true);
                break;
            case BattleState.Battle:
                //instance.StartCoroutine(instance.SpawnNewLittleGuys());
                break;
            case BattleState.Cutscene:
                CameraManager.i.Cutscene();
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
                instance.primaryAttackSlot.SetButtonActive(false);
                instance.secondaryAttackSlot.SetButtonActive(false);
                break;
                // etc
        }
    }

    public void LittleGuyQuote(LittleGuyController controller, LittleGuyQuotes possibleQuotes)
	{
        LittleGuySpeechBubble bubble = Instantiate(speechBubblePrefab, littleGuyQuoteRoot);

        float max = possibleQuotes.quotes.Sum(q => q.relativeChance);

        float rand = Random.Range(0, max);

        float sum = 0f;

        int quoteChoiceIndex = 0;

        for (int i = 0; i < max; i++)
		{
            if (sum + possibleQuotes.quotes[i].relativeChance > rand)
			{
                quoteChoiceIndex = i;
                break;
			}

            sum += possibleQuotes.quotes[i].relativeChance;
		}

        string toPrint = string.Format(possibleQuotes.quotes[quoteChoiceIndex].quote, controller.info.Name, controller.info.FullName, "Warrior", "BOSSMAN", "foul beast");

        bubble.AttachToPlayer(controller, toPrint);
	}
}
