using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System.Data.Common;

public abstract class Projectile : MonoBehaviour, ILevelup
{
    // origin SO
    [SerializeField]
    private SOProjectile origin_data;
    
    // copied SO
    private SOProjectile data;
    public SOProjectile Data => data;

    private float timer;
    private Vector3 prevPosition;
    private float accumulatedDistance;

    private GameObject owner;
    private Entity entity;
    public GameObject Owner => owner;

    private List<ProjectileEffect> effects;
    private ProjectileEffect pEffect;
    // VFX
    public GameObject muzzlePrefab;
    public GameObject hitPrefab;

    private void Awake()
    {
        // 
        Initialize();
        
    }

    // Double check
    public void Initialize()
    {
        data = Instantiate(origin_data);
    }

    public virtual void Setup(GameObject owner, float damage = 1, float speed = 1, float range = 1, float duration = 1, float attackRate = 1, float size = 1)
    {
        this.owner = owner;

        data.damage *= damage;
        data.speed *= speed;
        data.range *= range;
        data.duration *= duration;
        data.attackRate *= attackRate;

        data.size *= size;

        Setup();
    }
    private void Setup()
    {
        timer = 0;
        accumulatedDistance = 0;

        transform.localScale = Vector3.one * data.size;
        if(muzzlePrefab != null )
        {
            muzzlePrefab.transform.localScale = Vector3.one * data.size;
        }
        if(hitPrefab != null )
        {
            hitPrefab.transform.localScale = Vector3.one * data.size;
        }

        effects = new List<ProjectileEffect>();
        pEffect = new PoisonEffect(owner.GetComponent<Entity>(), 10, 0.3f);
        effects.Add(pEffect);
        Debug.Log($"{effects.Count}, {pEffect}");


        entity = owner.GetComponent<Entity>();

        // parent collision disable
        Collider projectileCollider = GetComponent<Collider>();
        Collider[] ownerColliders = owner.GetComponentsInChildren<Collider>();
        foreach (Collider collider in ownerColliders)
        {
            Physics.IgnoreCollision(projectileCollider, collider);
        }
    }

    void Start()
    {
        if (muzzlePrefab != null)
        {
            //muzzlePrefab.transform.localScale = Vector3.one * data.size;
            var muzzleVFX = Instantiate(muzzlePrefab, transform.position, Quaternion.identity);
            muzzleVFX.transform.forward = gameObject.transform.forward;
            var psMuzzle = muzzleVFX.GetComponent<ParticleSystem>();
            if (psMuzzle != null)
            {
                Destroy(muzzleVFX, psMuzzle.main.duration);
            }
            else
            {
                var psChild = muzzleVFX.transform.GetChild(0).GetComponent<ParticleSystem>();
                Destroy(muzzleVFX, psChild.main.duration);
            }
        }
    }

    void Update()
    {
        timer += Time.deltaTime;
        accumulatedDistance += Vector3.Distance(prevPosition, transform.position);

        prevPosition = transform.position;

        if(timer >= data.duration ||
            accumulatedDistance >= data.range)
        {
            Destroy(gameObject);
        }

        if (data.speed != 0)
        {
            Move();
        }
        else
        {
            Debug.Log("No Speed");
        }
    }

    protected abstract void Move();


    void OnCollisionEnter(Collision c)
    {
        data.speed = 0;

        ContactPoint contact = c.contacts[0];
        Quaternion rot = Quaternion.FromToRotation(Vector3.up, contact.normal);
        Vector3 pos = contact.point;

        if (hitPrefab != null)
        {
            //hitPrefab.transform.localScale = Vector3.one * data.size;
            var hitVFX = Instantiate(hitPrefab, pos, rot);
            var psHit = hitVFX.GetComponent<ParticleSystem>();
            if (psHit != null)
            {
                Destroy(hitVFX, psHit.main.duration);
            }
            else
            {
                var psChild = hitVFX.transform.GetChild(0).GetComponent<ParticleSystem>();
                Destroy(hitVFX, psChild.main.duration);
            }
        }

        if (c.gameObject.TryGetComponent<IDamageable>(out IDamageable target))
        {
            // Prev Attack
            foreach (var effect in effects)
            {
                effect.ApplyEffect(target);
            }

            // Main Attack
            Debug.Log($"Main Attack Frame:{Time.frameCount}");
            target.TakeDamage(data.damage, owner.GetComponent<Entity>(), data.name);
            // After Attack
        }
        Destroy(gameObject);
    }

    public void LevelUp()
    {
        throw new System.NotImplementedException();
    }
}
