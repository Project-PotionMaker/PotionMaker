using UnityEngine;

public class ReturningChairState : MovingState
{
    public ReturningChairState(Customer owner) : base(owner)
    {
        StateType = ECustomerStateType.ReturningChair;
    }
    public override void EnterState()
    {
        base.EnterState();
    }
    public override void Update(float deltaTime)
    {
        base.Update(deltaTime);
    }
    public override void ExitState()
    {
        base.ExitState();
    }
}
