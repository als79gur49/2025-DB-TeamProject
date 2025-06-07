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
    private LayerMask groundLayer = 1 << 8;
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
        if(IsDead)
        {
            return;
        }

        Move();
        RotateToMouse();
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
        else if(Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
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

    private void RotateToMouse()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit, 100f))
        {
            Vector3 lookPos = hit.point - transform.position;
            lookPos.y = 0; // 수평 회전만
            if (lookPos != Vector3.zero)
            {
                Quaternion targetRotation = Quaternion.LookRotation(lookPos);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, 10f * Time.deltaTime);
            }

            float angle = Vector3.Angle((agent.destination - transform.position).normalized, lookPos.normalized);
            
            if(angle <= 90f)
            {
                //animation.SetForwardLoop();
            }
            else
            {
                //animation.SetBackwardLoop();
            }
        }

        
    }

    protected override void levelup()
    {
    }
}