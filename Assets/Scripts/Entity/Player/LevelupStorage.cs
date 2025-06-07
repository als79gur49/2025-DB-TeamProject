using System.Collections.Generic;
using UnityEngine;

public class LevelupStorage : MonoBehaviour
{
    //모든 가능한 무기 및 스킬들 넣어두기. 0 -> 1
    private List<ILevelup> levelupable;
    public List<ILevelup> Levelupable => levelupable;

    private SkillIconManager skillIconManager;
    private const int maxSelectedIcons = 3;
    // UI쪽에 필요 개수만큼 보내서. 해당 스크립트에서 처리하기.

    public void Setup(SkillIconManager skillIconManager)
    {
        levelupable = new List<ILevelup>();
        this.skillIconManager = skillIconManager;

    }

    public void Levelup()
    {
        if(levelupable != null || levelupable.Count == 0)
        {
            Debug.Log("레벨 업 가능한 스킬 없음");
            return;
        }

        List<ILevelup> randomSkills = new List<ILevelup>(levelupable);
        // 랜덤하게 섞기
        int count = Mathf.Min(maxSelectedIcons, randomSkills.Count);
        List<ILevelup> selected = randomSkills.GetRange(0, count);

        skillIconManager.ShowSkillIcons(selected);
    }

    public void AddLevelupable(ILevelup item)
    {
        if(levelupable.Contains(item))
        {
            // 해당 아이템 레벨 업
        }
        else
        {
            levelupable.Add(item);
        }
    }
}
