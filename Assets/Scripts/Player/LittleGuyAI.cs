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
    }

    private LittleGuyState aiState;
    private LittleGuyInformation information;
    private NavMeshAgent navMeshAgent;

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
    }

    public void StartAIRoutine()
    {
        InterruptAIRoutine();
        aiRoutine = StartCoroutine(DefaultLoop());
    }

    public void InterruptAIRoutine()
    {
        if (aiRoutine != null)
        {
            StopCoroutine(aiRoutine);
            aiRoutine = null;
        }
    }

    private IEnumerator ThinkingFunction()
    {
        if (opponent == null)
        {
            opponent = BattleManager.instance.player;
        }
        Vector3 directionToBoss = (transform.position - opponent.transform.position);
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

        float desiredRoamDistance = 6 - 2 * aggressiveness + cautiousness;

        if (navMeshAgent.SamplePathPosition(NavMesh.GetAreaFromName("Danger"), cautiousness, out NavMeshHit navMeshHitInfo))
        {
            dodgeDirectionFromNavMesh = transform.position - navMeshHitInfo.position;
            dodgeDirectionFromNavMesh.y = 0;
            dodgeDirectionInfluence = 1;
        }

        if (bossState == BossCharacter.BossState.Windup && dodgeDirectionInfluence * diceRoll * reactionSpeed > 0.25f)
        {
            yield return DodgeRoll(dodgeDirectionFromNavMesh.normalized);
        }

        bool wantsToMove = diceRoll > 0.5f;

        if (wantsToMove)
        {
            yield return MoveToDirection((directionToBoss * (distanceToBoss - desiredRoamDistance)).normalized);
        }

        // Don't do anything.
        yield return reactionSpeed;
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

        for (int i = 0; i < currentPathCorners.Length; i++)
		{
            transform.position = currentPathCorners[i];

            yield return new WaitForSeconds(2f);
        }
    }
}
