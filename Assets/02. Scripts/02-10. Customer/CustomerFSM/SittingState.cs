using UnityEngine;

public class SittingState : WaitingState
{
    public SittingState(Customer owner) : base(owner)
    {
        StateType = ECustomerStateType.Sitting;
    }
    public override void EnterState()
    {
        base.EnterState();
        _owner.CustomerMove.Animator.SetTrigger("Sit");
    }
}
