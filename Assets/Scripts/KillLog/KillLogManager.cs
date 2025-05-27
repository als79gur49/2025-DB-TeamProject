using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class KillLogManager : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI text;

    public void AddLog(KillLog log)
    {
        text.text = $"{log.Enemy}가 {log.Weapon}로 {log.Self}을 죽였습니다.";
    }
}
