using UnityEngine;
using UnityEngine.UI;

public class EntityUI : MonoBehaviour
{
    [SerializeField]
    private Slider hpSlider;

    [SerializeField]
    private Entity entity;


    private void OnEnable()
    {
        entity.onTakeDamage.AddListener(UpdateHPHUD);

        hpSlider.value = 1;
    }

    public void UpdateHPHUD(float current, float max)
    {
        //Debug.Log($"{current} / {max}hp");
        if(current <= 0)
        {
            hpSlider.value = 0;

            return;
        }

        hpSlider.value = current / max;
    }
}
