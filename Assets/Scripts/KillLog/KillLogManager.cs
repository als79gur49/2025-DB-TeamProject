using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class KillLogManager : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI text;

    public void AddLog(KillLog log)
    {
        text.text = $"{log.Enemy}�� {log.Weapon}�� {log.Self}�� �׿����ϴ�.";
    }
}
