using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;

public class RankingManager : MonoBehaviour
{
    private Dictionary<string, int> rankingList; // 사용자 이름과 점수만 따로 저장하는 리스트, Unique보장
    // 정렬 필요하면 
     // rankingList.OrderByDescending(t => t.Value).ToList();

    private RankingSQL sql;

    public Dictionary<string, int> RankingList => rankingList;


    private void Awake()
    {
        sql = GetComponent<RankingSQL>();

        rankingList = new Dictionary<string, int>();
    }

    public void Setup(RankingSQL sql)
    {
        this.sql = sql;
        
    }

    private void Update()
    {
        foreach(var t in rankingList)
        {
            Debug.Log($"이름: {t.Key}, 점수: {t.Value}");
        }
    }

    public void UpdateSQL()
    {
        //sql 업데이트
    }

    // rankingList에서 entity의 점수 변경
    public void UpdateEntity(Entity entity) // Score 변경되는 경우
    {
        string name = entity.Info.EntityName;

        if (rankingList.ContainsKey(name))
        {
            rankingList[name] = entity.Data.Score;
            Debug.Log($"{entity.Info.EntityName}의 Score:{entity.Data.Score}");

            UpdateSQL();
        }
        else
        {

        }
    }

    // rankingList에 entity 추가
    public void AddEntity(Entity entity)
    {
        if(rankingList.TryAdd(entity.Info.EntityName, entity.Data.Score))
        {
            Debug.Log($"{entity.Info?.EntityName} 랭킹에 추가");

            UpdateSQL();
        }
        else
        {

        }
        
    }

    public void RemoveEntity(Entity entity)
    {
        if (rankingList.Remove(entity.Info.EntityName))
        {
            Debug.Log($"{entity.Info?.EntityName} 랭킹에 삭제");

            UpdateSQL();
        }
        else
        {

        }  
    }
};