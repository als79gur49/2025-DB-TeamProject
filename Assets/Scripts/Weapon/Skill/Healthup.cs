using UnityEngine;

public class Healthup : BaseSkill
{
    public override void Setup(Entity owner, SOSkill data = null)
    {
        base.Setup(owner, data);
    }

    public override void LevelUp()
    {
        base.LevelUp();
        Debug.Log("�ִ� ü�� ���� tmp");
        owner.Data.IncreaseMaxHp(20);
    }
}
