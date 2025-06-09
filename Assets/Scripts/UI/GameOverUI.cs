using System.Collections;
using System.Collections.Generic;
using UnityEditor.Search;
using UnityEngine;
using UnityEngine.UI;

public class GameOverUI : MonoBehaviour
{
    [SerializeField]
    private InfoList infoPrefab;
    [SerializeField]
    private Transform ListParent;

    [SerializeField]
    private InfoList player;

    private int showInfoNum = 10;


    public void ShowGameOver()
    {
        ShowEntityInfo();
        ShowPlayerInfo();
    }

    // ÀüÃ¼ ·©Å· 
    private void ShowEntityInfo()
    {
        List<RankingData> list = RankingRepository.GetTopRankings(showInfoNum);

        foreach(RankingData rankingData in list)
        {
            InfoList clone = Instantiate(infoPrefab);
            clone.transform.SetParent(ListParent, false);

            clone.Setup(rankingData.Rank, rankingData.PlayerName, rankingData.Score);
        }

    }
    
    // ÇÃ·¹ÀÌ¾î ·©Å·
    private void ShowPlayerInfo()
    {
        //RankingData data = RankingRepository.GetTopRankings;
        //
        //player.Setup(data.Rank, data.PlayerName, data.Score);
    }
}
