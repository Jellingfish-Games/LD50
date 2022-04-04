using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;
using DG.Tweening;

public class LittleGuyAI : MonoBehaviour
{
    private enum LittleGuyState
    {
        Standing,
        Roaming,
        Approaching,
        Fleeing,
        Attacking,
        Staggered,
        Dead
    }

    private enum LittleGuyAnimationState
	{
        Idle,
        Run,
        Hurt,
        Intro,
        Bomb,
        Potion,
        SwingRun,
        HeavySwing,
        Death,
        Roll,
        CastSpell
    }

    private LittleGuyState aiState;
    private LittleGuyAnimationState animationState;
    private LittleGuyInformation information;
    private NavMeshAgent navMeshAgent;
    private Animator animator;
    private SpriteRenderer spriteRenderer;

    private NavMeshPath currentPath;
    private Vector3[] currentPathCorners;

    private float movementDirectionMultiplier = 1;
    float safety = 1;

    private Coroutine aiRoutine;

    private BossCharacter opponent;

    public LittleGuyAttackHitboxList heavySwingHitboxes;
    public LittleGuyAttackHitboxList runSwingHitboxes;

    public BombObject bombObject;
    public Transform throwTransform;

    public bool flip => spriteRenderer.flipX;
    private bool danger;
    private Vector3 dangerPoint;

    private bool canThrowBomb = true;

    private Vector3 knockBack;

    private bool canDrinkPotion = true;

    public LittleGuyAttackHitbox wizardProjectile;

    private bool canScream = true;

    void Awake()
    {
        currentPath = new NavMeshPath();

        opponent = BattleManager.instance.player;
        information = GetComponent<LittleGuyInformation>();
        navMeshAgent = GetComponent<NavMeshAgent>();
        animator = GetComponentInChildren<Animator>();
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
    }

    public void StartAIRoutine()
    {
        InterruptAIRoutine();

        if (navMeshAgent.hasPath && navMeshAgent.isStopped)
		    navMeshAgent.isStopped = false;

        aiState = LittleGuyState.Standing;
		aiRoutine = StartCoroutine(DefaultLoop());
    }

    public void InterruptAIRoutine()
    {
        if (aiRoutine != null)
        {
            StopCoroutine(aiRoutine);
            aiRoutine = null;

            navMeshAgent.isStopped = true;
        }
    }

