using UnityEngine;

public class CustomerAtCounterState : CustomerBaseState
{
    public CustomerAtCounterState(Customer owner) : base(owner)
    {
        _stateType = ECustomerStateType.AtCounter;
    }
}
