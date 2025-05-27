using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class RankingManager : MonoBehaviour
{
    private Dictionary<string, int> rankingList; // ����� �̸��� ������ ���� �����ϴ� ����Ʈ, Unique����
    // ���� �ʿ��ϸ� 
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

    public void UpdateSQL()
    {
        //sql ������Ʈ
    }
    public void UpdateEntity(Entity entity) // Score ����Ǵ� ���
    {
        if(rankingList.TryGetValue(entity.Info.EntityName, out int score))
        {
            score = entity.Data.Score;
            Debug.Log($"{entity.Info.EntityName}�� Score:{entity.Data.Score}");

            UpdateSQL();
        }
        else
        {

        }
    }

    public void AddEntity(Entity entity)
    {
        if(rankingList.TryAdd(entity.Info.EntityName, entity.Data.Score))
        {
            Debug.Log($"{entity.Info?.EntityName} ��ŷ�� �߰�");

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
            Debug.Log($"{entity.Info?.EntityName} ��ŷ�� ����");

            UpdateSQL();
        }
        else
        {

        }  
    }
};