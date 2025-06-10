using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class Pair
{
    public string name;
    public Sprite sprite;
}

public class KillLogManager : MonoBehaviour
{
    [SerializeField]
    private KillLogPanel killLogPrefab;

    [SerializeField]
    private Transform killLogParent;

    // ���� �̸�, ��������Ʈ ����
    [SerializeField]
    private List<Pair> weaponSprites;

    private float removeLogTime = 5f;

    public void AddLog(KillLog log)
    {
        Pair resultPair = weaponSprites.FirstOrDefault();
     
        //log.Weapon���� ���� �̸��� �����ϰ� �ش� ��������Ʈ ��ȯ
        foreach (Pair pair in weaponSprites)
        {
            if (log.Weapon.Contains(pair.name))
            {
                resultPair = pair;
                break;
            }
        }

        KillLogPanel clone = Instantiate(killLogPrefab);
        clone.Setup(log.Enemy, resultPair.sprite, log.Self);
        clone.transform.SetParent(killLogParent);
        Destroy(clone.gameObject, removeLogTime);
    }
}
