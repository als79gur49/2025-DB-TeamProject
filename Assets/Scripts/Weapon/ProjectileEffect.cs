using UnityEngine;

public abstract class ProjectileEffect
{
    protected Entity owner;

    public ProjectileEffect(Entity owner)
    {
        this.owner = owner;
    }
    public abstract void ApplyEffect(IDamageable target);
}
