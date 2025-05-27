using System.Collections;
using UnityEngine;

public class PoisonEffect : ProjectileEffect
{
    public PoisonEffect(Entity owner, float duration, float tick) : base(owner)
    {
        this.duration = duration;
        this.tick = tick;

        timer = 0;
    }

    private float duration;
    private float tick;

    private float timer;

    public override void ApplyEffect(IDamageable target)
    {
        CoroutineSingleton.Instance.RunCoroutine(poisonCoroutine(target));
    }

    public IEnumerator poisonCoroutine(IDamageable target)
    {
        while(timer < duration)
        {
            if(target.IsDead)
            {
                break;
            }

            target?.TakeDamage(10, owner, "Poison");
            timer += tick;

            yield return new WaitForSeconds(tick);
        }

        yield return null;
    }
}
