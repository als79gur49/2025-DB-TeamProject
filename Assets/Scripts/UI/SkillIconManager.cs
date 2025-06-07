using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillIconManager : MonoBehaviour
{
    [SerializeField]
    private GameObject iconPanel;
    [SerializeField] // 최대 크기 5
    private IconData[] icons;

    [SerializeField]
    private Sprite defaultIconSprite;
    private string defaultName = "존재하지 않음.";
    private string defaultDescription = "세부 내용 없음.";

    public void ShowSkillIcons(List<ILevelup> list)
    {
        for(int i = 0; i<list.Count; ++i) 
        {
            icons[i].enabled = true;

            icons[i].Setup(defaultIconSprite, defaultName, defaultDescription);
            icons[i].SelectButton.onClick.AddListener(()=> list[i].LevelUp());
        }
        for (int i = list.Count; i < 5; ++i)
        {
            icons[i].enabled = false;
        }
    }
}
