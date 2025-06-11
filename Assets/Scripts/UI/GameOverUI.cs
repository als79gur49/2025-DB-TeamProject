using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Search;
using UnityEngine;
using UnityEngine.UI;

public class GameOverUI : MonoBehaviour
{
    [SerializeField]
    private InfoList infoPrefab;
    // 랭킹 정보들의 부모Transform
    [SerializeField]
    private Transform ListParent;

    // 플레이어 랭킹 정보 UI(개인)
    [SerializeField]
    private InfoList player;

    private int showInfoNum = 10;

    private RectTransform rect;
    private float duration = 1.5f;

    private int playerId;
    private int sessionId;
    public void Setup(int playerId, int sessionId)
    {
        this.playerId = playerId;
        this.sessionId = sessionId;
    }

    public void ShowGameOver()
    {
        MoveAnimation();
        ShowEntityInfo();
        ShowPlayerInfo();
    }

    // 아래에서 위로 올라오는 애니메이션
    private void MoveAnimation()
    {
        rect = GetComponent<RectTransform>();
        rect.DOAnchorPosY(1000, duration).From().SetEase(Ease.OutBack);
    }

    // 전체 랭킹 
    private void ShowEntityInfo()
    {
        //List<RankingData> list = RankingRepository.GetTopRankings(showInfoNum);
        //List<RankingData> list = RankingManager.GetLiveRanking(showInfoNum);
        List<RankingData> list = RankingManager.GetSessionEndRanking(sessionId, showInfoNum);

        foreach (RankingData rankingData in list)
        {
            InfoList clone = Instantiate(infoPrefab);
            clone.transform.SetParent(ListParent, false);

            clone.Setup(rankingData.Rank, rankingData.EntityName, rankingData.Score);
        }

    }
    
    // 플레이어 랭킹
    private void ShowPlayerInfo()
    {
       // RankingData playerData = RankingManager.GetEntityBestRecord(playerId);
       //
       // player.Setup(playerData.Rank, playerData.EntityName, playerData.Score);
    }
}
