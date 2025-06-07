using System.Collections.Generic;
using UnityEngine;

public class LevelupStorage : MonoBehaviour
{
    //��� ������ ���� �� ��ų�� �־�α�. 0 -> 1
    private List<ILevelup> levelupable;
    public List<ILevelup> Levelupable => levelupable;

    private SkillIconManager skillIconManager;
    private const int maxSelectedIcons = 3;
    // UI�ʿ� �ʿ� ������ŭ ������. �ش� ��ũ��Ʈ���� ó���ϱ�.

    public void Setup(SkillIconManager skillIconManager)
    {
        levelupable = new List<ILevelup>();
        this.skillIconManager = skillIconManager;

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
        if(levelupable.Contains(item))
        {
            // �ش� ������ ���� ��
        }
        else
        {
            levelupable.Add(item);
        }
    }
}
