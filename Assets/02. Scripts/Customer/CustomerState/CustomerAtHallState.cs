using UnityEngine;

public class CustomerAtHallState : CustomerBaseState
{
    public CustomerAtHallState(Customer owner) : base(owner)
    {
        _stateType = ECustomerStateType.AtHall;
    }
}
