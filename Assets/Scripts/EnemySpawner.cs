using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField]
    private Entity enemy;

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
            Entity clone = GameObject.Instantiate(enemy);

            clone.Setup(rankingManager);
        }
    }
}
