using UnityEngine;

public class PickingUpState : MovingState
{
    public PickingUpState(Customer owner) : base(owner)
    {
        StateType = ECustomerStateType.PickingUp;
    }

  
 
    public override void ExitState()
    {
        base.ExitState();

        CustomerManager.Instance.OnServedSuccess(_owner, _owner.RequestedPotionTID); 
    }
}