    private IEnumerator ThinkingFunction()
    {
        if (aiState == LittleGuyState.Dead) yield break;

        if (opponent == null)
        {
            opponent = BattleManager.instance.player;
        }
        Vector3 directionToBoss = -(transform.position - opponent.transform.position);
        float distanceToBoss = directionToBoss.magnitude;
        float percentageHP = information.BattleStats.HP / information.BattleStats.MaxHP;
        float reactionSpeed = information.BattleStats.Awareness;
        float bossPercentageHP = opponent.hp / opponent.maxHP;
        float aggressiveness = information.BattleStats.Aggressiveness * 0.75f + 0.25f * (1 - bossPercentageHP);
        float cautiousness = (1 - percentageHP) * 0.5f + 0.5f;
        Vector3 dodgeDirectionFromNavMesh = Vector3.zero;
        float dodgeDirectionInfluence = 0;
        BossCharacter.BossState bossState = opponent.state;

        safety += Time.deltaTime * (1 - aggressiveness + cautiousness) * 4;

        float diceRoll = Random.Range(0.0f, 1.0f);

        float desiredRoamDistance = 5 - 2 * aggressiveness + cautiousness - safety;

        if (danger)
        {
            dodgeDirectionFromNavMesh = transform.position - dangerPoint;
            dodgeDirectionFromNavMesh.y = 0;
            dodgeDirectionInfluence = 1;
            safety = 0;
            danger = false;

            Debug.Log("DANGER");
        }
        else if (aiState == LittleGuyState.Attacking && diceRoll < 0.05f)
        {
            dodgeDirectionInfluence = 0.5f * (1 - aggressiveness);
            dodgeDirectionFromNavMesh = Quaternion.Euler(0, Random.Range(0, 360), 0) * Vector3.forward;
        }

        if (dodgeDirectionInfluence > 0 && dodgeDirectionInfluence * diceRoll * reactionSpeed > 0.35f)
        {
            Debug.Log("Dodge roll!");
            yield return DodgeRoll(transform.position + dodgeDirectionFromNavMesh.normalized * 3f);
        }
        else
        {
            if (distanceToBoss < 1 && diceRoll + aggressiveness * 0.5f + (aiState == LittleGuyState.Approaching ? 0.2f : 0f) > 1)
            {
                Debug.Log("MeleeAttack");
                yield return MeleeAttack(aggressiveness);
            }
            else if (distanceToBoss > 1 && distanceToBoss < 3f + aggressiveness * 0.3f && diceRoll + aggressiveness * diceRoll * reactionSpeed + (aiState == LittleGuyState.Approaching ? 0.2f : 0f) > 1f)
			{
                Debug.Log("RunAttack");

                if (information.Class == LittleGuyClass.Warrior)
                    yield return RunMeleeAttack(aggressiveness);
                else if (information.Class == LittleGuyClass.Wizard)
                    yield return WizardAttack(aggressiveness);
			}
            else if (aiState == LittleGuyState.Fleeing)
            {
                Vector3 bossDirection = opponent.transform.position - transform.position;

                bossDirection.y = 0;
                bossDirection = bossDirection.normalized * Random.Range(2f, 5f - aggressiveness + cautiousness);

                if (Random.Range(0f, 1f) - cautiousness * 0.2f - reactionSpeed * 0.1 < 0.4f)
				{
                    yield return DodgeRoll(transform.position - bossDirection);
				}

                yield return MoveToPosition(transform.position - bossDirection);

                aiState = LittleGuyState.Roaming;
            }
            else if (aiState == LittleGuyState.Approaching)
			{
                yield return MoveToPosition(opponent.transform.position);
			}
            else
            {
                if (aiState == LittleGuyState.Staggered || aiState == LittleGuyState.Standing)
                {
                    diceRoll += 0.25f;
                }

                float drinkThreshold = information.BattleStats.MaxHP * (0.3f + cautiousness * 0.2f);

                if (canDrinkPotion && information.BattleStats.HP < drinkThreshold && Random.Range(0f, 1f) + cautiousness * 0.2f > 0.7f)
				{
                    yield return DrinkPotion();
				}

                bool wantsToMove = diceRoll + 0.15f * aggressiveness > 0.25f;

                if (wantsToMove)
                {
                    if (diceRoll + aggressiveness - cautiousness > 0.4f)
                    {
                        aiState = LittleGuyState.Approaching;
                    }
                    else if (canThrowBomb && Random.Range(0f, 1f) + aggressiveness * 0.5f > 0.8f)
					{
                        yield return ThrowBomb();
					}
                    else
                    {
                        aiState = LittleGuyState.Roaming;

                        float newMoveDirection = (Random.Range(0, 2) - 1);

                        if (diceRoll < 0.4f)
                        {
                            movementDirectionMultiplier = newMoveDirection;
                        }

                        Vector3 moveDirection = (directionToBoss).normalized * -desiredRoamDistance;
                        moveDirection = Quaternion.Euler(0, movementDirectionMultiplier * Random.Range(10, 45), 0) * moveDirection;
                        Debug.DrawRay(opponent.transform.position + Vector3.up, opponent.transform.position + moveDirection, Color.red, 2);

                        if (Random.Range(0f, 1f) < 0.3f)
                        {
                            Debug.Log("DODGEROLL");
                            yield return DodgeRoll(opponent.transform.position + moveDirection);
                        }

                        yield return MoveToPosition(opponent.transform.position + moveDirection);
                        
                    }
                }
                else
                {
                    // Don't do anything.
                    navMeshAgent.isStopped = true;

                    aiState = LittleGuyState.Standing;

                    yield return new WaitForSeconds(1 + diceRoll - reactionSpeed);
                }
            }
        }
    }

    private IEnumerator DodgeRoll(Vector3 dodgeTarget)
    {
        currentPath = new NavMeshPath();

        NavMeshHit hit;

        navMeshAgent.isStopped = false;

        NavMesh.SamplePosition(dodgeTarget, out hit, 10000f, NavMesh.AllAreas);

        navMeshAgent.CalculatePath(hit.position, currentPath);
        currentPathCorners = currentPath.corners;

        navMeshAgent.destination = dodgeTarget;

        animationState = LittleGuyAnimationState.Roll;
        animator.Play("Guy_Roll");

        yield return new WaitUntil(() => navMeshAgent.remainingDistance < 0.5f || animationState != LittleGuyAnimationState.Roll);

        aiState = LittleGuyState.Standing;

        yield return null;
    }

