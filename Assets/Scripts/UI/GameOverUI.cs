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

    private string playerName;
    private int sessionId;

    private int playerId2;
    private int sessionId2;
    public void Setup(string playerName, int sessionId)
    {
        this.playerName = playerName;
        this.sessionId = sessionId;

        playerId2 = GameSessionManager.GetCurrentPlayerEntityId();
        sessionId2 = GameSessionManager.GetCurrentSessionId();
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
     //   // 가장 최근 플레이어 정보 조회
     //   PlayerModel playerData = PlayerRepository.GetPlayerByName(playerName);
     //
     //   // 가장 최근 세션과 가장 최근 플레이어ID로 해당 세션에서의 엔티티 정보 조회
     //   //var sessionEntity = SessionEntityRepository.GetSessionEntity(sessionId, playerData.PlayerID);
     //   var sessionEntity = SessionEntityRepository.GetSessionEntity(sessionId, playerData.PlayerName);
     //   if(sessionEntity != null)
     //   {
     //       player.Setup(sessionEntity.FinalRank ?? -1, sessionEntity.EntityName, sessionEntity.Score);
     //   }

        SessionEntityModel sessionEntity2 = GameSessionManager.GetCurrentSessionEntities(sessionId2, playerId2);
        if(sessionEntity2 != null)
        {
            Debug.Log($"{sessionEntity2.FinalRank}");
            player.Setup(sessionEntity2.FinalRank ?? -1, sessionEntity2.EntityName, sessionEntity2.Score);
        }

        List<RankingData> list = RankingManager.GetSessionEndRanking(sessionId, showInfoNum);
        foreach(RankingData rankingData in list)
        {
            if(rankingData.EntityID == playerId2)
            {
                player.Setup(rankingData.Rank, rankingData.EntityName, rankingData.Score);
            }
            break;
        }
    }
}
