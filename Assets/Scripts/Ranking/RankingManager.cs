using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;

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

    private void Update()
    {
        foreach(var t in rankingList)
        {
            Debug.Log($"�̸�: {t.Key}, ����: {t.Value}");
        }
    }

    public void UpdateSQL()
    {
        //sql ������Ʈ
    }

    // rankingList���� entity�� ���� ����
    public void UpdateEntity(Entity entity) // Score ����Ǵ� ���
    {
        string name = entity.Info.EntityName;

        if (rankingList.ContainsKey(name))
        {
            rankingList[name] = entity.Data.Score;
            Debug.Log($"{entity.Info.EntityName}�� Score:{entity.Data.Score}");

            UpdateSQL();
        }
        else
        {

        }
    }

    // rankingList�� entity �߰�
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