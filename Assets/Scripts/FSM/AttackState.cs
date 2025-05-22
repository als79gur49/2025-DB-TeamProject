using UnityEngine;

public class AttackState : IState
{
    private float cooldown = 1f;
    private float timer;

    public void Enter(AIInput input)
    {
        timer = 0;
        Debug.Log("Attack Enter");
    }

    public void Execute(AIInput input)
    {
        timer += Time.deltaTime;

        if(timer > cooldown)
        {
            //attack

            timer = 0;
        }

        

        Debug.Log("Attack Execute");
    }

    public void Exit(AIInput input)
    {
        Debug.Log("Attack Exit");
    }

}
