using Photon.Pun;
using Unity.VisualScripting;
using UnityEngine;

public abstract class CustomerBaseState
{
    protected ECustomerStateType _stateType;
    public ECustomerStateType StateType { get => _stateType; set => _stateType = value; }
    protected Customer _owner;
    public CustomerBaseState(Customer owner)
    {
        _owner = owner;
    }
    public virtual void EnterState()
    {
        if (PhotonNetwork.IsMasterClient == false)
        {
            return; // 마스터 클라이언트만 이동 가능
        }
        Debug.Log($"{_owner.name} entered state: {StateType}");
    }
    public virtual void Update()
    {
        if (PhotonNetwork.IsMasterClient == false)
        {
            return; // 마스터 클라이언트만 이동 가능
        }
    }
    public virtual void ExitState()
    {
        if (PhotonNetwork.IsMasterClient == false)
        {
            return; // 마스터 클라이언트만 이동 가능
        }
        Debug.Log($"{_owner.name} exited state: {StateType}");
    }
}
