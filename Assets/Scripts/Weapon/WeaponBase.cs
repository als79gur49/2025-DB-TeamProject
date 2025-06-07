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
        // List�� ���纻 ���� - ���纻 �������� �ʰ� ���� ���� �� foreach ��ȸ �� �ǵ���� ���� �߻�
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

            // clone�� �纻�̱⿡ ���� projectile �ѱ��
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
