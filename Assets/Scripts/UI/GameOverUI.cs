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
    // ��ŷ �������� �θ�Transform
    [SerializeField]
    private Transform ListParent;

    // �÷��̾� ��ŷ ���� UI(����)
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

    // �Ʒ����� ���� �ö���� �ִϸ��̼�
    private void MoveAnimation()
    {
        rect = GetComponent<RectTransform>();
        rect.DOAnchorPosY(1000, duration).From().SetEase(Ease.OutBack);
    }

    // ��ü ��ŷ 
    private void ShowEntityInfo()
    {
        //List<RankingData> list = RankingRepository.GetTopRankings(showInfoNum);
        //List<RankingData> list = RankingManager.GetLiveRanking(showInfoNum);
        
        //List<RankingData> list = RankingManager.GetSessionEndRanking(sessionId, showInfoNum);
        List<RankingData> list = RankingManager.GetSessionEndRanking(sessionId, showInfoNum);

        foreach (RankingData rankingData in list)
        {
            InfoList clone = Instantiate(infoPrefab);
            clone.transform.SetParent(ListParent, false);

            clone.Setup(rankingData.Rank, rankingData.EntityName, rankingData.Score);
        }

    }
    
    // �÷��̾� ��ŷ
    private void ShowPlayerInfo()
    {
        List<RankingData> list = RankingManager.GetSessionEndRanking(sessionId, GameSessionManager.GetSession(sessionId2).TotalEntities);
        foreach (RankingData rankingData in list)
        {
            // ��ġ ����, ���� �÷��̾� ��ŷ�� ã������ ���� DB���� �����ͼ� ����غ�����, ��ŷ�� ����� ǥ��X
            if (rankingData.IsPlayer)
            {
                player.Setup(rankingData.Rank, rankingData.EntityName, rankingData.Score);

                break;
            }
        }

        return;
    }
}
