using UnityEngine;

public class ReturningLine : MovingState
{
    public ReturningLine(Customer owner) : base(owner)
    {
        StateType = ECustomerStateType.ReturningLine;
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
