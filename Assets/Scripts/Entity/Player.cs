using UnityEngine;
using UnityEngine.AI;
using System.Collections.Generic;

public class Player : Entity
{
    //input  데이터 입력
    //brain  데이터 계산
    //output 행동
    private NavMeshAgent agent;
    private Dictionary<KeyCode, Vector3> arrowVector;

    protected override void Setup()
    {
        base.Setup();

        agent = GetComponent<NavMeshAgent>();

        arrowVector = new Dictionary<KeyCode, Vector3>()
        {
            {KeyCode.UpArrow, Vector3.forward},
            {KeyCode.RightArrow, Vector3.right},
            {KeyCode.DownArrow, Vector3.back},
            {KeyCode.LeftArrow, Vector3.left}
        };
    }

    private void Update()
    {
        Move(); 
        Attack();

        if(data.HP <= 0)
        {
            if (TryGetComponent<BoxCollider>(out var collider))
            {
                collider.enabled = false;
            }

            animation.Death();

            onDeath?.Invoke();
        }
    }

    private void Move()
    {
        // 현재 위치에서 입력된 방향으로 이동
        Vector3 moveDirection = Vector3.zero;
        foreach (var pair in arrowVector)
        {
            if(Input.GetKey(pair.Key))
            {                
                moveDirection += pair.Value;
            }
        }

        if(moveDirection == Vector3.zero)
        {
            animation.SetIdle();
            agent.isStopped = true;
        }
        else if(Input.GetKey(KeyCode.LeftShift))
        {
            animation.SetRun();
            agent.isStopped = false;
            agent.SetDestination(transform.position + moveDirection);
            agent.speed = 2.5f;
        }
        else
        {
            animation.SetWalk();
            agent.isStopped = false;
            agent.SetDestination(transform.position + moveDirection);
            agent.speed = 1.3f;
        }
    }
}