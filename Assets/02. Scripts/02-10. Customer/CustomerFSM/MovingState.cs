using UnityEngine;

public class MovingState : BaseCustomerState
{
    public MovingState(Customer owner) : base(owner)
    {
    }
    public override void EnterState()
    {
        base.EnterState();
        _owner.CustomerMove.Animator.SetBool("Move", true);
        _owner.CustomerMove.SwitchNavmeshToAgent();
    }
    public override void Update(float deltaTime)
    {
        base.Update(deltaTime);
    }
    public override void ExitState()
    {
        base.ExitState();
        _owner.CustomerMove.Animator.SetBool("Move", false);
        _owner.CustomerMove.SwitchNavMeshToObstacle(); 
    }
}