using System.Collections;
using System.Collections.Generic;
using TMPro;
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

    private MemoryPool<Entity> memoryPool;
    private RankingManager rankingManager;
    private DamagePopupManager damagePopupManager;
    private KillLogManager killLogManager;

    public EntityData Data => data;
    public EntityInfo Info => info;

    public MemoryPool<Entity> MemoryPool => memoryPool;

    [SerializeField]
    private Transform damageTextPoint;
    [SerializeField]
    private Transform firePoint;

    [SerializeField]
    private GameObject projectile;

    public bool IsDead => (data.HP <= 0);

    // �������� ���ݹ��� ���� �̸�, ���� �̸�
    private KeyValuePair<Entity, string> lastDamagedInfo;

    private bool isInitialized = false;

    public void Setup(
        EntityInfo info,
        EntityData data,
        MemoryPool<Entity> memoryPool = null, RankingManager rankingManager = null,
        DamagePopupManager damagePopupManager = null, KillLogManager killLogManager = null)
    {
        if (!isInitialized)
        {
            this.damagePopupManager = damagePopupManager;
            this.killLogManager = killLogManager;
            this.memoryPool = memoryPool;
            this.rankingManager = rankingManager;

            Setup();

            isInitialized = true;
        }

        Init(info, data);

        GetComponent<BoxCollider>().enabled = true;

        onDeath.AddListener(StartDespawnTimer); // n�� �� memoryPool�� ��ȯ
        onDeath.AddListener(DeathLog); // lastDamagedInfo, KillLogManager 
        onDeath.AddListener(GiveScoreToLastAttacker); // lastDamagedInfo, data
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

    private void Init(EntityInfo info, EntityData data)
    {
        this.info = info;
        this.data = data;

        Init();
    }
    private void Init()
    {
        rankingManager?.AddEntity(this);
        brain.Init();
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
    public void TakeDamage(int amount, Entity enemy, string weaponName)
    {
        int prevHp = data.HP;

        // defense �߰��� ���� ���� ����
        data.TakeDamage(amount);

        // ������ ������ ����� ����
        lastDamagedInfo = new KeyValuePair<Entity, string>(enemy, weaponName);

        damagePopupManager.PrintDamage(Color.black, amount, damageTextPoint.position, 3);

        // hpUI ����
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

    private void OnDisable()
    {
        onDeath.RemoveAllListeners();

        rankingManager?.RemoveEntity(this);
    }

    // ų�α� ��� ex) x�� y�� z�� óġ
    public void DeathLog()
    {
        if (lastDamagedInfo.Key == null || lastDamagedInfo.Value == null)
        {
            return;
        }

        KillLog log = new KillLog(info.EntityName, lastDamagedInfo.Value, lastDamagedInfo.Key.Info.EntityName);
        killLogManager.AddLog(log);
    }

    // óġ�� ������ ���� �ο�
    public void GiveScoreToLastAttacker()
    {
        GiveScoreToEnemy(lastDamagedInfo.Key);

        rankingManager.UpdateEntity(lastDamagedInfo.Key);
    }
    private void GiveScoreToEnemy(Entity enemy)
    {
        enemy?.data.AddScore(100);
    }
};
