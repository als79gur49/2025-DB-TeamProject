using UnityEngine;

public abstract class EntitySpawner : MonoBehaviour
{
    // SessionManager에서 주입
    [SerializeField] 
    protected DamagePopupManager damagePopupManager;
    [SerializeField] 
    protected KillLogManager killLogManager;
    [SerializeField] 
    protected ScoreBlockSpawner scoreBlockSpawner;

    [Header("스킨 오브젝트")]
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
            Debug.LogError($"{GetType().Name}이 초기화되지 않았습니다!");
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