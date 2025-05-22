using UnityEngine;

public abstract class ITransition
{
    private EntityStates state;

    public ITransition(EntityStates state)
    {
        this.state = state;
    }

    protected abstract bool Check(FSM fsm, AIInput input);
    public bool CheckTransition(FSM fsm, AIInput input)
    {
        bool result = Check(fsm, input);

        return result;
    }
}
