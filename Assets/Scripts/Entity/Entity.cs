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

    // FSM 관련
    private AIInput input;
    private FSM brain;
    //private EntityOutput output;

    // Animation 관련
    private EntityAnimation animation;

    // data 관련
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

    // 마지막을 공격받은 적의 이름, 무기 이름
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

        onDeath.AddListener(StartDespawnTimer); // n초 후 memoryPool로 반환
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
            Debug.Log("발사 위치 혹은 투사체 x");

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

        // defense 추가할 꺼면 로직 수정
        data.TakeDamage(amount);

        // 마지막 공격한 상대의 정보
        lastDamagedInfo = new KeyValuePair<Entity, string>(enemy, weaponName);

        damagePopupManager.PrintDamage(Color.black, amount, damageTextPoint.position, 3);

        // hpUI 수정
        onTakeDamage?.Invoke(prevHp, data.HP);
    }

    //DeadState가 MonoBehavior가 없어서 Coroutine 사용이 되지 않아서 임시로 Entity로 옮겨둚.
    public void StartDespawnTimer()
    {
        StartCoroutine(ReturnToPoolAfterDelay(5));
    }

    private IEnumerator ReturnToPoolAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        // 메모리 풀로 반환
        memoryPool.DeactivatePoolItem(this);
    }

    private void OnDisable()
    {
        onDeath.RemoveAllListeners();

        rankingManager?.RemoveEntity(this);
    }

    // 킬로그 출력 ex) x가 y로 z를 처치
    public void DeathLog()
    {
        if (lastDamagedInfo.Key == null || lastDamagedInfo.Value == null)
        {
            return;
        }

        KillLog log = new KillLog(info.EntityName, lastDamagedInfo.Value, lastDamagedInfo.Key.Info.EntityName);
        killLogManager.AddLog(log);
    }

    // 처치한 적에게 점수 부여
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
