using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChaseTransition : ITransition
{
    public ChaseTransition(EntityStates state) : base(state)
    {

    }

    protected override bool Check(FSM fsm, AIInput input)
    {
        if(input.DistanceToTarget() <= fsm.ChaseRange)
        {
            return true;
        }

        return false;
    }

}
