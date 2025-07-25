using System;
using UnityEngine;

public abstract class BaseCustomerState
{
    protected Customer _owner;
    private ECustomerStateType _stateType;
    public ECustomerStateType StateType {  get => _stateType; set => _stateType = value;}

    public event Action OnStateEntered;
    public event Action OnStateExited;
    public BaseCustomerState(Customer owner)
    {
        _owner = owner;
    }

    public virtual void EnterState()
    {
        OnStateEntered?.Invoke();
    }
    public virtual void Update(float dletaTime)
    {
    }
    public virtual void ExitState()
    {
        OnStateExited?.Invoke();
    }
}
