using UnityEngine;

public class CustomerOutState : CustomerBaseState
{
    public CustomerOutState(Customer owner) : base(owner)
    {
        _stateType = ECustomerStateType.Out;
    }
}
