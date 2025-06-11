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

    public UnityEvent<int> onLevelup;
    public UnityEvent<int, int> onAddExperience;
    // Animation ����

    protected EntityAnimation animation;

    // data 관련
    private EntityInfo info;
    protected EntityData data;

    private DamagePopupManager damagePopupManager;
    private KillLogManager killLogManager;
    private ScoreBlockSpawner scoreBlockSpawner;

    // 소유 중인 무기 및 공격 클래스
    private WeaponBase weapon;
    private ProjectileStorage projectileStorage;
    [SerializeField]
    private List<Projectile> storages; // 해당 내용은 임시로 투사체 넣어둔 곳 실제로는 외부에서 레벨 업 등을 통해서 projectileStorage에 넣어주기

    public WeaponBase Weapon=>weapon;

    // 이름, 공격력, 스코어 등
    public EntityData Data => data;
    public EntityInfo Info => info;

    [SerializeField]
    private Transform damageTextPoint;
    [SerializeField]
    protected Transform firePoint;

    [SerializeField]
    private GameObject levelupPrefab;

    public bool IsDead { get => data.HP <= 0; }

    // 마지막을 공격받은 적의 이름, 무기 이름
    private KeyValuePair<Entity, string> lastDamagedInfo;

    const int levelupAmount = 1000;
    public int LevelupAmount => levelupAmount;
    public void Setup(EntityInfo info, EntityData data,
        DamagePopupManager damagePopupManager,
        KillLogManager killLogManager, ScoreBlockSpawner scoreBlockSpawner)
    {  
        this.damagePopupManager = damagePopupManager;
        this.killLogManager = killLogManager;
        this.scoreBlockSpawner = scoreBlockSpawner;

        this.info = info;
        this.data = data;

        Setup();

        GetComponent<BoxCollider>().enabled = true;

        onDeath.AddListener(DeathLog); // lastDamagedInfo, KillLogManager 
        onDeath.AddListener(GiveScoreToLastAttacker); // lastDamagedInfo, data
        onDeath.AddListener(SpawnLevelupBlocks); // 죽으면 경험치블럭들 생성
    }

    protected virtual void Setup()
    {
        animation = GetComponent<EntityAnimation>();

        weapon = GetComponent<WeaponBase>();
        weapon.Setup(this, firePoint, weapon.Data);

        projectileStorage = GetComponent<ProjectileStorage>();
        projectileStorage.Setup(storages);
    }
    protected virtual void Start()
    {
        // AI 엔티티인 경우 DB에 자동 등록
        if (this is not Player)
        {
            EntityGameManager.AddAIEntity(GetInstanceID(), info.EntityName);
        }
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
    public void RecoverHP(float amount)
    {
        data.AddHp(amount);

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

            // DB에 점수 추가
            EntityGameManager.OnEntityScoreAddbyName(lastDamagedInfo.Key.info.EntityName, 100);
        }
    }
    public void AddScore(int amount)
    {
        int remainExp = data.Score % levelupAmount;
        int levelupNum = (remainExp + amount) / levelupAmount;
        data.AddScore(amount);

        remainExp = (remainExp + amount) % levelupAmount;
        onAddExperience?.Invoke(remainExp, levelupAmount);

        for (int i = 0; i < levelupNum; ++i)
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
            data.Levelup();
            onLevelup?.Invoke(data.Level);
        }

    }
    protected abstract void levelup();

    private void SpawnLevelupBlocks()
    {
        int amount = 3 + data.Score / 1000;
        scoreBlockSpawner.SpawnScoreBlocksByKilling(transform.position, amount);
    }

    public void ChangeWeapon(WeaponBase weapon)
    {
        this.weapon = weapon;
    }

};
