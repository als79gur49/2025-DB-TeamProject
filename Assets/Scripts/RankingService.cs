using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class RankingService : MonoBehaviour
{
    List<Entity> prevRanking;
    List<Entity> currentRanking;

    public void Setup()
    {
        prevRanking = new List<Entity>();
        currentRanking = new List<Entity>();
    }

    public void CalculateRanking(HashSet<Entity> entities)
    {
        prevRanking = currentRanking;

        currentRanking = entities
            .OrderByDescending(p => p.Data.Score)
            .ToList();
    }

    public void UpdateRankingBoard()
    {

    }
};
