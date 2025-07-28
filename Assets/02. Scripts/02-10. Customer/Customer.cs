using Photon.Pun;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Customer : MonoBehaviour
{
    private ECustomerStateType _currentState; // 현재 상태 컴포넌트
    public ECustomerStateType CurrentState { get => _currentState; set => _currentState = value; } // 현재 상태 컴포넌트
    private CustomerMove _customerMove; // 이동 능력 컴포넌트
    public CustomerMove CustomerMove { get => _customerMove; set => _customerMove = value; } // 이동 능력 컴포넌트
    private CustomerEndurance _customerEndurance; // 인내심
    public CustomerEndurance CustomerEndurance { get => _customerEndurance; set => _customerEndurance = value; } // 인내심 컴포넌트


    [SerializeField]
    private int _requestedPotionTID = 10000;
    [SerializeField]
    private GameObject _potionHandler;
    public GameObject PotionHandler { get => _potionHandler; set => _potionHandler = value; } // 포션 핸들러 오브젝트
    public int RequestedPotionTID { get => _requestedPotionTID; set=> _requestedPotionTID = value; } // 요청한 포션 ID

    private PhotonView _photonView;
    public PhotonView PhotonView { get => _photonView; set => _photonView = value; } // PhotonView 컴포넌트

    public event Action OnStateChanged;

    private GameObject _chair;
    public GameObject Chair { get => _chair; set => _chair = value; } // 의자 오브젝트

    private void Awake()
    {
        _photonView = GetComponent<PhotonView>();
        _customerMove = GetComponent<CustomerMove>();
        _customerEndurance = GetComponent<CustomerEndurance>();

    }
    private void OnEnable()
    {
        _currentState = ECustomerStateType.Lining; // 초기 상태 설정
        //_requestedPotionTID = RandomPotion();
    }

    public void TransitionState(ECustomerStateType nextState)
    {
        if (!PhotonNetwork.IsMasterClient)
        {
            return; // 마스터 클라이언트만 상태를 설정할 수 있음
        }
        _photonView.RPC(nameof(RPC_TransitionState), RpcTarget.All, nextState); 
    }
    [PunRPC]
    public void RPC_TransitionState(ECustomerStateType nextState)
    {
        _currentState = nextState;
        OnStateChanged?.Invoke();
    }

    public void ReturnPotion()
    {
        if (!PhotonNetwork.IsMasterClient)
        {
            return; // 마스터 클라이언트만 포션을 반환할 수 있음
        }
        if (_potionHandler.transform.childCount == 0)
        {
            return; 
        }
        GameObject potion = _potionHandler.transform.GetChild(0).gameObject;
        if(ReferenceEquals(potion, null) == false)
        {
            potion.transform.SetParent(null); 
            CraftItemFactory.Instance.Return(potion); 
        }

    }
}
