using UnityEngine;

public class CustomerAtLineState : CustomerBaseState
{
    public CustomerAtLineState(Customer owner) : base(owner)
    {
        _stateType = ECustomerStateType.AtLine;
    }
}
