using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : EntitySpawner
{
    [SerializeField]
    private Enemy[] enemy;
    [SerializeField]
    private Mesh[] enemyMeshes;

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

            string name = "Test_Enemy_" + Random.Range(0, 10000);

            clone.Setup(new EntityInfo(name, "Test_Image"), new EntityData(100, 10, 1), memoryPool, damagePopupManager, killLogManager, scoreBlockSpawner);
            clone.ChangeState(EntityStates.IdleState);
        }
    }
}
