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

    // Animation ����
    protected EntityAnimation animation;

    // data ����
    private EntityInfo info;
    protected EntityData data;

    private RankingManager rankingManager;
    private DamagePopupManager damagePopupManager;
    private KillLogManager killLogManager;

    // ���� ���� ���� �� ���� Ŭ����
    private WeaponBase weapon;
    private ProjectileStorage projectileStorage;
    [SerializeField]
    private List<Projectile> storages; // �ش� ������ �ӽ÷� ����ü �־�� �� �����δ� �ܺο��� ���� �� ���� ���ؼ� projectileStorage�� �־��ֱ�

    // �̸�, ���ݷ�, ���ھ� ��
    public EntityData Data => data;
    public EntityInfo Info => info;

    [SerializeField]
    private Transform damageTextPoint;
    [SerializeField]
    private Transform firePoint;
    [SerializeField]
    private SOWeapon startWeapon;

    public bool IsDead { get => data.HP <= 0; }

    // �������� ���ݹ��� ���� �̸�, ���� �̸�
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
        onDeath.AddListener(RemoveFromRanking); // rankingManager���� ����
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

        // defense �߰��� ���� ���� ����
        data.TakeDamage(amount);
        Debug.Log($"{enemy.Info.EntityName}�� {info.EntityName}���� {weaponName}���� {amount}��ŭ�� ���� ����");

        // ������ ������ ����� ����
        lastDamagedInfo = new KeyValuePair<Entity, string>(enemy, weaponName);

        damagePopupManager.PrintDamage(Color.black, amount, damageTextPoint.position, 3);

        // hpUI ����
        onTakeDamage?.Invoke(prevHp, data.HP);
    }

    private void OnDisable()
    {
        onDeath.RemoveAllListeners();
    }

    // ų�α� ��� ex) x�� y�� z�� óġ
    private void DeathLog()
    {
        if (lastDamagedInfo.Key == null || lastDamagedInfo.Value == null)
        {
            return;
        }

        KillLog log = new KillLog(info.EntityName, lastDamagedInfo.Value, lastDamagedInfo.Key.Info.EntityName);
        killLogManager.AddLog(log);
    }

    // óġ�� ������ ���� �ο�
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
