using UnityEngine;

public class Straight : Projectile
{
    public override void Setup(GameObject owner, int damage = 1, float speed = 1, int range = 1, float duration = 1, float attackRate = 1, float size = 1)
    {
        base.Setup(owner, damage, speed, range, duration, attackRate, size);
    }

    protected override void Move()
    {
        transform.position += transform.forward * (data.speed * Time.deltaTime);
    }
}
