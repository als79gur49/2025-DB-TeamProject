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
    [SerializeField]
    private Transform ListParent;

    [SerializeField]
    private InfoList player;

    private int showInfoNum = 10;

    private RectTransform rect;
    private float duration = 1.5f;

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
        //RankingData data = RankingRepository.GetTopRankings;
        //
        //player.Setup(data.Rank, data.PlayerName, data.Score);
    }
}
