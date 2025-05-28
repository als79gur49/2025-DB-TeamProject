using TMPro.EditorUtilities;
using UnityEngine;

public abstract class WeaponBase : MonoBehaviour
{
    protected Entity owner;
    protected Transform firePoint;

    protected int level;
    protected float cooldown;

    protected Projectile projectile; 

}
