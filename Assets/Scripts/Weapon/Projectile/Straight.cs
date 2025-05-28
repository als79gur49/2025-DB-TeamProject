using UnityEngine;

public class Straight : Projectile
{
    public override void Setup(GameObject owner, int damage, float speed, int range, float duration = 10)
    {
        base.Setup(owner, damage, speed, range, duration);
    }

    protected override void Move()
    {
        transform.position += transform.forward * (data.speed * Time.deltaTime);
    }
}
