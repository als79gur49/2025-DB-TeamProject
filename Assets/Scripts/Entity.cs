using System.Collections.Generic;
using UnityEngine;


public abstract class Entity : MonoBehaviour, IAttack
{
    private AIInput input;
    private FSM brain;
    //private EntityOutput output;

    private EntityAnimation animation;

    private EntityInfo info;
    protected EntityData data;

    private RankingManager rankingManager;

    public EntityData Data => data;
    public EntityInfo Info => info;

    [SerializeField]
    private Transform firePoint;
    [SerializeField]
    private GameObject projectile;

    public void Setup(RankingManager rankingManager, EntityInfo info, EntityData data)
    {
        this.info = info;
        this.data = data;

        this.rankingManager = rankingManager;
        rankingManager?.AddEntity(this);

        Setup();
    }

    private void Setup()
    {
        input = GetComponent<AIInput>();
        input.self = this.gameObject;
        if (TryGetComponent<FSM>(out FSM brain))
        {
            this.brain = brain;
            this.brain.Setup(this);
        }

        animation = GetComponent<EntityAnimation>();
    }

    private void Update()
    {
        brain.Execute(input);
    }

    public void ChangeState(EntityStates nextState)
    {
        brain.ChangeState(nextState, input);
    }
    private void OnDisable()
    {
        //Enable�� Setup���� ���� �۵��Ͽ� AddEntity��ġ�� Setup���� ����   
        rankingManager?.RemoveEntity(this);
    }

    public void Attack()
    {
        GameObject clone;

        if (firePoint == null || projectile == null)
        {
            Debug.Log("�߻� ��ġ Ȥ�� ����ü x");

            return;
        }

        clone = Instantiate(projectile, firePoint.position, Quaternion.identity);
        clone.transform.localRotation = Quaternion.LookRotation(transform.forward, Vector3.up);
    }
};
