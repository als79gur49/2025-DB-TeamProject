using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EntityAnimation : MonoBehaviour
{
    private Animator animator;
    private NavMeshAgent agent;

    private string speed = "Speed_f";

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

    private void Update()
    {
        float tmpSpeed = agent.velocity.magnitude;
        Debug.Log("현재 속도: " + tmpSpeed);
        animator.SetFloat(speed, tmpSpeed);
    }
}
