using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class RankingManager : MonoBehaviour
{
    //entity 저장하는 및 추가, 삭제 하는 곳
    //

    private List<Entity> entityList;

    private RankingSQL sql;

    public List<Entity> EntityList => entityList; //랭킹 UI만들 때 참조

    private void Awake()
    {
        sql = GetComponent<RankingSQL>();
        entityList = new List<Entity>();
    }

    public void Setup(RankingSQL sql)
    {
        this.sql = sql;
    }

    public void UpdateRanking()
    {
        //sql 업데이트
    }

    public void AddEntity(Entity entity)
    {
        if( !entityList.Contains(entity)) // 중복 제거
        {
            // 추가 성공
            entityList.Add(entity);

            //내림차순 정렬
            entityList = entityList
                .OrderByDescending(t => t.Data.Score)
                .ToList();

            UpdateRanking();
            Debug.Log($"{entity.Info?.EntityName} 랭킹에 추가");
        }
        else
        {

        }
    }

    public void RemoveEntity(Entity entity)
    {
        if (entityList.Contains(entity))
        {
            // 삭제 성공
            entityList.Remove(entity);

            entityList = entityList
                .OrderByDescending(t => t.Data.Score)
                .ToList();

            UpdateRanking();
            Debug.Log($"{entity.Info?.EntityName} 랭킹에 삭제");
        }
        else
        {

        }
    }
};
