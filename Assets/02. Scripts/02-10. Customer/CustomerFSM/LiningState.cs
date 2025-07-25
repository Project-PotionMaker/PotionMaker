using UnityEngine;

public class LiningState : WaitingState
{
    public LiningState(Customer owner) : base(owner)
    {
        StateType = ECustomerStateType.Lining;
    }
    public override void EnterState()
    {
        base.EnterState();
        _owner.CustomerMove.Animator.SetTrigger("Stand");
        if (ReferenceEquals(_owner, CustomerManager.Instance.OrderHandler.PotionOrderLine.Peek()))
        {
            CustomerManager.Instance.CanOrdered = true;
        }
    }
}