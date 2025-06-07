using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;


public abstract class Entity : MonoBehaviour, IAttack, IDamageable
{
    // event
    public UnityEvent<float, float> onTakeDamage;
    public UnityEvent onDeath;

    // Animation 관련
    protected EntityAnimation animation;

    // data 관련
    private EntityInfo info;
    protected EntityData data;

    private RankingManager rankingManager;
    private DamagePopupManager damagePopupManager;
    private KillLogManager killLogManager;
    private ScoreBlockSpawner scoreBlockSpawner;

    // 소유 중인 무기 및 공격 클래스
    private WeaponBase weapon;
    private ProjectileStorage projectileStorage;
    [SerializeField]
    private List<Projectile> storages; // 해당 내용은 임시로 투사체 넣어둔 곳 실제로는 외부에서 레벨 업 등을 통해서 projectileStorage에 넣어주기

    // 이름, 공격력, 스코어 등
    public EntityData Data => data;
    public EntityInfo Info => info;

    [SerializeField]
    private Transform damageTextPoint;
    [SerializeField]
    private Transform firePoint;
    [SerializeField]
    private SOWeapon startWeapon;

    [SerializeField]
    private GameObject levelupPrefab;

    public bool IsDead { get => data.HP <= 0; }

    // 마지막을 공격받은 적의 이름, 무기 이름
    private KeyValuePair<Entity, string> lastDamagedInfo;


    public void Setup(EntityInfo info, EntityData data,
        RankingManager rankingManager = null, DamagePopupManager damagePopupManager = null, KillLogManager killLogManager = null, ScoreBlockSpawner scoreBlockSpawner = null)
    {  
        this.damagePopupManager = damagePopupManager;
        this.killLogManager = killLogManager;
        this.rankingManager = rankingManager;
        this.scoreBlockSpawner = scoreBlockSpawner;

        Setup();

        this.info = info;
        this.data = data;
        rankingManager?.AddEntity(this);

        GetComponent<BoxCollider>().enabled = true;

        onDeath.AddListener(DeathLog); // lastDamagedInfo, KillLogManager 
        onDeath.AddListener(GiveScoreToLastAttacker); // lastDamagedInfo, data
        onDeath.AddListener(RemoveFromRanking); // rankingManager에서 제거
        onDeath.AddListener(SpawnLevelupBlocks); // 죽으면 경험치블럭들 생성
    }
    protected virtual void Setup()
    {
        animation = GetComponent<EntityAnimation>();

        weapon = GetComponent<WeaponBase>();
        weapon.Setup(startWeapon, this, firePoint);

        projectileStorage = GetComponent<ProjectileStorage>();
        projectileStorage.Setup(storages);
    }

    // IAttack
    public void Attack()
    {
        weapon?.Shot(projectileStorage);
    }

    // IDamageable
    public void TakeDamage(float amount, Entity enemy, string weaponName)
    {
        float prevHp = data.HP;

        // defense 추가할 꺼면 로직 수정
        data.TakeDamage(amount);
        Debug.Log($"{enemy.Info.EntityName}가 {info.EntityName}에게 {weaponName}으로 {amount}만큼의 피해 입힘");

        // 마지막 공격한 상대의 정보
        lastDamagedInfo = new KeyValuePair<Entity, string>(enemy, weaponName);

        damagePopupManager.PrintDamage(Color.black, amount, damageTextPoint.position, 3);

        // hpUI 수정
        onTakeDamage?.Invoke(data.HP, data.MaxHp);
    }

    private void OnDisable()
    {
        onDeath.RemoveAllListeners();
    }

    // 킬로그 출력 ex) x가 y로 z를 처치
    private void DeathLog()
    {
        if (lastDamagedInfo.Key == null || lastDamagedInfo.Value == null)
        {
            return;
        }

        KillLog log = new KillLog(info.EntityName, lastDamagedInfo.Value, lastDamagedInfo.Key.Info.EntityName);
        killLogManager.AddLog(log);
    }

    // 처치한 적에게 점수 부여
    private void GiveScoreToLastAttacker()
    {
        if(lastDamagedInfo.Key != null)
        {
            lastDamagedInfo.Key.AddScore(100);

            rankingManager.UpdateEntity(lastDamagedInfo.Key);
        }
    }
    public void AddScore(int amount)
    {
        const int levelupAmount = 1000;

        int remainExp = data.Score % levelupAmount;
        int levelupNum = (remainExp + amount) / levelupAmount;
        data.AddScore(amount);

        for(int i = 0; i < levelupNum; ++i)
        {
            //Do Levelup
            if(levelupPrefab != null)
            {
                GameObject clone = Instantiate(levelupPrefab, transform.position, Quaternion.identity);
                clone.transform.localScale *= 1.5f;
                clone.transform.SetParent(transform, true);
                Destroy(clone, 5f);
            }
            levelup();
        }

    }
    protected abstract void levelup();

    private void SpawnLevelupBlocks()
    {
        int amount = 3 + data.Score / 1000;
        scoreBlockSpawner.SpawnScoreBlocksByKilling(transform.position, amount);
    }

    private void RemoveFromRanking()
    {
        rankingManager?.RemoveEntity(this);
    }
};
