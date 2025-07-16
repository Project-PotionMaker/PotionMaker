using UnityEngine;
using System.Collections.Generic;
using System;
using Photon.Pun;
using UnityEditor;
using VInspector;
public class CustomerManager : MonoBehaviourSingleton<CustomerManager>
{
    //접수 받기, 포션 제공하기만 클라이언트에서 마스터에게 요청 가능
    //나머지는 마스터에서만 호출 가능

    private CustomerLineHandler _lineHandler; // 손님 줄을 물리적으로 관리하는 컴포넌트
    public CustomerLineHandler LineHandler { get => _lineHandler; set => _lineHandler = value; }
    private CustomerOrderHandler _orderHandler; // 주문을 처리하는 컴포넌트
    public CustomerOrderHandler OrderHandler { get => _orderHandler; set => _orderHandler = value; }
    private PhotonView _photonView;
    private int _lostCustomerCount;
    public int LostCustomerCount { get => _lostCustomerCount; set => _lostCustomerCount = value; }

    [Foldout("Inspector")]
    [SerializeField]
    private int _maxCustomerLost = 5;
    public int MaxCustomerLost { get => _maxCustomerLost; set => _maxCustomerLost = value; }
    [SerializeField]
    private float _inviteCoolTime;
    public float InviteCoolTime { get => _inviteCoolTime; set => _inviteCoolTime = value; }
    private float _inviteTimer = 0f; // 손님 초대 타이머
    private int _remainCustomers;
    public int RemainCustomers { get => _remainCustomers; set => _remainCustomers = value; }
 
    [Foldout("Hierarchy")]
    [Header("임시 포지션")]    //TODO : 임시 포지션, Layout에서 가져오는 것으로 변경 필요
    [SerializeField]
    private Transform _shopEntry;
    public Transform ShopEntry { get => _shopEntry; set => _shopEntry = value; }
    [SerializeField]
    private Transform _hallEntry; // 줄 이탈 초기위치
    public Transform HallEntry { get => _hallEntry; set => _hallEntry = value; }
    [SerializeField]
    private Transform _servingCounter; // 포션을 제공하는 판매대 위치
    public Transform ServingCounter { get => _servingCounter; set => _servingCounter = value; }
    [SerializeField]
    private Transform _counterLocation; // 접수대 위치
    public Transform CounterLocation { get => _counterLocation; set => _counterLocation = value; }
    [SerializeField]
    private Transform _exitDoor; // 손님이 나가는 문 위치
    public Transform ExitDoor { get => _exitDoor; set => _exitDoor = value; }

    // 이벤트는 필요시 추가
    protected override void Awake()
    {
        base.Awake(); 
        _orderHandler = new CustomerOrderHandler();
        _lineHandler = new CustomerLineHandler();
        _orderHandler.Init();
        _lostCustomerCount = 0;
    }
    private void Start()
    {
        _photonView = GetComponent<PhotonView>();
        PhaseManager.Instance.PhaseDictionary[EPhaseType.ServingPhase].OnPhaseEntered += SetLists;
        CustomerPoolManager.Instance.ObjectSpawnedActions.TryAdd(ENPCType.Customer, null);
        CustomerPoolManager.Instance.ObjectSpawnedActions[ENPCType.Customer] += OnCustomerIn;

    }
    public void SetLists()
    {
        _orderHandler.SetLists();
        _inviteTimer = _inviteCoolTime;
        _remainCustomers = 0;
    }

    public void InviteCustomer(float deltaTime)
    {
        if (!PhotonNetwork.IsMasterClient)
        {
            return;
        }
        _inviteTimer -= deltaTime;
        if (_inviteTimer > 0)
        {
            return;
        } 
        _inviteTimer = _inviteCoolTime;
        Debug.Log("손님 초대");
        CustomerPoolManager.Instance.GetObjectAsync(0);
        RemainCustomers++;
    }

    public void OnCustomerIn(int viewID)
    {
        Debug.Log($"손님 생성: {viewID}");
        PhotonView photonView = PhotonView.Find(viewID);
        Customer customer = photonView.GetComponent<Customer>();
        customer.transform.position = _shopEntry.position; // 손님을 상점 입구에 생성
        _orderHandler.PotionOrderLine.Enqueue(customer);
        _lineHandler.ReLining();
    }