    private IEnumerator MeleeAttack(float aggressiveness)
	{
		float diceRoll = Random.Range(0f, 1f);
        
		aiState = LittleGuyState.Attacking;

        animator.Play("Guy_Swing_Heavy", -1, 0f);
        animationState = LittleGuyAnimationState.HeavySwing;

        yield return new WaitUntil(() => animationState != LittleGuyAnimationState.HeavySwing);

        if ((opponent.transform.position - transform.position).magnitude < 1) {
            if (aggressiveness + diceRoll > 1.3f)
            {
                yield return MeleeAttack(aggressiveness - 0.25f);
            }
            else
            {
                aiState = LittleGuyState.Fleeing;
            }
        }
        else
		{
            aiState = LittleGuyState.Roaming;
		}

        yield return null;
    }

    private IEnumerator RunMeleeAttack(float aggressiveness)
    {
        float diceRoll = Random.Range(0f, 1f);

        aiState = LittleGuyState.Attacking;

        animator.Play("Guy_Swing_Run", -1, 0f);
        animationState = LittleGuyAnimationState.SwingRun;

        float runTime = 1f + (aggressiveness + diceRoll) * 0.5f;

        Vector3 target = opponent.transform.position + (opponent.transform.position - transform.position).normalized * 2f;

        navMeshAgent.destination = target;
        navMeshAgent.speed *= 1.2f;

        runSwingHitboxes.SetCurrentHitbox(0);

        yield return new WaitForSeconds(runTime);

        runSwingHitboxes.SetCurrentHitbox(-1);

        navMeshAgent.speed /= 1.2f;

        if ((opponent.transform.position - transform.position).magnitude < 1)
        {
            aiState = LittleGuyState.Fleeing;
        }
        else
        {
            aiState = LittleGuyState.Roaming;
        }

        yield return null;
    }

    public void Celebrate()
    {
        InterruptAIRoutine();
        animator.Play("Guy_Intro", -1, 0f);
    }

    private IEnumerator WizardAttack(float aggressiveness)
    {
        float diceRoll = Random.Range(0f, 1f);

        aiState = LittleGuyState.Attacking;

        animator.Play("Guy_Swing_Heavy", -1, 0f);
        animationState = LittleGuyAnimationState.CastSpell;

        yield return new WaitUntil(() => animationState != LittleGuyAnimationState.CastSpell);

        if ((opponent.transform.position - transform.position).magnitude < 1)
        {
            if (aggressiveness + diceRoll > 1.3f)
            {
                yield return MeleeAttack(aggressiveness - 0.25f);
            }
            else
            {
                aiState = LittleGuyState.Fleeing;
            }
        }
        else
        {
            aiState = LittleGuyState.Roaming;
        }

        yield return null;
    }

    void ShootProjectile()
	{
        SFX.GuySpellCast.Play();

        Vector3 target = opponent.transform.position;
        Vector3 targetDelta = target - transform.position;

        var projectile = Instantiate(wizardProjectile, throwTransform.position, Quaternion.identity);

        float projectileSpeed = 4f * Random.Range(0.7f, 1.2f);

        projectile.GetComponent<WizardProjectile>().velocity = targetDelta.normalized * projectileSpeed;
        projectile.transform.localScale = Vector3.one * 3f;
        projectile.attacker = information;

        projectile.transform.localRotation = Quaternion.AngleAxis(Mathf.Atan2(-targetDelta.z, targetDelta.x) * Mathf.Rad2Deg, Vector3.up);
    }

    IEnumerator ThrowBomb()
	{

        aiState = LittleGuyState.Attacking;

        navMeshAgent.isStopped = true;

        animator.Play("Guy_Bomb", -1, 0f);

        animationState = LittleGuyAnimationState.Bomb;

        yield return new WaitUntil(() => animationState != LittleGuyAnimationState.Bomb);

        navMeshAgent.isStopped = false;

        aiState = LittleGuyState.Roaming;

        yield return null;
	}

