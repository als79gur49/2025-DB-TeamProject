using System.Collections;
using UnityEngine;
using DG.Tweening;

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
    private float tickDamage = 3;
    private float timer;

    public override void ApplyEffect(IDamageable target)
    {
        Tween poisonTween;
        Debug.Log("Apply Effect");
        poisonTween = DOTween.Sequence()
            .JoinCallback(() => {
                Debug.Log($"Poison Attack Frame:{Time.frameCount}");
                target?.TakeDamage(tickDamage, owner, "Poison");
            })
            .AppendInterval(tick)
            .SetLoops((int)(duration/ tick));

       // float lastTick = -tick; // 0
       // DOVirtual.Float(0f, duration, duration, progress => {
       //     if (progress - lastTick >= tick)
       //     {
       //         lastTick += tick;
       //         target?.TakeDamage(80, owner, "Poison");
       //         Debug.Log($"Poison Attack Frame:{Time.frameCount}");
       //     }
       // });



        //CoroutineSingleton.Instance.RunCoroutine(poisonCoroutine(target));
    }

    public IEnumerator poisonCoroutine(IDamageable target)
    {
        Debug.Log("Apply Effect");
        while (timer < duration)
        {
            if(target.IsDead)
            {
                break;
            }
            Debug.Log($"Poison Attack Frame:{Time.frameCount}");
            target?.TakeDamage(10, owner, "Poison");
            timer += tick;

            yield return new WaitForSeconds(tick);
        }

        yield return null;
    }
}
