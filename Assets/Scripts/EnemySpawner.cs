using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField]
    private Entity[] enemy;
    [SerializeField]
    private Mesh[] enemyMeshes;

    [SerializeField]
    public RankingManager rankingManager;
    [SerializeField]
    public DamagePopupManager damagePopupManager;
    [SerializeField]
    public KillLogManager killLogManager;

    private MemoryPool<Entity> memoryPool;

    private void Awake()
    {
        memoryPool = new MemoryPool<Entity>(enemy[0], this.transform, 5);
    }
    public void Setup(RankingManager rankingManager, DamagePopupManager damagePopupManager, KillLogManager killLogManager)
    {
        this.rankingManager = rankingManager;
        this.damagePopupManager = damagePopupManager;
        this.killLogManager = killLogManager;   
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Q))
        {
            //Entity clone = GameObject.Instantiate(enemy[Random.Range(0, enemy.Length)]);
            Entity clone = memoryPool.ActivatePoolItem();

            string name = "Test_Enemy_" + Random.Range(0, 10000);

            clone.Setup(memoryPool, rankingManager, damagePopupManager, killLogManager, new EntityInfo(name, "Test_Image"), new EntityData(100, 10, 1));

            TestSql.Init();

            TestSql.GetScore("test00");

            clone.ChangeState(EntityStates.IdleState);
        }
    }
}
