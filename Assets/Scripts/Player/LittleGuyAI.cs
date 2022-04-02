using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;

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
        float aggressiveness = information.BattleStats.Aggressiveness;
        float bossPercentageHP = opponent.hp / opponent.maxHP;
        float cautiousness = 1;
        Vector3 dodgeDirectionFromNavMesh = Vector3.zero;
        float dodgeDirectionInfluence = 0;
        BossCharacter.BossState bossState = opponent.state;

        float diceRoll = Random.Range(0.0f, 1.0f);

        float desiredRoamDistance = 3 - 2 * aggressiveness + cautiousness;

        if (navMeshAgent.SamplePathPosition(NavMesh.GetAreaFromName("Danger"), cautiousness, out NavMeshHit navMeshHitInfo))
        {
            dodgeDirectionFromNavMesh = transform.position - navMeshHitInfo.position;
            dodgeDirectionFromNavMesh.y = 0;
            dodgeDirectionInfluence = 1;
        }
        else if(bossState == BossCharacter.BossState.Windup && dodgeDirectionInfluence * diceRoll * reactionSpeed > 0.25f)
        {
            yield return DodgeRoll(dodgeDirectionFromNavMesh.normalized);
        } else
        {
            bool wantsToMove = diceRoll > 0.5f;

            if (wantsToMove)
            {
                yield return MoveToDirection((directionToBoss * (distanceToBoss - desiredRoamDistance)).normalized);
            }
            else
            {
                // Don't do anything.
                yield return reactionSpeed;
            }
        }
    }

    private IEnumerator DodgeRoll(Vector3 dodgeDirection)
    {
        yield return MoveToDirection(dodgeDirection * 3);
    }

    IEnumerator DefaultLoop()
    {
        while (true)
        {
            yield return ThinkingFunction();
        }
    }

    public void RandomizeDestination()
	{
        StartCoroutine(MoveToPosition(new Vector3(-10, 0, 0)));
    }

    private IEnumerator MoveToDirection(Vector3 target)
    {
        yield return MoveToPosition(transform.position + target);
    }

    private IEnumerator MoveToPosition(Vector3 target)
    {
        currentPath = new NavMeshPath();

        NavMeshHit hit;

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

        animator.Play("Guy_Hurt", -1, 0f);
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
                if (animationState != LittleGuyAnimationState.Run && navMeshAgent.velocity.magnitude > 0.01f)
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
