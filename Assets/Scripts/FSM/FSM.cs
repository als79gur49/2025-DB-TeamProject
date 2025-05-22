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
    private int attackRange = 5;
    [SerializeField]
    private int chaseRange = 35;

    public int AttackRange => attackRange;
    public int ChaseRange => chaseRange;

    public void Setup(Entity owner)
    {
        this.owner = owner;

        states = new Dictionary<EntityStates, IState>();
        states.TryAdd(EntityStates.IdleState, new IdleState(3));
        states.TryAdd(EntityStates.PatrolState, new PatrolState(owner.gameObject, 20, 3));
        states.TryAdd(EntityStates.AttackState, new AttackState());
        states.TryAdd(EntityStates.ChaseState, new ChaseState());

        transitions = new Dictionary<EntityStates, ITransition>();
        transitions.TryAdd(EntityStates.ChaseState, new ChaseTransition(EntityStates.ChaseState));
        transitions.TryAdd(EntityStates.IdleState, new IdleTransition(EntityStates.IdleState));


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
        EntityStates nextState = EntityStates.Default;
        
        //전환조건 체크
        foreach(var t in transitions)
        {
            if (t.Value.CheckTransition(this, input) &&
                states.ContainsKey(t.Key))
            {
                nextState = t.Key;

                break;
            }
        }

        //다른 상태일 경우 변경
        if(currentState != states[nextState])
        {
            ChangeState(nextState, input);
        }

        if(currentState != null)
        {
            currentState.Execute(input);
        }
    }
}
