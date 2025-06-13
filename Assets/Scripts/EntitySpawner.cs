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
        [Header("���� ��ġ ����")]
        [SerializeField]
        protected bool useRandomSpawnPoint = false;
        private Vector3 standardPosition = new Vector3(-40, 0.5f, -40);

        private float xSize = 20 * 5;
        private float ySize = 20 * 5;

        protected EntityBasicData entityBasicData;

        public virtual void Setup(DamagePopupManager damagePopupManager, KillLogManager killLogManager, ScoreBlockSpawner scoreBlockSpawner,
            EntityBasicData basicData)
        {
            this.damagePopupManager = damagePopupManager;
            this.killLogManager = killLogManager;
            this.scoreBlockSpawner = scoreBlockSpawner;
            isInitialized = true;
            entityBasicData = basicData;
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

        protected Vector3 GetRandomSpawnPoint()
        {   
            if(! useRandomSpawnPoint)
            {
                return Vector3.zero;
            }

            int maxNum = 10;
            for(int i = 0; i < maxNum; ++i)
            {
                Vector3 spawnPoint = standardPosition +
                                     Vector3.right * Random.Range(0, (int)xSize) +
                                     Vector3.forward * Random.Range(0, (int)ySize);

                Debug.DrawLine(spawnPoint, spawnPoint + Vector3.up * 10, Color.red, 5f);
                if (Physics.CheckBox(spawnPoint, new Vector3(0.5f, 0.3f, 0.5f), Quaternion.identity))
                {
                    continue;
                }
                return spawnPoint;
            }

            return Vector3.zero;
        }
    }