    public void OnArrivedLine(Customer customer)
    {
        if (!PhotonNetwork.IsMasterClient)
        {
            return;
        }
        PhotonView customerView = _orderHandler.PotionOrderLine.Peek().GetComponent<PhotonView>();
        //TODO : 접수대에 손님을 등록 (접수 가능 상태)
    }

    public void RegisterOrder() // 플레이어가 접수를 받으면 호출
    {
        if (PhotonNetwork.IsMasterClient)
        {
            RegisterOrderInternal();
        }
        else
        {
            _photonView.RPC(nameof(RPC_RegisterOrder), RpcTarget.MasterClient);
        }
    }
    [PunRPC]
    private void RPC_RegisterOrder()
    {
        RegisterOrderInternal();
    }
    private void RegisterOrderInternal()
    {
        if (_orderHandler.PotionOrderLine.Count == 0)
        {
            return;
        }
        Customer customer = _orderHandler.PotionOrderLine.Dequeue();
        int potionTID = customer.GetComponent<Customer>().RequestedPotionTID;

        _orderHandler.AddOrder(potionTID, customer);
        customer.MoveTo(_hallEntry.position);
        _lineHandler.ReLining(); // 줄 다시 세우기
    }

    public void LostCustomer(Customer customer) // 인내심이 바닥나면 호출
    {
        if(PhotonNetwork.IsMasterClient == false)
        {
            return;
        }
        _orderHandler.RemoveAnywhere(customer); // 주문 목록에서 손님 제거
        _lineHandler.PutOutCustomer(customer); // 손님을 나가게 하기
        _lostCustomerCount++;
        if(_lostCustomerCount >= _maxCustomerLost)
        {
            PhaseManager.Instance.TransitionPhase(EPhaseType.EndingPhase);
        }
    }

    public void ServePotion(int potionTID)// 판매대에 올려놓으면 호출
    {
        if (PhotonNetwork.IsMasterClient)
        {
            ServePotionInternal(potionTID); 
        }
        else
        {
            _photonView.RPC(nameof(RPC_ServePotion), RpcTarget.MasterClient, potionTID);
        }
    }
    [PunRPC]
    public void RPC_ServePotion(int potionTID)
    {
        ServePotionInternal(potionTID);
    }

    public void ServePotionInternal(int potionTID)
    {
        if (PhotonNetwork.IsMasterClient == false)
        {
            return;
        }
        if (!_orderHandler.PotionOrderMap.ContainsKey(potionTID) || _orderHandler.PotionOrderMap[potionTID].Count == 0)
        {
            Debug.Log($"No customers in hall for potion TID: {potionTID}");
            return; // 해당 TID의 손님이 없으면 실패
        }
        Customer customer = _orderHandler.PotionOrderMap[potionTID].First.Value;
        customer.MoveTo(_servingCounter.position); // 손님을 판매대 위치로 이동
        //TODO : 가져가기 전까지 포션 상호작용 불가로 만들기
    }

    public void OnServedSuccess(int potionTID) // 손님이 판매대에 도착하면 호출 
    {
        if (PhotonNetwork.IsMasterClient == false)
        {
            return;
        }
        //TODO : 구매 성공, Currency 증가
        Debug.Log($"Potion served successfully for TID: {potionTID}");
        Customer customer = _orderHandler.PotionOrderMap[potionTID].First.Value;
        _orderHandler.PotionOrderMap[potionTID].RemoveFirst(); // 손님 제거
        _lineHandler.PutOutCustomer(customer); // 손님을 나가게 하기
    }

    public void OnLastOrderTime() //영업시간 종료되면 호출
    {
        if (PhotonNetwork.IsMasterClient == false)
        {
            return;
        }
        while (_orderHandler.PotionOrderLine != null && _orderHandler.PotionOrderLine.Count > 0)
        {
            Customer customer = _orderHandler.PotionOrderLine.Dequeue();
            _lineHandler.PutOutCustomer(customer);
        }
    }

    public void ReturnCustomer(Customer customer) // 손님이 출구에 도착하면 호출
    {
        if(PhotonNetwork.IsMasterClient == false)
        {
            return;
        }
        CustomerPoolManager.Instance.ReturnObject(customer.gameObject,ENPCType.Customer);
        RemainCustomers--;
    }
}
