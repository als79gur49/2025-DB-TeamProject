using TMPro;
using UnityEngine;

public class DamagePopupManager : MonoBehaviour
{
    //�̱������� ����

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

            //�̵� ���

            //������� ���
        }
    }
};
