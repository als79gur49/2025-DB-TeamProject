using UnityEngine;

public abstract class Entity : MonoBehaviour
{
    protected EntityInput input;
    protected EntityBrain brain;
    protected EntityOutput output;

    private EntityInfo info;
    protected EntityData data;

    public EntityData Data => data;

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
};
