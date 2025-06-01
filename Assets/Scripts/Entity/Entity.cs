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

    public bool IsDead { get => data.HP <= 0; }

    // 마지막을 공격받은 적의 이름, 무기 이름
    private KeyValuePair<Entity, string> lastDamagedInfo;


    public void Setup(EntityInfo info, EntityData data,
        RankingManager rankingManager = null, DamagePopupManager damagePopupManager = null, KillLogManager killLogManager = null)
    {  
        this.damagePopupManager = damagePopupManager;
        this.killLogManager = killLogManager;
        this.rankingManager = rankingManager;

        Setup();

        this.info = info;
        this.data = data;
        rankingManager?.AddEntity(this);

        GetComponent<BoxCollider>().enabled = true;

        onDeath.AddListener(DeathLog); // lastDamagedInfo, KillLogManager 
        onDeath.AddListener(GiveScoreToLastAttacker); // lastDamagedInfo, data
        onDeath.AddListener(RemoveFromRanking); // rankingManager에서 제거
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
        onTakeDamage?.Invoke(prevHp, data.HP);
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
        GiveScoreToEnemy(lastDamagedInfo.Key);

        rankingManager.UpdateEntity(lastDamagedInfo.Key);
    }
    private void GiveScoreToEnemy(Entity enemy)
    {
        enemy?.data.AddScore(100);
    }

    private void RemoveFromRanking()
    {
        rankingManager?.RemoveEntity(this);
    }
};
