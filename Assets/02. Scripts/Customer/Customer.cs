using Photon.Pun;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Customer : MonoBehaviour
{
    private ECustomerStateType _currentState;
    public ECustomerStateType CurrentState { get => _currentState; } // 현재 상태
    private CustomerMoveAbility _moveAbility; // 이동 능력 컴포넌트
    public CustomerMoveAbility MoveAbility { get => _moveAbility; set => _moveAbility = value; } // 이동 능력 컴포넌트
    private CustomerEnduranceAbility _enduranceAbility; // 인내심
    public CustomerEnduranceAbility EnduranceAbility { get => _enduranceAbility; set => _enduranceAbility = value; } // 인내심 컴포넌트

    private int _requestedPotionTID = 0;
    public int RequestedPotionTID { get => _requestedPotionTID; set=> _requestedPotionTID = value; } // 요청한 포션 ID

    private PhotonView _photonView;
    public PhotonView PhotonView { get => _photonView; set => _photonView = value; } // PhotonView 컴포넌트

    private int _priorityOffset;
    public int PriorityOffset { get => _priorityOffset; set => _priorityOffset = value; } // 우선순위 편향

    public event Action OnStateChanged; // 상태 변경 이벤트

    private void Awake()
    {
        _photonView = GetComponent<PhotonView>();
        _moveAbility = GetComponent<CustomerMoveAbility>();
        _enduranceAbility = GetComponent<CustomerEnduranceAbility>();
    }
    private void OnEnable()
    {
        _currentState = ECustomerStateType.Lining; 
        //_requestedPotionTID = RandomPotion();
    }

    public void SetCurrentState(ECustomerStateType nextState)
    {
        if (!PhotonNetwork.IsMasterClient)
        {
            return; // 마스터 클라이언트만 상태를 설정할 수 있음
        }
        _photonView.RPC(nameof(RPC_SetCurrentState), RpcTarget.All, nextState); 
    }
    [PunRPC]
    public void RPC_SetCurrentState(ECustomerStateType nextState)
    {
        Debug.Log(nextState);
        _currentState = nextState; 
        OnStateChanged?.Invoke();
    }
}