    void ThrowBombProjectile()
    {
        SFX.GuyBombThrow.Play();

        Vector3 target = opponent.transform.position;
        Vector3 targetDelta = target - transform.position;

        BombObject bomb = Instantiate(bombObject, throwTransform.position, Quaternion.identity);

        float bombSpeed = targetDelta.magnitude * 1.5f * Random.Range(0.7f, 1.2f);

        bomb.transform.DOMoveY(0, 1f);

        bombSpeed = Mathf.Clamp(bombSpeed, 1f, 6f);

        bomb.velocity = targetDelta.normalized * bombSpeed;
        bomb.info = information;

        StartCoroutine(BombCooldown());
    }

    IEnumerator BombCooldown()
	{
        canThrowBomb = false;

        float time = 2f * information.BattleStats.BombCooldownScale;

        yield return new WaitForSeconds(time);

        canThrowBomb = true;

        yield return null;
	}

    IEnumerator DrinkPotion()
	{
        animator.Play("Guy_Potion", -1, 0f);

        animationState = LittleGuyAnimationState.Potion;

        yield return new WaitUntil(() => animationState != LittleGuyAnimationState.Potion);

        canDrinkPotion = false;
        yield return null;
	}

    IEnumerator DefaultLoop()
    {
        yield return EnterArena();

        while (true)
        {
            yield return ThinkingFunction();
        }
    }

    private IEnumerator EnterArena()
    {
        //navMeshAgent.enabled = false;
        //transform.DOMove(BattleManager.instance.littleGuySpawnPosition2, 3);
        //yield return new WaitForSeconds(4f);
        //yield return MoveToDirection(-transform.forward * 8);
        //yield return MoveToDirection(BattleManager.instance.littleGuySpawnPosition2.position);
        navMeshAgent.enabled = true;
        yield return new WaitUntil(() => BattleManager.state == BattleManager.BattleState.Battle);
        //yield return MoveToDirection(- transform.forward * 8);
    }

    private IEnumerator MoveToDirection(Vector3 target)
    {
        yield return MoveToPosition(transform.position + target);
    }

    private IEnumerator MoveToPosition(Vector3 target)
    {
        currentPath = new NavMeshPath();

        NavMeshHit hit;

        navMeshAgent.isStopped = false;

        NavMesh.SamplePosition(target, out hit, 10000f, NavMesh.AllAreas);

        navMeshAgent.CalculatePath(hit.position, currentPath);
        currentPathCorners = currentPath.corners;

        navMeshAgent.destination = target;

        yield return new WaitUntil(() => navMeshAgent.remainingDistance < 0.5f);
    }

    public void Hurt()
	{
        if (aiState != LittleGuyState.Dead)
        {
            InterruptAIRoutine();

            animationState = LittleGuyAnimationState.Hurt;

            aiState = LittleGuyState.Staggered;

            safety = (information.BattleStats.Aggressiveness + information.BattleStats.Awareness) / 2;

            animator.Play("Guy_Hurt", -1, 0f);

            if (canScream)
            {
                SFX.GuyGetHit.Play();

                StartCoroutine(HitSoundCooldown());
            }
        }
    }

    public void Warn(Vector3 where)
	{
        if (aiState == LittleGuyState.Dead) return;

        danger = true;

        dangerPoint = where;

        float diceRoll = Random.Range(0f, 1f);
        if (information.BattleStats.DodgeSkill * 0.3f + diceRoll > 0.8f)
		{
            StartAIRoutine();
		}
	}

    public void Die()
    {
        SFX.GuyDie.Play();

        aiState = LittleGuyState.Dead;
        InterruptAIRoutine();

        animationState = LittleGuyAnimationState.Death;

        animator.Play("Guy_Death", -1, 0f);

        if (!BattleManager.instance.encounteredLittleGuyStatPackages.Contains(information.StatPackage))
            BattleManager.instance.encounteredLittleGuyStatPackages.Add(information.StatPackage);


        runSwingHitboxes.SetCurrentHitbox(-1);
        heavySwingHitboxes.SetCurrentHitbox(-1);


        StartCoroutine(Despawn());
    }

    private IEnumerator Despawn()
    {
        yield return new WaitForSeconds(5);
        

        Destroy(gameObject);
        
        if (BattleManager.instance.littleGuys.Count == 0)
		{
            BattleManager.SwitchToNewState(BattleManager.BattleState.Cutscene);
        }
    }

