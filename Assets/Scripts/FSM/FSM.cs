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


    //주로 상위 Entity에서 호출
    public void ChangeState(EntityStates state, AIInput input)
    {
        if(states.TryGetValue(state, out IState nextState))
        {
            ChangeState(nextState, input);
        }
    }

    //주로 하위 State에서 호출
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
        //공격 가능 거리이면 공격
        if(input.DistanceToTarget() < attackRange)
        {
            return EntityStates.AttackState;
        }
        else if(input.DistanceToTarget() < chaseRange)
        {
            return EntityStates.ChaseState;
        }
        //추적 가능 거리이면 추적

        //모두 그렇지 않으면 제자리

        return EntityStates.IdleState;
    }

}
