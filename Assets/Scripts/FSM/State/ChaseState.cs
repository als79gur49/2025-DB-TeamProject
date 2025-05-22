using UnityEngine;
using UnityEngine.AI;

public class ChaseState : IState
{
    private NavMeshAgent agent;
    private GameObject target;
    public void Enter(AIInput input)
    {
        agent = input.self.GetComponent<NavMeshAgent>();
        target = input.target;

        agent.SetDestination(target.transform.position);
        agent.isStopped = false;

        Debug.Log("Chase Enter");
    }

    public void Execute(AIInput input)
    {
        agent.SetDestination(target.transform.position);

        Debug.Log("Chase Execute");
    }

    public void Exit(AIInput input)
    {
        agent.isStopped = true;

        Debug.Log("Chase Exit");
    }
}
