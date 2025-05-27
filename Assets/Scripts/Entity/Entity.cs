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

    // memoryPool 관련
    private MemoryPool<Entity> memoryPool;

    // Ranking 관련
    private RankingManager rankingManager;

    private DamagePopupManager damagePopupManager;

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

        lastDamagedInfo = new KeyValuePair<Entity, string>(enemy, weaponName);

        damagePopupManager.PrintDamage(Color.black, amount, damageTextPoint.position, 3);

        // 2가지 이벤트, hpUI 수정
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



    private void OnEnable()
    {
        // 향후 Setup을 옮기기
        GetComponent<BoxCollider>().enabled = true;

        onDeath.AddListener(StartDespawnTimer);
        onDeath.AddListener(DeathLog);
    }
    private void OnDisable()
    {
        onDeath.RemoveListener(StartDespawnTimer);
        //Enable이 Setup보다 빨리 작동하여 AddEntity위치는 Setup으로 변경   
        rankingManager?.RemoveEntity(this);
    }

    public void DeathLog()
    {
        if (lastDamagedInfo.Key == null || lastDamagedInfo.Value == null)
        {
            return;
        }

        GameObject tmp = GameObject.Find("TmpKillLog");
        TextMeshProUGUI text = tmp.GetComponent<TextMeshProUGUI>();
        
        // 향후 무기 이미지 이용하여서 무기 이름 대신 교체하기
        text.text = $"{lastDamagedInfo.Key.Info.EntityName}가 {lastDamagedInfo.Value}로 {info.EntityName}을 죽였습니다.";
    }
};
