using System.Collections.Generic;
using UnityEngine;

public class LiveRankingUI : MonoBehaviour
{
    [SerializeField]
    private InfoList[] entities;
    [SerializeField]
    private InfoList player;

    private int playerId;

    public void Setup(int playerId)
    {
        this.playerId = playerId;
    }

    public void UpdateEntitiesRanking(List<RankingData> data)
    {
        if(entities == null || data == null)
        {
            return;
        }

        int count = Mathf.Min(entities.Length, data.Count);

        for(int i = 0; i < entities.Length; ++i)
        {
            if(i < count)
            {
                entities[i].Setup(data[i].Rank, data[i].EntityName, data[i].Score);
            }
            else
            {
                entities[i].SetupNULL();
            }
        }
    }
    public void UpdatePlayerRanking(RankingData data)
    {
        if(player == null)
        {
            return;
        }

        player.Setup(data.Rank, data.EntityName, data.Score);
    }
    private void Update()
    {
        //RankingManager.GetLiveRanking(10);

        UpdateEntitiesRanking(RankingManager.GetLiveRanking(10));
        if(playerId != 0)
        {
            UpdatePlayerRanking(RankingManager.GetEntityBestRecord(playerId));
        }    
    }
}
