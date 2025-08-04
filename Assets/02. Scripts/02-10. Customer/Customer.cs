using JetBrains.Annotations;
using Mirror;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Customer : NetworkBehaviour
{
    private ECustomerStateType _currentState; // 현재 상태 컴포넌트
    public ECustomerStateType CurrentState { get => _currentState; set => _currentState = value; } // 현재 상태 컴포넌트
    private CustomerMove _customerMove; // 이동 능력 컴포넌트
    public CustomerMove CustomerMove { get => _customerMove; set => _customerMove = value; } // 이동 능력 컴포넌트
    private CustomerEndurance _customerEndurance; // 인내심
    public CustomerEndurance CustomerEndurance { get => _customerEndurance; set => _customerEndurance = value; } // 인내심 컴포넌트


    [SerializeField]
    private int _requestedPotionTID = 10000;
    public int RequestedPotionTID { get => _requestedPotionTID; set => _requestedPotionTID = value; } // 요청한 포션 ID
    private uint _servedPotionNetId;
    public uint ServedPotionNetId { get => _servedPotionNetId; set => _servedPotionNetId = value; }

    [SerializeField]
    private GameObject _potionHandler;
    public GameObject PotionHandler { get => _potionHandler; set => _potionHandler = value; } // 포션 핸들러 오브젝트

    public event Action OnStateChanged;

    private Transform _chairPosition;
    public  Transform ChairPosition { get => _chairPosition; set => _chairPosition = value; } // 의자 위치
    private  float _chairRotate;
    public float ChairRotate { get => _chairRotate; set => _chairRotate = value; } // 의자 회전
    private uint _pickupTableId;
    public uint PickupTableId { get => _pickupTableId; set => _pickupTableId = value; }

    private void Awake()
    {
        _customerMove = GetComponent<CustomerMove>();
        _customerEndurance = GetComponent<CustomerEndurance>();

    }
    private void OnEnable()
    {
        _currentState = ECustomerStateType.Lining; // 초기 상태 설정
        if(PhaseManager.Instance.PotionDataList.Count > 0 )
        {
            int index = UnityEngine.Random.Range(0, PhaseManager.Instance.PotionDataList.Count);
            _requestedPotionTID = PhaseManager.Instance.PotionDataList[index].TID;
        }
        else
        {
            Debug.LogWarning("PotionDataList is empty. Cannot assign a requested potion to the customer.");
        }
    }

    [Server]
    public void TransitionState(ECustomerStateType nextState)
    {
        RpcTransitionState(nextState);
    }
    [ClientRpc]
    public void RpcTransitionState(ECustomerStateType nextState)
    {
        _currentState = nextState;
        OnStateChanged?.Invoke();
    }
    [Server]
    public void ReturnPotion()
    {
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

    [ClientRpc]
    public void HandlePotion()
    {
        if (NetworkClient.spawned.TryGetValue(ServedPotionNetId, out NetworkIdentity identity))
        {
            GameObject potion = identity.gameObject;
            potion.transform.SetParent(_potionHandler.transform);
            potion.transform.localPosition = Vector3.zero;
        }
        else
        {
            Debug.LogWarning($"Potion with netId {ServedPotionNetId} not found on client.");
        }
    }
}
