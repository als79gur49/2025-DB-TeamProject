using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileMove : MonoBehaviour
{
    public float speed;
    public float fireRate;
    public GameObject muzzlePrefab;
    public GameObject hitPrefab;

    private GameObject owner;
    private Entity entity;
    public GameObject Owner => owner;

    private List<ProjectileEffect> effects;

    public void Setup(GameObject owner)
    {
        this.owner = owner;
        entity = owner.GetComponent<Entity>();

        Collider projectileCollider = GetComponent<Collider>();
        Collider[] ownerColliders = owner.GetComponentsInChildren<Collider>();
        
        foreach (Collider collider in ownerColliders)
        {
            Physics.IgnoreCollision(projectileCollider, collider);
        }

        effects = new List<ProjectileEffect>();
        effects.Add(new MoreDamageEffect(entity));
        effects.Add(new PoisonEffect(entity, 5, 0.1f));
    }

    void Start()
    {
        if (muzzlePrefab != null)
        {
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
        if (speed != 0)
        {
            transform.position += transform.forward * (speed * Time.deltaTime);
        }
        else
        {
            Debug.Log("No Speed");
        }
    }

    void OnCollisionEnter (Collision co)
    {
        speed = 0;

        ContactPoint contact = co.contacts[0];
        Quaternion rot = Quaternion.FromToRotation(Vector3.up, contact.normal);
        Vector3 pos = contact.point;

        if(hitPrefab != null)
        {
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

        if(co.gameObject.TryGetComponent<IDamageable>(out IDamageable target))
        {
            foreach(var effect in effects)
            {
                effect.ApplyEffect(target);
            }

            target.TakeDamage(50, owner.GetComponent<Entity>(), "TmpBow");
        }
        Destroy(gameObject);
    }
}