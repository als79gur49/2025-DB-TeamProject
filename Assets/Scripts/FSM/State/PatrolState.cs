using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using static UnityEngine.EventSystems.EventTrigger;

public class PatrolState : IState
{
    private NavMeshAgent agent;
    private int wanderRadius;
    private int threshold = 1;
    private float wanderTime = 4;
    private float timer;

    private Vector3 wanderPosition;

    public PatrolState(int wanderRadius, float wanderTime)
    {
        this.wanderRadius = wanderRadius;
        this.wanderTime = wanderTime;
    }
    public void Enter(AIInput input)
    {
        agent = input.self.GetComponent<NavMeshAgent>();
        agent.isStopped = false;

        timer = 0;
        wanderPosition = input.self.transform.position + Utils.RandomPositionFromRadius(wanderRadius);

        agent.SetDestination(wanderPosition);
        Debug.Log("Patrol Enter");
    }

    public void Execute(AIInput input)
    {
        timer += Time.deltaTime;
        if(Utils.IsNearTarget(agent.destination, input.self.transform.position, threshold) ||
            timer >= wanderTime)
        {
            Random.InitState(Random.Range(int.MinValue, int.MaxValue));
            agent.SetDestination(Utils.RandomPositionFromRadius(wanderRadius, input.self.transform.position));

            timer = 0;
        }
        Debug.Log("Patrol Execute");
    }

    public void Exit(AIInput input)
    {
        agent.isStopped = true;

        Debug.Log("Patrol Exit");
    }

}
