using UnityEngine;
using UnityEngine.Events;

public interface IDamageable
{
    public void TakeDamage(int amount, Entity enemy, string weaponName);
}
