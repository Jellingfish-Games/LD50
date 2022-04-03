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
        Roll
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

        if (navMeshAgent.SamplePathPosition(NavMesh.GetAreaFromName("Danger"), cautiousness, out NavMeshHit navMeshHitInfo))
        {
            dodgeDirectionFromNavMesh = transform.position - navMeshHitInfo.position;
            dodgeDirectionFromNavMesh.y = 0;
            dodgeDirectionInfluence = 1;
            safety = 0;
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
            else if (aiState == LittleGuyState.Fleeing)
            {
                Vector3 bossDirection = opponent.transform.position - transform.position;

                bossDirection.y = 0;
                bossDirection = bossDirection.normalized * Random.Range(2f, 5f - aggressiveness + cautiousness);

                if (Random.Range(0f, 1f) - cautiousness * 0.2f < 0.4f)
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

                bool wantsToMove = diceRoll + 0.15f * aggressiveness > 0.25f;

                if (wantsToMove)
                {
                    if (diceRoll + aggressiveness - cautiousness > 0.4f)
                    {
                        aiState = LittleGuyState.Approaching;
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
        yield return null;
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
        InterruptAIRoutine();

        animationState = LittleGuyAnimationState.Hurt;

        aiState = LittleGuyState.Staggered;

        safety = (information.BattleStats.Aggressiveness + information.BattleStats.Awareness) / 2;

        animator.Play("Guy_Hurt", -1, 0f);
    }

    public void Die()
    {
        InterruptAIRoutine();

        animationState = LittleGuyAnimationState.Death;

        aiState = LittleGuyState.Dead;

        animator.Play("Guy_Death", -1, 0f);
        StartCoroutine(Despawn());
    }

    private IEnumerator Despawn()
    {
        yield return new WaitForSeconds(5);
        Destroy(gameObject);
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
}
