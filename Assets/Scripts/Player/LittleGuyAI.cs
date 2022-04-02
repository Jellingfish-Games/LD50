using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;

public class LittleGuyAI : MonoBehaviour
{
    private LittleGuyInformation information;
    private NavMeshAgent navMeshAgent;

    private NavMeshPath currentPath;
    private Vector3[] currentPathCorners;

    void Start()
    {
        currentPath = new NavMeshPath();

        information = GetComponent<LittleGuyInformation>();
        navMeshAgent = GetComponent<NavMeshAgent>();
    }

    public void RandomizeDestination()
	{
        currentPath = new NavMeshPath();

        NavMeshHit hit;

        NavMesh.SamplePosition(new Vector3(-10, 0, 0), out hit, 10000f, NavMesh.AllAreas);

        navMeshAgent.CalculatePath(hit.position, currentPath);
        currentPathCorners = currentPath.corners;

        StartCoroutine(FollowPath());
	}

    // Update is called once per frame
    void Update()
    {

    }

    IEnumerator FollowPath()
	{
        for (int i = 0; i < currentPathCorners.Length; i++)
		{
            transform.position = currentPathCorners[i];

            

            yield return new WaitForSeconds(2f);
        }
    }
}
