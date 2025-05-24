using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeadState : IState
{
    public DeadState()
    {
    }

    public void Enter(AIInput input)
    {
        if (input.Entity.TryGetComponent<BoxCollider>(out var collider))
        {
            collider.enabled = false;
        }

        input.Animation.Death();

        input.Entity.onDeath?.Invoke();

        Debug.Log("Dead Enter");
    }

    public void Execute(AIInput input)
    {
        Debug.Log("Dead Execute");
    }

    public void Exit(AIInput input)
    {
        Debug.Log("Dead Exit");
    }

}
