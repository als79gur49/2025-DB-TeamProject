using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EntityStates
{
    Default = -1,
    IdleState = 0,
    PatrolState,
    AttackState,
    ChaseState
}


public class FSM : MonoBehaviour
{
    private Entity owner;

    private Dictionary<EntityStates, IState> states;
    private Dictionary<EntityStates, ITransition> transitions;
    private IState currentState;   

    [SerializeField]
    private int attackRange = 7;
    [SerializeField]
    private int chaseRange = 30;

    public int AttackRange => attackRange;
    public int ChaseRange => chaseRange;

    public void Setup(Entity owner)
    {
        this.owner = owner;

        states = new Dictionary<EntityStates, IState>();
        states.TryAdd(EntityStates.IdleState, new IdleState(3));
        states.TryAdd(EntityStates.PatrolState, new PatrolState(20, 3));
        states.TryAdd(EntityStates.AttackState, new AttackState());
        states.TryAdd(EntityStates.ChaseState, new ChaseState());

        //���� ������ ��� ���� ������� �˻�. Idle, Patorl�� ������ true�̱⿡ Patrol ���۵�
        transitions = new Dictionary<EntityStates, ITransition>();
        transitions.TryAdd(EntityStates.AttackState, new AttackTransition(EntityStates.AttackState, attackRange));
        transitions.TryAdd(EntityStates.ChaseState, new ChaseTransition(EntityStates.ChaseState, chaseRange));
        transitions.TryAdd(EntityStates.PatrolState, new PatrolTransition(EntityStates.PatrolState));
        transitions.TryAdd(EntityStates.IdleState, new IdleTransition(EntityStates.IdleState));


        currentState = states[EntityStates.IdleState];
    }


    //�ַ� ���� Entity���� ȣ��
    public void ChangeState(EntityStates state, AIInput input)
    {
        if(states.TryGetValue(state, out IState nextState))
        {
            ChangeState(nextState, input);
        }
    }

    //�ַ� ���� State���� ȣ��
    public void ChangeState(IState nextState, AIInput input)
    {
        currentState?.Exit(input);
        currentState = nextState;
        currentState?.Enter(input);
    }

    public void Execute(AIInput input)
    {
        EntityStates nextState = EntityStates.Default;
        
        //��ȯ���� üũ
        foreach(var t in transitions)
        {
            if (t.Value.CheckTransition(input) &&
                states.ContainsKey(t.Key))
            {
                nextState = t.Key;

                break;
            }
        }

        //�ٸ� ������ ��� ����
        if(currentState != states[nextState])
        {
            ChangeState(nextState, input);
        }

        if(currentState != null)
        {
            currentState.Execute(input);
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, chaseRange);
    }
}
