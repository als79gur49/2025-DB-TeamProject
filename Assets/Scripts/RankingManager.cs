using System.Collections.Generic;
using UnityEngine;

public class RangkingManager : MonoBehaviour
{
    private HashSet<Entity> entities; // 유니크 보장

    private RankingService service;

    public void Setup(RankingService service)
    {
        this.service = service;
    }

    public void UpdateRanking()
    {

    }

    public void AddEntity(Entity entity)
    {
        if (entities.Add(entity))
        {
            // 추가 성공
        }
        else
        {
            // 추가 실패
        }
    }

    public void RemoveEntity(Entity entity)
    {
        if (entities.Remove(entity))
        {
            // 삭제 성공
        }
        else
        {
            // 삭제 실패
        }
    }
};
