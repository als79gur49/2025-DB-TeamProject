using UnityEngine;

public abstract class EntitySpawner : MonoBehaviour
{
    // SessionManager���� ����
    [SerializeField] 
    protected DamagePopupManager damagePopupManager;
    [SerializeField] 
    protected KillLogManager killLogManager;
    [SerializeField] 
    protected ScoreBlockSpawner scoreBlockSpawner;

    [Header("��Ų ������Ʈ")]
    [SerializeField] 
    protected Mesh[] skinnedMesh;
    [SerializeField] 
    protected Material[] material;

    protected bool isInitialized = false;

    public virtual void Setup(DamagePopupManager damagePopupManager, KillLogManager killLogManager, ScoreBlockSpawner scoreBlockSpawner)
    {
        this.damagePopupManager = damagePopupManager;
        this.killLogManager = killLogManager;
        this.scoreBlockSpawner = scoreBlockSpawner;
        isInitialized = true;
    }

    protected virtual void ValidateSetup()
    {
        if (!isInitialized)
        {
            Debug.LogError($"{GetType().Name}�� �ʱ�ȭ���� �ʾҽ��ϴ�!");
        }
    }
    protected void ApplyRandomSkin(Entity entity)
    {
        if (skinnedMesh != null && skinnedMesh.Length > 0 &&
            material != null && material.Length > 0)
        {
            entity.SetSkin(skinnedMesh[Random.Range(0, skinnedMesh.Length)],
                          material[Random.Range(0, material.Length)]);
        }
    }
}