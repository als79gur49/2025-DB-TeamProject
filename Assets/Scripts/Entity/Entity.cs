using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


public abstract class Entity : MonoBehaviour, IAttack, IDamageable
{
    // event
    public UnityEvent<int, int> onTakeDamage;
    public UnityEvent onDeath;

    // FSM ����
    private AIInput input;
    private FSM brain;
    //private EntityOutput output;

    // Animation ����
    private EntityAnimation animation;

    // data ����
    private EntityInfo info;
    protected EntityData data;

    // memoryPool ����
    private MemoryPool<Entity> memoryPool;

    // Ranking ����
    private RankingManager rankingManager;

    public EntityData Data => data;
    public EntityInfo Info => info;

    public MemoryPool<Entity> MemoryPool => memoryPool;
    private DamagePopupManager damagePopupManager;

    [SerializeField]
    private Transform damageTextPoint;
    [SerializeField]
    private Transform firePoint;
    [SerializeField]
    private GameObject projectile;


    public bool IsDead => (data.HP <= 0);


    public void Setup(MemoryPool<Entity> memoryPool,RankingManager rankingManager, DamagePopupManager damagePopupManager,EntityInfo info, EntityData data)
    {
        this.info = info;
        this.data = data;

        this.memoryPool = memoryPool;
        this.rankingManager = rankingManager;
        rankingManager?.AddEntity(this);
        this.damagePopupManager = damagePopupManager;

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

        clone.TryGetComponent<ProjectileMove>(out ProjectileMove p);
        p.Setup(this.gameObject);

    }

    // IDamageable
    public void TakeDamage(int amount)
    {
        int prevHp = data.HP;

        //defense �߰��� ���� ���� ����
        data.TakeDamage(amount);

        damagePopupManager.PrintDamage(Color.black, amount, damageTextPoint.position, 3);

        //2���� �̺�Ʈ, hpUI ����, ������ ���
        onTakeDamage?.Invoke(prevHp, data.HP);
    }

    //DeadState�� MonoBehavior�� ��� Coroutine ����� ���� �ʾƼ� �ӽ÷� Entity�� �Ű܊R.
    public void StartDespawnTimer()
    {
        StartCoroutine(ReturnToPoolAfterDelay(5));
    }

    private IEnumerator ReturnToPoolAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        // �޸� Ǯ�� ��ȯ
        memoryPool.DeactivatePoolItem(this);
    }



    private void OnEnable()
    {
        // ���� Setup�� �ű��
        GetComponent<BoxCollider>().enabled = true;

        onDeath.AddListener(StartDespawnTimer);
    }
    private void OnDisable()
    {
        onDeath.RemoveListener(StartDespawnTimer);
        //Enable�� Setup���� ���� �۵��Ͽ� AddEntity��ġ�� Setup���� ����   
        rankingManager?.RemoveEntity(this);
    }
};
