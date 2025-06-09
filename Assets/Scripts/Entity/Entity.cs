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

    private DamagePopupManager damagePopupManager;
    private KillLogManager killLogManager;
    private ScoreBlockSpawner scoreBlockSpawner;

    // ���� ���� ���� �� ���� Ŭ����
    private WeaponBase weapon;
    private ProjectileStorage projectileStorage;
    [SerializeField]
    private List<Projectile> storages; // �ش� ������ �ӽ÷� ����ü �־�� �� �����δ� �ܺο��� ���� �� ���� ���ؼ� projectileStorage�� �־��ֱ�

    public WeaponBase Weapon=>weapon;

    // �̸�, ���ݷ�, ���ھ� ��
    public EntityData Data => data;
    public EntityInfo Info => info;

    [SerializeField]
    private Transform damageTextPoint;
    [SerializeField]
    protected Transform firePoint;

    [SerializeField]
    private GameObject levelupPrefab;

    public bool IsDead { get => data.HP <= 0; }

    // �������� ���ݹ��� ���� �̸�, ���� �̸�
    private KeyValuePair<Entity, string> lastDamagedInfo;


    public void Setup(EntityInfo info, EntityData data,
        RankingManager rankingManager, DamagePopupManager damagePopupManager,
        KillLogManager killLogManager, ScoreBlockSpawner scoreBlockSpawner)
    {  
        this.damagePopupManager = damagePopupManager;
        this.killLogManager = killLogManager;
        this.scoreBlockSpawner = scoreBlockSpawner;

        Setup();

        this.info = info;
        this.data = data;

        GetComponent<BoxCollider>().enabled = true;

        onDeath.AddListener(DeathLog); // lastDamagedInfo, KillLogManager 
        onDeath.AddListener(GiveScoreToLastAttacker); // lastDamagedInfo, data
        onDeath.AddListener(SpawnLevelupBlocks); // ������ ����ġ������ ����
    }
    protected virtual void Setup()
    {
        animation = GetComponent<EntityAnimation>();

        weapon = GetComponent<WeaponBase>();
        weapon.Setup(this, firePoint, weapon.Data);

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
        if (lastDamagedInfo.Key != null)
        {
            lastDamagedInfo.Key.AddScore(100);
        }
    }

    public void AddScore(int amount)
    {
        const int levelupAmount = 1000;

        int remainExp = data.Score % levelupAmount;
        int levelupNum = (remainExp + amount) / levelupAmount;
        data.AddScore(amount);

        for (int i = 0; i < levelupNum; ++i)
        {
            //Do Levelup
            if (levelupPrefab != null)
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

    public void ChangeWeapon(WeaponBase weapon)
    {
        this.weapon = weapon;
    }
};
