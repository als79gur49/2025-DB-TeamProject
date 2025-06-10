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

    private int playerId;
    public void Setup(int playerId)
    {
        this.playerId = playerId;
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
        List<RankingData> list = RankingRepository.GetTopRankings(showInfoNum);

        foreach(RankingData rankingData in list)
        {
            InfoList clone = Instantiate(infoPrefab);
            clone.transform.SetParent(ListParent, false);

            clone.Setup(rankingData.Rank, rankingData.PlayerName, rankingData.Score);
        }

    }
    
    // �÷��̾� ��ŷ
    private void ShowPlayerInfo()
    {
        PlayerModel p = PlayerRepository.GetPlayerById(playerId);
        player.Setup(111, p.PlayerName, p.HighestScore);
        //RankingData data = RankingRepository.GetTopRankings;
        //
        //player.Setup(data.Rank, data.PlayerName, data.Score);
    }
}
