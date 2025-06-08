using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using static UnityEngine.UI.GridLayoutGroup;

public class SkillIconManager : MonoBehaviour
{
    // ILeveup�������̽�, �߻� Ŭ���� ����ȭ�� �� �Ǿ �������� ��ġ��.
    [SerializeField]
    private WeaponBase[] weapons;
    [SerializeField]
    private BaseSkill[] skills;

    [SerializeField]
    private GameObject iconPanel;
    [SerializeField] // �ִ� ũ�� 5
    private IconData[] icons;
    [SerializeField]
    private IconData iconPrefab;

    [SerializeField]
    private Sprite defaultIconSprite;
    private string defaultName = "�������� ����.";
    private string defaultDescription = "���� ���� ����.";

    public List<ILevelup> GetLevelupable()
    {
        // ���� ����
        List<ILevelup> lists = new List<ILevelup>();
        foreach(var weapon in weapons)
        {
            lists.Add(Instantiate(weapon));
        }
        foreach(var skill in skills)
        {
            lists.Add(Instantiate(skill));
        }

        return lists;
    }

    public void ShowSkillIcons(List<ILevelup> list)
    {
        List<IconData> iconLists = new List<IconData>();

        for(int i = 0; i<list.Count; ++i)
        {
            IconData clone = Instantiate(iconPrefab);
            clone.transform.SetParent(iconPanel.transform);
            clone.Setup(defaultIconSprite, defaultName, defaultDescription);

            if (list[i] is BaseSkill skill)
            {
                clone.Setup(skill.Data.sprite, skill.Data.skillName, skill.Data.description);
            }
            else if (list[i] is WeaponBase weapon)
            {
                clone.Setup(weapon.Data.sprite, weapon.Data.weaponName, defaultDescription);
            }


            // i ĸ���ؼ� ���
            int capturedIndex = i;
            clone.SelectButton.onClick.AddListener(() =>
            {
                // ������ ���
                list[capturedIndex].LevelUp();

                // ��� ������ ����
                foreach(var icon in iconLists)
                {
                    Destroy(icon.gameObject);
                }
            });

            iconLists.Add(clone);
        }
    }
}
