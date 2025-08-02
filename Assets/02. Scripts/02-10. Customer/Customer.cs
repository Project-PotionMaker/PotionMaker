using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Customer : NetworkBehaviour
{
    [SyncVar (hook = nameof(SyncState))]
    private ECustomerStateType _currentState; // 현재 상태 컴포넌트
    public ECustomerStateType CurrentState { get => _currentState; set => _currentState = value; } // 현재 상태 컴포넌트
    private CustomerMove _customerMove; // 이동 능력 컴포넌트
    public CustomerMove CustomerMove { get => _customerMove; set => _customerMove = value; } // 이동 능력 컴포넌트
    private CustomerEndurance _customerEndurance; // 인내심
    public CustomerEndurance CustomerEndurance { get => _customerEndurance; set => _customerEndurance = value; } // 인내심 컴포넌트


    [SerializeField]
    [SyncVar]
    private int _requestedPotionTID = 10000;
    public int RequestedPotionTID { get => _requestedPotionTID; set => _requestedPotionTID = value; } // 요청한 포션 ID
    [SerializeField]
    private GameObject _potionHandler;
    public GameObject PotionHandler { get => _potionHandler;} // 포션 핸들러 오브젝트

    public event Action OnStateChanged;

    private Transform _chairPosition;
    public  Transform ChairPosition { get => _chairPosition; set => _chairPosition = value; } // 의자 위치
    private  float _chairRotate;
    public float ChairRotate { get => _chairRotate; set => _chairRotate = value; } // 의자 회전

    private uint _pickupTableNetworkId;
    public uint PickupTableNetworkId { get => _pickupTableNetworkId; set => _pickupTableNetworkId = value; } // 픽업 테이블의 PhotonView ID

    private void Awake()
    {
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
        if(isServer)
        {
            _currentState = nextState;
        }
    }

    private void SyncState(ECustomerStateType OldValue, ECustomerStateType NewValue)
    {
        OnStateChanged?.Invoke();
    }

    public void ReturnPotion()
    {
        if(isServer == false)
        {
            return; // 서버에서만 실행
        }
        if (_potionHandler.transform.childCount == 0)
        {
            return; 
        }
        GameObject potion = _potionHandler.transform.GetChild(0).gameObject;
        if(ReferenceEquals(potion, null) == false)
        {
            potion.transform.SetParent(null); 
            CraftItemFactory.Instance.CmdReturn(potion); 
        }

    }
}
