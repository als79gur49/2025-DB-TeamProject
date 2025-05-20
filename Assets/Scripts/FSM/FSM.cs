using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EntityStates
{
    IdleState = 0,
    PatrolState,
}


public class FSM : MonoBehaviour
{
    private Dictionary<EntityStates, IState> states;

    private IState currentState;

    private Entity owner;

    public void Setup(Entity owner)
    {
        this.owner = owner;

        states = new Dictionary<EntityStates, IState>();
        states.TryAdd(EntityStates.IdleState, new IdleState(3));
        states.TryAdd(EntityStates.PatrolState, new PatrolState(owner.gameObject, 20, 3));
    }

    //�ַ� ���� Entity���� ȣ��
    public void ChangeState(EntityStates state)
    {
        if(states.TryGetValue(state, out IState nextState))
        {
            ChangeState(nextState);
        }
    }

    //�ַ� ���� State���� ȣ��
    public void ChangeState(IState nextState)
    {
        currentState?.Exit(owner);
        currentState = nextState;
        currentState?.Enter(owner);
    }

    public void Execute()
    {
        if (currentState != null)
        {
            currentState.Execute(owner);
        }
    }

}
