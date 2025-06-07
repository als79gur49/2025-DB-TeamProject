using System.Collections.Generic;
using UnityEngine;

public class LevelupStorage : MonoBehaviour
{
    private List<ILevelup> levelupable;
    public List<ILevelup> Levelupable => levelupable;
    // UI쪽에 필요 개수만큼 보내서. 해당 스크립트에서 처리하기.
    private void Awake()
    {
        levelupable = new List<ILevelup>();
    }

    public void Setup()
    {

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
