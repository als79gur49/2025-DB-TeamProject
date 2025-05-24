using UnityEngine;

public class AttackTransition : ITransition
{
    private int attackRange = 10;
    private LayerMask groundLayer = (1 << 11);

    public AttackTransition(EntityStates state, int attackRange) : base(state)
    {
        this.attackRange = attackRange;
    }

    protected override bool Check(AIInput input)
    {
        // ��ǥ ���⿡ ���������� ���� ���
        if (Physics.Raycast(input.self.transform.position, (input.target.transform.position - input.self.transform.position).normalized, 100, groundLayer))
        {
            return false;
        }
        // ���� �ȿ� ���� ���
        if (input.DistanceToTarget() > attackRange)
        {
            return false;
        }

        return true;
    }
}
