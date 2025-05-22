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

    public void Enter(AIInput input)
    {
        timer = 0;
        Debug.Log("Idle Enter");
    }

    public void Execute(AIInput input)
    {
        timer += Time.deltaTime;

        if(timer >= idleTime)
        {
        }
        
        Debug.Log("Idle Execute");
    }

    public void Exit(AIInput input)
    {
        Debug.Log("Idle Exit");
    }

}
