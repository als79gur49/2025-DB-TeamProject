using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField]
    private Entity[] enemy;

    [SerializeField]
    public RankingManager rankingManager;


    public void Setup(RankingManager rankingManager)
    {
        this.rankingManager = rankingManager;
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Q))
        {
            Entity clone = GameObject.Instantiate(enemy[Random.Range(0, enemy.Length)]);

            string name = "Test_Enemy_" + Random.Range(0, 10000);

            clone.Setup(rankingManager, new EntityInfo(name, "Test_Image"), new EntityData(100, 10, 1));

            TestSql.Init();

            TestSql.GetScore("test00");

            clone.ChangeState(EntityStates.IdleState);
        }
    }
}
