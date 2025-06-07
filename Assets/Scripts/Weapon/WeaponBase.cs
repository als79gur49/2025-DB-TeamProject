using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public abstract class WeaponBase : MonoBehaviour, ILevelup
{
    protected Entity owner;
    protected Transform firePoint;

    protected SOWeapon data;

    public virtual void Shot(ProjectileStorage storage)
    {
        // List로 복사본 생성 - 복사본 생성하지 않고 원본 수정 시 foreach 순회 중 건드려서 에러 발생
        foreach(var projectile in storage.Projectiles.ToList())
        {
            Debug.Log(projectile.Key.Data);
            Debug.Log(projectile.Key.Data.attackRate);
            if((Time.time - projectile.Value) * 1000 < projectile.Key.Data.attackRate)
            {
                continue;
            }

            Projectile clone = Instantiate(projectile.Key, firePoint.position, firePoint.rotation);
            clone.Setup(owner.gameObject, data.damageMultiplier, data.speedMultiplier, data.rangeMultiplier, data.durationMultiplier,data.attackRateMultiplier, data.sizeMultiplier);

            // clone은 사본이기에 원본 projectile 넘기기
            storage.SetTimeCurrent(projectile.Key);
        }
    }

    public virtual void Setup(SOWeapon data, Entity owner, Transform firePoint)
    {
        this.data = Instantiate(data);

        this.owner = owner;
        this.firePoint = firePoint;
    }

    public bool LevelUp()
    {
        Debug.Log($"{data.level} Prev WeaponLevel");
        data.level += 1;
        Debug.Log($"{data.level} After WeaponLevel");
        return true;
    }
}
