using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

public class EnemySpawner : EntitySpawner
{
    [SerializeField] 
    private Enemy[] enemyPrefabs;
    private MemoryPool<Enemy> memoryPool;

    // �̺�Ʈ
    public UnityEvent<Enemy> onEnemySpawned;
    public UnityEvent<Enemy> onEnemyDeath;

    // ���� ���� ���� �� ����
    private int maxEnemyCount = 10;
    private int currentEnemyCount;
    private float timer;
    [SerializeField]
    private float spawnMultiplier = 1f;
    private void Awake()

    {
        if (enemyPrefabs != null && enemyPrefabs.Length > 0)
        {
            memoryPool = new MemoryPool<Enemy>(enemyPrefabs[0], this.transform, 5);
        }
    }

    public override void Setup(DamagePopupManager damagePopupManager, KillLogManager killLogManager, ScoreBlockSpawner scoreBlockSpawner)
    {
        base.Setup(damagePopupManager, killLogManager, scoreBlockSpawner);

        if (enemyPrefabs != null && enemyPrefabs.Length > 0)
        {
            memoryPool = new MemoryPool<Enemy>(enemyPrefabs[0], this.transform, 5);
        }
    }

    public Enemy SpawnEnemy()
    {
        ValidateSetup();

        if (memoryPool == null)
        {
            Debug.LogError("MemoryPool�� ����.");

            return null;
        }

        Enemy clone = memoryPool.ActivatePoolItem();
        // Enemy MemoryPool�̱⿡, GetInstanceID() ���� �� ����. 
        string name = $"Enemy_{clone.GetInstanceID()}_{Random.Range(0, 100)}";

        clone.Setup(new EntityInfo(name, "Test_Image"), new EntityData(1, 100, 10, 1),
                   memoryPool, damagePopupManager, killLogManager, scoreBlockSpawner);

        clone.AddScore(12345 + Random.Range(0, 23456));
        //FSM �⺻ ���� ����
        clone.ChangeState(EntityStates.IdleState);

        // ��Ų ����
        ApplyRandomSkin(clone);

        // ��� �̺�Ʈ ����
        clone.onDeath.AddListener(() => HandleEnemyDeath(clone));

        onEnemySpawned?.Invoke(clone);

        return clone;
    }

    private void HandleEnemyDeath(Enemy enemy)
    {
        onEnemyDeath?.Invoke(enemy);
    }

    private bool CheckEnemyCount()
    {
        GameStatusInfo gameInfo = EntityGameManager.GetCurrentGameStatus();
        currentEnemyCount = gameInfo.AliveEntities - gameInfo.PlayerEntities;

        return gameInfo.IsActive;
    }
    private void Update()
    {
        if(! CheckEnemyCount())
        {
            return;
        }
        // ���� ���ڰ� �������� ���� ������ �������
        float spawnDelay = currentEnemyCount / spawnMultiplier;

        if (Time.time > timer + spawnDelay)
        {
            SpawnEnemy();

            timer = Time.time;
        }
    }
}

/*
 using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : EntitySpawner
{
    [SerializeField]
    private Enemy[] enemy;

    private MemoryPool<Enemy> memoryPool;

    private void Awake()
    {
        memoryPool = new MemoryPool<Enemy>(enemy[0], this.transform, 5);
    }

    public void Setup(DamagePopupManager damagePopupManager, KillLogManager killLogManager)
    {
        this.damagePopupManager = damagePopupManager;
        this.killLogManager = killLogManager;

        memoryPool = new MemoryPool<Enemy>(enemy[0], this.transform, 5);
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Q))
        {
            //Entity clone = GameObject.Instantiate(enemy[Random.Range(0, enemy.Length)]);
            Enemy clone = memoryPool.ActivatePoolItem();

            string name = "Test_Enemy_01_" + Random.Range(0, 10000);


            clone.Setup(new EntityInfo(name, "Test_Image"), new EntityData(1, 100, 10, 1), memoryPool, damagePopupManager, killLogManager, scoreBlockSpawner);
            clone.AddScore(5678 + Random.Range(0, 6789));
            clone.ChangeState(EntityStates.IdleState);

            if (skinnedMesh != null || material != null)
            {
                clone.SetSkin(skinnedMesh[Random.Range(0, skinnedMesh.Length)],
                                         material[Random.Range(0, material.Length)]);
            }
        }
    }
}

 */ 