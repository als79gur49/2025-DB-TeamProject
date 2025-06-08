using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;

public class LevelupStorage : MonoBehaviour
{
    private List<ILevelup> levelupable;
    public List<ILevelup> Levelupable => levelupable;

    private SkillIconManager skillIconManager;
    private const int maxSelectedIcons = 3;

    private Entity owner;
    private Transform firePoint;

    public void Setup(SkillIconManager skillIconManager, Entity owner, Transform firePoint)
    {
        this.skillIconManager = skillIconManager;

        this.owner = owner;
        this.firePoint = firePoint;

        levelupable = new List<ILevelup>();
        levelupable = skillIconManager.GetLevelupable();
        foreach(ILevelup obj in  levelupable)
        {
            if (obj is BaseSkill skill)
            {
                skill.Setup(owner);
            }
            else if (obj is WeaponBase weapon)
            {
                weapon.Setup(owner, firePoint);
            }
        }
    }

    public void Levelup()
    {
        if(levelupable == null || levelupable.Count == 0)
        {
            Debug.Log("���� �� ������ ��ų ����");
            return;
        }

        List<ILevelup> randomSkills = new List<ILevelup>(levelupable);
        // �����ϰ� ����
        int count = Mathf.Min(maxSelectedIcons, randomSkills.Count);
        List<ILevelup> selected = randomSkills.GetRange(0, count);

        skillIconManager.ShowSkillIcons(selected);
    }

    public void AddLevelupable(ILevelup item)
    {
        // levelupable.Any(x => x.GetType() == item.GetType())

        // ���� �񱳰� �ƴ� Ŭ���� Ÿ������ ��
        ILevelup sameTypeClassItem = levelupable.FirstOrDefault(x => x.GetType() == item.GetType());
        if(sameTypeClassItem != null)
        {
            levelupable.Remove(sameTypeClassItem);
            item.LevelUp();
            levelupable.Add(item);

        }
        else
        {
            levelupable.Add(item);
        }
    }
}
