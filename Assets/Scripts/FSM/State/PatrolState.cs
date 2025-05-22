using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class PatrolState : IState
{
    GameObject entity;

    private NavMeshAgent agent;
    private int wanderRadius;
    private int length = 1;
    private float wanderTime = 4;
    private float timer;

    private Vector3 wanderPosition;

    public PatrolState(GameObject entity, int wanderRadius, float wanderTime)
    {
        this.entity = entity;
        agent = entity.GetComponent<NavMeshAgent>();
        this.wanderRadius = wanderRadius;
        this.wanderTime = wanderTime;
    }
    public void Enter(AIInput input)
    {
        agent.isStopped = false;

        timer = 0;
        wanderPosition = entity.transform.position + Utils.RandomPositionFromRadius(wanderRadius);

        agent.SetDestination(wanderPosition);
        Debug.Log("Patrol Enter");
    }

    public void Execute(AIInput input)
    {
        timer += Time.deltaTime;

        OnDrawGizmos();

        if(Utils.IsNearTarget(wanderPosition, entity.transform.position, length) ||
            timer >= wanderTime)
        {
        }

        Debug.Log("Patrol Execute");
    }

    public void Exit(AIInput input)
    {
        agent.isStopped = true;

        Debug.Log("Patrol Exit");
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(wanderPosition + new Vector3(0, 20, 0), entity.transform.position + new Vector3(0, 20, 0));

    }
}
