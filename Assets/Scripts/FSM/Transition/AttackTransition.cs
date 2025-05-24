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
        // 목표 방향에 지형지물이 없을 경우
        if (Physics.Raycast(input.self.transform.position, (input.target.transform.position - input.self.transform.position).normalized, 100, groundLayer))
        {
            return false;
        }
        // 범위 안에 있을 경우
        if (input.DistanceToTarget() > attackRange)
        {
            return false;
        }

        return true;
    }
}
