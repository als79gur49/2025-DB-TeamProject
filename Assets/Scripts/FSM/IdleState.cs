using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IdleState : IState
{
    private float idleTime = 3f;
    private float timer;

    public IdleState(float idleTime)
    {
        this.idleTime = idleTime;
    }

    public void Enter(Entity entity)
    {
        timer = 0;
        Debug.Log("Idle Enter");
    }

    public void Execute(Entity entity)
    {
        timer += Time.deltaTime;

        if(timer >= idleTime)
        {
            entity.ChangeState(EntityStates.PatrolState);
        }
        
        Debug.Log("Idle Execute");
    }

    public void Exit(Entity entity)
    {
        Debug.Log("Idle Exit");
    }

}
