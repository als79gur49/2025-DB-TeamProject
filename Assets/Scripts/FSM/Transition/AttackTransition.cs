using UnityEngine;

public class AttackTransition : ITransition
{
    private int attackRange = 10;

    public AttackTransition(EntityStates state, int attackRange) : base(state)
    {
        this.attackRange = attackRange;
    }

    protected override bool Check(AIInput input)
    {
        if (input.DistanceToTarget() <= attackRange)
        {
            return true;
        }

        return false;
    }

}
