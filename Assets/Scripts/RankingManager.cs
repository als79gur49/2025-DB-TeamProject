using System.Collections.Generic;
using UnityEngine;

public class RangkingManager : MonoBehaviour
{
    private HashSet<Entity> entities; // ����ũ ����

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
            // �߰� ����
        }
        else
        {
            // �߰� ����
        }
    }

    public void RemoveEntity(Entity entity)
    {
        if (entities.Remove(entity))
        {
            // ���� ����
        }
        else
        {
            // ���� ����
        }
    }
};
