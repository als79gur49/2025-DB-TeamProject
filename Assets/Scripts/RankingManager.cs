using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class RankingManager : MonoBehaviour
{
    //entity �����ϴ� �� �߰�, ���� �ϴ� ��
    //

    private List<Entity> entityList;

    private RankingSQL sql;

    public List<Entity> EntityList => entityList; //��ŷ UI���� �� ����

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
        //sql ������Ʈ
    }

    public void AddEntity(Entity entity)
    {
        if( !entityList.Contains(entity)) // �ߺ� ����
        {
            // �߰� ����
            entityList.Add(entity);

            //�������� ����
            entityList = entityList
                .OrderByDescending(t => t.Data.Score)
                .ToList();

            UpdateRanking();
            Debug.Log($"{entity.Info?.EntityName} ��ŷ�� �߰�");
        }
        else
        {

        }
    }

    public void RemoveEntity(Entity entity)
    {
        if (entityList.Contains(entity))
        {
            // ���� ����
            entityList.Remove(entity);

            entityList = entityList
                .OrderByDescending(t => t.Data.Score)
                .ToList();

            UpdateRanking();
            Debug.Log($"{entity.Info?.EntityName} ��ŷ�� ����");
        }
        else
        {

        }
    }
};
