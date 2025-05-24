using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EntityAnimation : MonoBehaviour
{
    private Animator animator;
    private NavMeshAgent agent;

    private string speed = "Speed_f";
    private string death = "Death_b";
    private string deathType = "DeathType_int";
    private void Awake()
    {
        animator = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();
    }

    public void Setup(Animator animator, NavMeshAgent agent)
    {
        this.animator = animator;
        this.agent = agent;
    }

    public void Death()
    {
        animator.SetInteger(deathType, Random.Range(1, 3));
        animator.SetBool(death, true);
    }

    public void SetRun()
    {
        SetSpeed(1);
    }
    public void SetWalk()
    {
        SetSpeed(0.5f);
    }
    public void SetIdle()
    {
        SetSpeed(0);
    }

    private void SetSpeed(float moveSpeed)
    {
        animator.SetFloat(speed, moveSpeed);
    }
}
