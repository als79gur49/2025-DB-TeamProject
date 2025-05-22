using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EntityStates
{
    IdleState = 0,
    PatrolState,
    AttackState,
    ChaseState
}


public class FSM : MonoBehaviour
{
    private Dictionary<EntityStates, IState> states;

    private IState currentState;

    private Entity owner;

    [SerializeField]
    private int attackRange = 5;
    [SerializeField]
    private int chaseRange = 35;


    public void Setup(Entity owner)
    {
        this.owner = owner;

        states = new Dictionary<EntityStates, IState>();
        states.TryAdd(EntityStates.IdleState, new IdleState(3));
        states.TryAdd(EntityStates.PatrolState, new PatrolState(owner.gameObject, 20, 3));
        states.TryAdd(EntityStates.AttackState, new AttackState());
        states.TryAdd(EntityStates.ChaseState, new ChaseState());

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
        EntityStates nextState = DecideNetxtState(input);   

        if(currentState != states[nextState])
        {
            ChangeState(nextState, input);
        }

        if (currentState != null)
        {
            currentState.Execute(input);
        }
    }
    
    public EntityStates DecideNetxtState(AIInput input)
    {
        //���� ���� �Ÿ��̸� ����
        if(input.DistanceToTarget() < attackRange)
        {
            return EntityStates.AttackState;
        }
        else if(input.DistanceToTarget() < chaseRange)
        {
            return EntityStates.ChaseState;
        }
        //���� ���� �Ÿ��̸� ����

        //��� �׷��� ������ ���ڸ�

        return EntityStates.IdleState;
    }

}
