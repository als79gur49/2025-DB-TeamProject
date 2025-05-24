using TMPro;
using UnityEngine;

public class DamagePopupManager : MonoBehaviour
{
    //싱글턴으로 변경

    [SerializeField]
    private DamagePopup popupPrefab;

    private MemoryPool<DamagePopup> popupPool;

    private void Awake()
    {
        popupPool = new MemoryPool<DamagePopup>(popupPrefab, 10);
    }

    public void PrintDamage(Color c, int amount)
    {
        DamagePopup popup = popupPool?.ActivatePoolItem();
        TextMeshProUGUI text = popup?.GetComponent<TextMeshProUGUI>();
        if (text != null)
        {
            text.text = amount.ToString();

            text.color = c;

            //이동 모션

            //사라지는 모션
        }
    }
};
