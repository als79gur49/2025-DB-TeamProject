using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class LevelupStorage : MonoBehaviour
{
    // 위의 두 클래스 합치기. 인터페이스 or 추상클래스는 직렬화 불가
    private List<ILevelup> levelupable;
    public List<ILevelup> Levelupable => levelupable;

    private SkillIconManager skillIconManager;
    private const int maxSelectedIcons = 3;
    // UI쪽에 필요 개수만큼 보내서. 해당 스크립트에서 처리하기.

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
        Debug.Log("Test");
        // 참조가 아닌 같은 클래스로 비교
        if(levelupable.Any(x => x.GetType() == item.GetType()))
        {
            Debug.Log("Test11");
            // 해당 아이템 레벨 업
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
        //    // 해당 아이템 레벨 업
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
