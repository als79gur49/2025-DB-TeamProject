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
            if((Time.time - projectile.Value) * 1000 < projectile.Key.Data.attackRate)
            {
                continue;
            }

            Projectile clone = Instantiate(projectile.Key, firePoint.position, firePoint.rotation);

            float levelupAmount = Mathf.Log(data.level + 1); // log2 =1,
            clone.Setup(owner.gameObject, 
                data.damageMultiplier * levelupAmount, 
                data.speedMultiplier * levelupAmount, 
                data.rangeMultiplier * levelupAmount,
                data.durationMultiplier * levelupAmount,
                data.attackRateMultiplier * levelupAmount,   
                data.sizeMultiplier * levelupAmount);

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

    public void LevelUp()
    {
        Debug.Log($"{data.level} Prev WeaponLevel");
        data.level += 1;
        Debug.Log($"{data.level} After WeaponLevel");
    }
}
