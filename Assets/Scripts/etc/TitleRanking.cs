using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class TitleRanking : MonoBehaviour
{
    [SerializeField]
    private Transform ListParent;
    [SerializeField]
    private InfoList prefab;

    private int showInfoNum = 10;
    private List<InfoList> rankingLists;

    private void Awake()
    {
        rankingLists = new List<InfoList>();
    }

    public void ShowRankings(int showInfoNum)
    {
        // 완료된 게임들의 모든 엔티티 최고 점수 랭킹
        var allTimeRanking = RankingManager.GetAllTimeRanking(10);

        // 부족한 개수만큼 추가
        int num = Mathf.Min(allTimeRanking.Count, showInfoNum);
        // 부족한 만큼만 생성
        while (rankingLists.Count < num)
        {
            rankingLists.Add(Instantiate(prefab, ListParent));
        }


        Debug.Log($"{num} {allTimeRanking.Count} {rankingLists.Count} {showInfoNum}");
        for(int i = 0; i < num; ++i)
        {
            RankingData rank = allTimeRanking[i];

            rankingLists[i].Setup(rank.Rank, rank.EntityName, rank.Score);
        }
    }
    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.X))
        {
            ShowRankings(14);
        }
    }
}
