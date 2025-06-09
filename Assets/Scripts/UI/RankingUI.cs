using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class RankingUI : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI text;

    public void UpdateRankingUI()
    {
        List<RankingData> rankings = RankingRepository.GetTopRankings(40);

        Debug.Log($"{rankings.Count}ÃÑ °³¼ö | {text.text}");
        text.text = "";
        foreach(RankingData ranking in rankings)
        {
            Debug.Log($"{ranking.Rank}µî | {ranking.PlayerName}: {ranking.Score}\n");
            string rankingText = $"{ranking.Rank}µî | {ranking.PlayerName}: {ranking.Score}\n";
            text.text += rankingText;
        }
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.S))
        {
            UpdateRankingUI();
        }
    }
}
