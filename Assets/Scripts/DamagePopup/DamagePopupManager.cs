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
        popupPool = new MemoryPool<DamagePopup>(popupPrefab, this.transform, 10);
    }

    public void PrintDamage(Color c, int amount, Vector3 point, float duration = 5f)
    {
        DamagePopup popup = popupPool?.ActivatePoolItem();

        popup.Setup(c, amount, point, duration, this);
    }

    public void DeactiveSelf(DamagePopup popup)
    {
        popupPool?.DeactivatePoolItem(popup);
    }
};
