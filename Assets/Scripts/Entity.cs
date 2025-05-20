using UnityEngine;

public abstract class Entity : MonoBehaviour
{
    protected EntityInput input;
    protected EntityBrain brain;
    protected EntityOutput output;

    private EntityInfo info;
    protected EntityData data;

    private RankingManager rankingManager;

    public EntityData Data => data;
    public EntityInfo Info => info;


    public void Setup(RankingManager rankingManager, EntityInfo info, EntityData data)
    {
        this.rankingManager = rankingManager;

        this.info = info;
        this.data = data;

        rankingManager?.AddEntity(this);
    }

    public interface EntityInput
    {
        public void DoInput();
    };

    public interface EntityBrain
    {
        public void DoCalculate();
    };

    public interface EntityOutput
    {
        public void DoOutput();
    };

    private void OnDisable()
    {
        //Enable�� Setup���� ���� �۵��Ͽ� AddEntity��ġ�� Setup���� ����   
        rankingManager?.RemoveEntity(this);
    }
};
