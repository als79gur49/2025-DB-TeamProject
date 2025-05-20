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
        //Enable이 Setup보다 빨리 작동하여 AddEntity위치는 Setup으로 변경   
        rankingManager?.RemoveEntity(this);
    }
};
