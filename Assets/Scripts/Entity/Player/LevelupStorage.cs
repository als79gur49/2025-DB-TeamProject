using System.Collections.Generic;
using UnityEngine;

public class LevelupStorage : MonoBehaviour
{
    private List<ILevelup> levelupable;
    public List<ILevelup> Levelupable => levelupable;

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
            // �ش� ������ ���� ��
        }
        else
        {
            levelupable.Add(item);
        }
    }
}
