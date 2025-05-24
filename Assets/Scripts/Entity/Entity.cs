using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


public abstract class Entity : MonoBehaviour, IAttack, IDamageable
{
    public UnityEvent<int, int> onTakeDamage;
    public UnityEvent onDeath;

    private AIInput input;
    private FSM brain;
    //private EntityOutput output;

    private EntityAnimation animation;

    private EntityInfo info;
    protected EntityData data;
    public bool IsDead => (data.HP <= 0);

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
        if (TryGetComponent<FSM>(out FSM brain))
        {
            this.brain = brain;
            this.brain.Setup(this);
        }

        animation = GetComponent<EntityAnimation>();

        input = GetComponent<AIInput>();
        input.self = this.gameObject;
        input.SetEntity(this);
        input.SetAnimation(animation);      
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

    // IAttack
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

    // IDamageable
    public void TakeDamage(int amount)
    {
        int prevHp = data.HP;

        //defense �߰��� ���� ���� ����
        data.TakeDamage(amount);

        //2���� �̺�Ʈ, hpUI ����, ������ ���
        onTakeDamage?.Invoke(prevHp, data.HP);
    }
};