    private void Update()
    {
        if (aiState == LittleGuyState.Attacking || aiState == LittleGuyState.Approaching)
        {
            float bossXDelta = opponent.transform.position.x - transform.position.x;

            if (bossXDelta > 1f)
                gameObject.transform.localScale = new Vector3(1, 1, 1);
            else if (bossXDelta < 1f)
                gameObject.transform.localScale = new Vector3(-1, 1, 1);
        }
        else
        {
            if (navMeshAgent.velocity.x > 1f)
                gameObject.transform.localScale = new Vector3(1, 1, 1);
            else if (navMeshAgent.velocity.x < 1f)
                gameObject.transform.localScale = new Vector3(-1, 1, 1);
        }

        transform.position += knockBack * Time.deltaTime;

        knockBack *= 0.96f;

        if (aiState != LittleGuyState.Dead && aiState != LittleGuyState.Staggered)
		{
            information.BattleStats.HP = Mathf.Min(information.BattleStats.MaxHP, information.BattleStats.HP + information.BattleStats.HealingPerSecond * Time.deltaTime);

        }

        UpdateAnimations();
    }

    // Animation transitions like hurt into other stuff
    public void OnAnimationEnd(string name)
    {
        if (name == "Hurt")
		{
            StartAIRoutine();
		}
        else if (name == "SwingHeavy")
		{
            animationState = LittleGuyAnimationState.Run; // restore control to coroutine
            animator.Play("Guy_Run");
        }
        else if (name == "Roll")
		{
            animationState = LittleGuyAnimationState.Idle;
            animator.Play("Guy_Idle");
		}
        else if (name == "Bomb")
		{
            animationState = LittleGuyAnimationState.Idle;
            animator.Play("Guy_Idle");
        }
        else if (name == "Potion")
        {
            animationState = LittleGuyAnimationState.Idle;
            animator.Play("Guy_Idle");
        }
    }

    public void OnAnimationMilestone(string message)
	{
        if (message == "SwingHeavyStart")
		{
            if (animationState == LittleGuyAnimationState.HeavySwing)
			{
                SFX.GuyHammerSwing.Play();
                heavySwingHitboxes.SetCurrentHitbox(0);
            }
            else if (animationState == LittleGuyAnimationState.CastSpell)
			{
                ShootProjectile();
			}
        }
        else if (message == "SwingHeavyEnd")
		{
            if (animationState == LittleGuyAnimationState.HeavySwing)
                heavySwingHitboxes.SetCurrentHitbox(-1);
        }
        if (message == "SwingRunStart")
        {
            SFX.GuySwordSwing.Play();
            runSwingHitboxes.SetCurrentHitbox(0);
        }
        else if (message == "SwingRunEnd")
        {
            runSwingHitboxes.SetCurrentHitbox(-1);
        }
        else if (message == "ThrowBomb")
		{
            ThrowBombProjectile();
		}
        else if (message == "DrinkPotion")
		{
            SFX.GuyPotionDrink.Play();
            information.BattleStats.HP = Mathf.Min(information.BattleStats.MaxHP, information.BattleStats.HP + information.BattleStats.MaxHP / 2);
		}
    }
    public void UpdateAnimations()
    {
        switch (aiState)
        {
            case LittleGuyState.Dead:
                break;
            case LittleGuyState.Staggered:
                break;
            case LittleGuyState.Attacking:
                break;
            default:
                if (animationState == LittleGuyAnimationState.Roll)
                {
                    // nothing
                }
                else if (animationState == LittleGuyAnimationState.Potion)
                {
                    //nothing
                }
                else if (animationState != LittleGuyAnimationState.Run && navMeshAgent.velocity.magnitude > 0.01f)
                {
                    animationState = LittleGuyAnimationState.Run;
                    animator.Play("Guy_Run");
                }
                else if (animationState != LittleGuyAnimationState.Idle && navMeshAgent.velocity.magnitude < 0.01f)
                {
                    animationState = LittleGuyAnimationState.Idle;
                    animator.Play("Guy_Idle");
                }

                break;
        }
    }

    public void ApplyKnockback(Vector3 direction)
    {
        direction.y = 0;

        knockBack += direction;
    }

    IEnumerator HitSoundCooldown()
	{
        canScream = false;

        yield return new WaitForSeconds(0.5f);

        canScream = true;
	}
}
