using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public abstract class WeaponBase : MonoBehaviour, ILevelup
{
    protected Entity owner;
    protected Transform firePoint;

    protected SOWeapon data;
    public SOWeapon Data => data;
    [SerializeField]
    private SOWeapon origin_data;

    public virtual void Shot(ProjectileStorage storage)
    {
        
        foreach(var projectile in storage.Projectiles.ToList())
        {
            if((Time.time - projectile.Value) * 1000 < projectile.Key.Data.attackRate)
            {
                continue;
            }

            Projectile clone = Instantiate(projectile.Key, firePoint.position, firePoint.rotation);

            float levelupAmount = Mathf.Log(data.level + 1); // log2 =1,
            clone.Setup(owner.gameObject, 
                Mathf.Round(data.damageMultiplier * levelupAmount), 
                data.speedMultiplier * levelupAmount, 
                data.rangeMultiplier * levelupAmount,
                data.durationMultiplier * levelupAmount,
                data.attackRateMultiplier * levelupAmount,   
                data.sizeMultiplier * levelupAmount);

            
            storage.SetTimeCurrent(projectile.Key);
        }
    }

    public virtual void Setup(Entity owner, Transform firePoint, SOWeapon data = null)
    {
        this.owner = owner;
        this.firePoint = firePoint;

        this.data = (data == null) ? Instantiate(origin_data) : Instantiate(data); 
    }

    public void LevelUp()
    {
        Debug.Log($"{data.level} Prev WeaponLevel");
        data.level += 1;
        Debug.Log($"{data.level} After WeaponLevel");
    }
}
