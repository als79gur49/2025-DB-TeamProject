using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class LevelupStorage : MonoBehaviour
{
    // ���� �� Ŭ���� ��ġ��. �������̽� or �߻�Ŭ������ ����ȭ �Ұ�
    private List<ILevelup> levelupable;
    public List<ILevelup> Levelupable => levelupable;

    private SkillIconManager skillIconManager;
    private const int maxSelectedIcons = 3;
    // UI�ʿ� �ʿ� ������ŭ ������. �ش� ��ũ��Ʈ���� ó���ϱ�.

    public void Setup(SkillIconManager skillIconManager)
    {
        levelupable = new List<ILevelup>();
        this.skillIconManager = skillIconManager;

        levelupable = skillIconManager.GetLevelupable();
    }

    public void Levelup()
    {
        if(levelupable != null || levelupable.Count == 0)
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
        Debug.Log("Test");
        // ������ �ƴ� ���� Ŭ������ ��
        if(levelupable.Any(x => x.GetType() == item.GetType()))
        {
            Debug.Log("Test11");
            // �ش� ������ ���� ��
            levelupable.Remove(item);

            item.LevelUp();
            levelupable.Add(item);
        }
        else
        {
            Debug.Log($"{levelupable[0]} And {item}");
            levelupable.Add(item);
        }

        //if(levelupable.Contains(item))
        //{
        //    Debug.Log("Test11");
        //    // �ش� ������ ���� ��
        //    levelupable.Remove(item);
        //
        //    item.LevelUp();
        //    levelupable.Add(item);
        //}
        //else
        //{
        //    Debug.Log($"{levelupable[0]} And {item}");
        //    levelupable.Add(item);
        //}
    }
}
