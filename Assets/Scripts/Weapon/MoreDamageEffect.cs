using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoreDamageEffect : ProjectileEffect
{
    public MoreDamageEffect(Entity owner) : base(owner)
    {

    }

    public override void ApplyEffect(IDamageable target)
    {
        target?.TakeDamage(1, owner, "추가데미지 효과");
    }
}
