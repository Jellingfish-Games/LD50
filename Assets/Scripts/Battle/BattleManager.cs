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
    public CanvasGroup mainUIGroup;
    public CanvasGroup blackFadeGroup;
    public CanvasGroup semitransparentBlackFadeGroup;
    public CanvasGroup attackReplacementGroup;
    public CanvasGroup victoryGroup;
    public CanvasGroup attackDisplayGroup;
    public YouDiedMenu youDiedMenu;

    public UnityEngine.UI.Button attackSelectionConfirmButton;
    public UnityEngine.UI.Text attackSelectionConfirmButtonText;


    public Transform attackSelectionGridRoot;

    public PlayerNameDisplay nameDisplay;

    private List<BossAttackUIElement> targets = new List<BossAttackUIElement>();

    public LittleGuyController littleGuyPrefab;
    public Transform littleGuySpawnPosition;
    public Transform littleGuySpawnPosition2;
    public Transform littleGuySpawnPosition3;

    public BossAttackUIElement attackElement;
    public AttackReplacementElement replacementElementPrefab;
    public Transform replacementElementRoot;

    public BossAttackUIElement primaryAttackSlot;
    public BossAttackUIElement secondaryAttackSlot;

    private int numLittleGuysKilled;

    public List<LittleGuyController> littleGuys = new List<LittleGuyController>();

    public List<LittleGuyStatPackage> encounteredLittleGuyStatPackages = new List<LittleGuyStatPackage>();

    public LittleGuyDeathBanner littleGuyDeathBanner;
    public BossDeathBanner bossDeathBanner;

    public int timeWhenMultipleGuysComeIn = 1;
    public int timeWhenChanceIncreases = 10;

    void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        Init();
    }

    private void Update()
    {
        //if (Input.GetKeyDown(KeyCode.Q))
        //{
        //    SwitchToNewState(BattleState.Loss);
        //}
    }

    private void Init()
	{
        //SwitchToNewState(BattleState.Battle);
        attackSelectionGroup.Hide(0);
        semitransparentBlackFadeGroup.Hide(0);
        attackReplacementGroup.Hide(0);
        mainUIGroup.Hide(0);
        blackFadeGroup.Show(1);
        victoryGroup.Hide(0);
        attackDisplayGroup.Hide(0);
        SwitchToNewState(BattleState.Intro);

        StartCoroutine(PeriodicallySpawnLittleGuys());
    }

    public void SpawnLittleGuyHealthBar(LittleGuyController controller)
    {
        LittleGuyHealthBar healthbar = Instantiate(littleGuyHealthBarPrefab, littleGuyHealthBarRoot);
        healthbar.guyRef = controller;
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

    public IEnumerator PeriodicallySpawnLittleGuys()
	{
        while (true)
		{
            if (state == BattleState.Battle && numLittleGuysKilled >= timeWhenMultipleGuysComeIn)
            {
                float chanceToSpawn = 0.1f;
                if (numLittleGuysKilled > timeWhenChanceIncreases)
                    chanceToSpawn += 1.0f - (5.0f / numLittleGuysKilled);

                if (Random.Range(0f, 1f) < chanceToSpawn)
                {
                    var guy = TrySpawnPreviouslyEncounteredLittleGuy();

                    if (guy != null)
                    {
                        yield return guy.ShortEnterCoroutine();
                    }
                }
            }

            yield return new WaitForSeconds(5f);
		}
	}

    public LittleGuyController SpawnLittleGuy()
    {

        var littleGuy = Instantiate(littleGuyPrefab, littleGuySpawnPosition.position, Quaternion.identity);

        if (encounteredLittleGuyStatPackages.Count > 0)
		{
            if (Random.Range(0f, 1f) < 0.5f)
			{
                littleGuy.info.StatPackage = encounteredLittleGuyStatPackages[Random.Range(0, encounteredLittleGuyStatPackages.Count)];

                float diceRoll = Random.Range(0f, 1f);

                if (true)//diceRoll - littleGuy.info.MetaStats.Stubborness * 0.2f > 0.3f)
				{
                    int levelUps = Random.Range(2, 5);

                    for (int i = 0; i < levelUps; i++)
					{
                        littleGuy.info.LevelUp();
					}
				}
			}
            else
			{
                if (Random.Range(0f, 1f) < 0.8f)
				{
                    int level = encounteredLittleGuyStatPackages.Max(s => s.BattleStats.Levelups) / 2;

                    for (int i = 0; i < level; i++)
                        littleGuy.info.LevelUp();
				}
			}
		}

        littleGuy.info.StatPackage.BattleStats.HP = littleGuy.info.StatPackage.BattleStats.MaxHP;
        littleGuys.Add(littleGuy);

        return littleGuy;
    }

    public LittleGuyController TrySpawnPreviouslyEncounteredLittleGuy()
	{
        if (encounteredLittleGuyStatPackages.Count == 0)
            return null;

        if (encounteredLittleGuyStatPackages.All(lgsp => littleGuys.Any(lg => lg.info.StatPackage == lgsp)))
            return null;


        var lgsps = encounteredLittleGuyStatPackages.ToList().Where(l => !littleGuys.Any(lg => lg.info.StatPackage == l)).OrderBy(_ => Random.Range(0f, 1f));
        LittleGuyStatPackage lgsp = lgsps.FirstOrDefault();

        if (lgsps.FirstOrDefault() == null)
		{
            return null;
		}

        var littleGuy = Instantiate(littleGuyPrefab, littleGuySpawnPosition.position, Quaternion.identity);
        littleGuy.info.StatPackage = lgsp;

        float diceRoll = Random.Range(0f, 1f);

        if (diceRoll - littleGuy.info.MetaStats.Stubborness * 0.2f > 0.3f)
        {
            int levelUps = Random.Range(2, 5);

            for (int i = 0; i < levelUps; i++)
            {
                littleGuy.info.LevelUp();
            }
        }
        littleGuys.Add(littleGuy);

        littleGuy.info.StatPackage.BattleStats.HP = littleGuy.info.StatPackage.BattleStats.MaxHP;

        return littleGuy;
    }

    public void ConfirmAttackSelection(bool returnToBattle = false)
    {
        if (primaryAttackSlot.attack != null && secondaryAttackSlot.attack != null)
        {
            player.ReplaceAttackInSlot(primaryAttackSlot.attack, false);
            player.ReplaceAttackInSlot(secondaryAttackSlot.attack, true);
            SwitchToNewState(returnToBattle ? BattleState.Battle : BattleState.Cutscene);
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

        int numSelected = 0;
        if (instance.secondaryAttackSlot.attack != null)
        {
            numSelected++;
        }
        if (instance.primaryAttackSlot.attack != null)
        {
            numSelected++;
        }

        if (numSelected == 2)
        {
            instance.attackSelectionConfirmButton.interactable = true;
            instance.attackSelectionConfirmButtonText.text = "START BATTLE";
        } else if (numSelected == 1)
        {
            instance.attackSelectionConfirmButton.interactable = false;
            instance.attackSelectionConfirmButtonText.text = "SELECT ONE";
        } else
        {
            instance.attackSelectionConfirmButton.interactable = false;
            instance.attackSelectionConfirmButtonText.text = "SELECT TWO";
        }
    }

    public static void RemoveChosenAttack(BossAttack attack)
    {
        if (instance.secondaryAttackSlot.attack == attack)
        {
            instance.secondaryAttackSlot.SetAttack(null);
            UpdateButtonActiveStates();

        } 
        else if (instance.primaryAttackSlot.attack == attack)
        {
            instance.primaryAttackSlot.SetAttack(null);

            UpdateButtonActiveStates();
        }
    }

    public static void SwitchToNewState(BattleState newState)
    {
        instance.StartCoroutine(SwitchToNewStateRoutine(newState));
    }

    private static IEnumerator SwitchToNewStateRoutine(BattleState newState)
    {
        yield return CleanupState();
        state = newState;
        switch (state)
        {
            case BattleState.Intro:
                instance.encounteredLittleGuyStatPackages = new List<LittleGuyStatPackage>();
                instance.StartCoroutine(instance.blackFadeGroup.Hide(0.5f));
                yield return SwitchToNewStateRoutine(BattleState.AttackSelection);
                break;
            case BattleState.AttackSelection:
                BattleManager.instance.player.RestrictControls();
                BattleManager.instance.player.LockInPlace();
                yield return instance.semitransparentBlackFadeGroup.Show(0.5f);
                instance.StartCoroutine(instance.attackDisplayGroup.Show(0.5f));
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
                yield return instance.attackSelectionGroup.Show(0.5f);
                instance.primaryAttackSlot.SetButtonActive(true);
                instance.secondaryAttackSlot.SetButtonActive(true);
                break;
            case BattleState.Battle:
                instance.player.UnlockPlace();
                instance.player.EnableControls();

                instance.StartCoroutine(instance.mainUIGroup.Show(0.5f));
                //instance.StartCoroutine(instance.SpawnNewLittleGuys());
                break;
            case BattleState.Cutscene:
                AudioManager.ChangeMusic(BGM.BossIntro, BGM.BossLoop);
                CameraManager.i.Cutscene();
                break;
            case BattleState.Victory:
                instance.StartCoroutine(instance.victoryGroup.Show(0.5f));
                instance.StartCoroutine(instance.youDiedMenu.DoSequence(false));
                yield return new WaitForSeconds(2);
                yield return instance.victoryGroup.Hide(0.5f);
                yield return new WaitForSeconds(1);
                // THIS IS HWERE CHARACTERS LEVEL UP
                SwitchToNewState(BattleState.Cutscene);
                break;
            case BattleState.Loss:
                instance.StartCoroutine(instance.victoryGroup.Show(0.5f));
                instance.StartCoroutine(instance.youDiedMenu.DoSequence(true));
                yield return new WaitForSeconds(2);
                yield return instance.blackFadeGroup.Show(0.5f);
                yield return new WaitForSeconds(1);
                GlobalManager.instance.LoadMainMenu();
                break;
            case BattleState.PhaseTransition:
                Time.timeScale = 0;
                instance.player.RestrictControls();
                yield return instance.semitransparentBlackFadeGroup.Show(0.5f);
                instance.GenerateAttackChoices();
                yield return instance.attackReplacementGroup.Show(0.5f);
                break;
        }
    }

    private static IEnumerator CleanupState()
    {
        switch (state)
        {
            case BattleState.Intro:
                // Hide elements visible during intro.
                break;
            case BattleState.AttackSelection:
                instance.primaryAttackSlot.SetButtonActive(false);
                instance.secondaryAttackSlot.SetButtonActive(false);
                instance.StartCoroutine(instance.attackSelectionGroup.Hide(0.5f));
                yield return instance.semitransparentBlackFadeGroup.Hide(0.5f);
                break;
            case BattleState.Battle:
                instance.StartCoroutine(instance.mainUIGroup.Hide(0.5f));
                break;
            case BattleState.PhaseTransition:
                instance.StartCoroutine(instance.attackReplacementGroup.Hide(0.5f));
                yield return instance.semitransparentBlackFadeGroup.Hide(0.5f);
                Time.timeScale = 1;
                instance.player.EnableControls();
                break;
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

        string[] insults =
        {
            "foul beast",
            "rotten thing",
            "lich",
            "monster",
            "evil creature",
            "beast",
        };

        string insult = insults[Random.Range(0, insults.Length)];

        string toPrint = string.Format(possibleQuotes.quotes[quoteChoiceIndex].quote, controller.info.Name, controller.info.FullName, controller.info.Class.ToString(), "Ignatius", insult);

        bubble.AttachToPlayer(controller, toPrint);
	}


    public void GenerateAttackChoices()
    {
        List<BossAttack> availableAttacks = new List<BossAttack>();
        foreach (var attackElem in instance.attacks.attacks)
        {
            if (attackElem.Unlocked() && attackElem.attack != instance.primaryAttackSlot.attack && attackElem.attack != instance.secondaryAttackSlot.attack)
            {
                availableAttacks.Add(attackElem.attack);
            }
        }

        foreach(Transform t in replacementElementRoot)
        {
            Destroy(t.gameObject);
        }

        BossAttack primaryReplacement = availableAttacks[Random.Range(0, availableAttacks.Count)];
        availableAttacks.Remove(primaryReplacement);

        AttackReplacementElement replacementElement1 = Instantiate(replacementElementPrefab, replacementElementRoot);
        replacementElement1.Initialize(instance.primaryAttackSlot.attack, primaryReplacement);

        BossAttack secondaryReplacement = availableAttacks[Random.Range(0, availableAttacks.Count)];
        AttackReplacementElement replacementElement2 = Instantiate(replacementElementPrefab, replacementElementRoot);
        replacementElement2.Initialize(instance.secondaryAttackSlot.attack, secondaryReplacement);
    }

    public void ReplaceAttackSlot(BossAttack oldAttack, BossAttack newAttack) { 
        if (oldAttack == primaryAttackSlot.attack)
        {
            primaryAttackSlot.SetAttack(newAttack);
        } else
        {
            secondaryAttackSlot.SetAttack(newAttack);
        }

        ConfirmAttackSelection(true);
    }

    public void WaveEnds()
    {
        SwitchToNewState(BattleState.Victory);
    }

    public void PlayerDies()
    {
        SwitchToNewState(BattleState.Loss);
    }
}
