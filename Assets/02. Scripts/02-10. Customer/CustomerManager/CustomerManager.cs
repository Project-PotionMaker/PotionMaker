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
    private bool _canOrdered = false; // 주문을 받을 수 있는 상태인지 여부

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
    private int _inviteIndex = 0;

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
        PhaseManager.Instance.PhaseDictionary[EPhaseType.EndingPhase].OnPhaseEntered += ForceReturn; // EndingPhase 진입 시 모든 손님을 반환
        //CustomerPool.Instance.ObjectSpawnedActions.TryAdd(ENPCType.Customer, null);
        //CustomerPool.Instance.ObjectSpawnedActions[ENPCType.Customer] += OnCustomerIn;

    }
    public void SetLists()
    {
        _orderHandler.SetLists();
        _inviteTimer = _inviteCoolTime;
        _remainCustomers = 0;
        _inviteIndex = 0;
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
        GameObject customer = CustomerFactory.Instance.Create(ENPCType.Customer,Vector3.zero,Quaternion.identity); // TODO : PoolManager완성 후 수정
        OnCustomerIn(customer.GetComponent<PhotonView>().ViewID); //TODO : PoolManager완성 후 수정
        //CustomerPool.Instance.GetObjectAsync(0);
        RemainCustomers++;
        _inviteIndex++;
    }

    public void OnCustomerIn(int viewID)
    {
        Debug.Log($"손님 생성: {viewID}");
        PhotonView photonView = PhotonView.Find(viewID);
        Customer customer = photonView.GetComponent<Customer>();
        customer.transform.position = _shopEntry.position; // 손님을 상점 입구에 생성
        customer.PriorityOffset = _inviteIndex*5; // 우선순위 편향 설정
        _orderHandler.PotionOrderLine.Enqueue(customer);
        _lineHandler.ReLining();
    }

    public void OnArrivedLine(Customer customer)
    {
        if (!PhotonNetwork.IsMasterClient)
        {
            return;
        }
        if(_orderHandler.PotionOrderLine.Peek() != customer)
        {
            return; // 줄에 도착했지만 첫번째 손님이 아니면 무시
        }
        PhotonView customerView = _orderHandler.PotionOrderLine.Peek().GetComponent<PhotonView>();
        _canOrdered = true;
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
        if(_canOrdered == false)
        {
            return;
        }
        _canOrdered = false; // 주문을 받은 후에는 다시 주문을 받을 수 없도록 설정
        Customer customer = _orderHandler.PotionOrderLine.Dequeue();
        int potionTID = customer.GetComponent<Customer>().RequestedPotionTID;

        _orderHandler.AddOrder(potionTID, customer);
        customer.CustomerMove.MoveTo(_hallEntry.position);
        customer.SetCurrentState(ECustomerStateType.Waiting); // 대기 상태로 변경
        _lineHandler.ReLining(); // 줄 다시 세우기
    }

    public void LostCustomer(Customer customer) // 인내심이 바닥나면 호출
    {
        if(PhotonNetwork.IsMasterClient == false)
        {
            return;
        }
        customer.SetCurrentState(ECustomerStateType.Leaving);
        if (_orderHandler.RemoveAnywhere(customer))// 주문 목록에서 손님 제거
        {
            _lineHandler.ReLining(); // 줄 다시 세우기
        }
        _lineHandler.PutOutCustomer(customer); // 손님을 나가게 하기
        _lostCustomerCount++;
        if(_lostCustomerCount >= _maxCustomerLost)
        {
            PhaseManager.Instance.TransitionPhase(EPhaseType.EndingPhase);
        }
    }

    public void ServePotion(int potionTID, Furniture pickupTable)// 판매대에 올려놓으면 호출
    {
        if (PhotonNetwork.IsMasterClient)
        {
            ServePotionInternal(potionTID,pickupTable); 
        }
        else
        {
            _photonView.RPC(nameof(RPC_ServePotion), RpcTarget.MasterClient, potionTID);
        }
    }
    [PunRPC]
    public void RPC_ServePotion(int potionTID, Furniture pickupTable)
    {
        ServePotionInternal(potionTID,pickupTable);
    }

    public void ServePotionInternal(int potionTID,Furniture pickupTable)
    {
        if (PhotonNetwork.IsMasterClient == false)
        {
            return;
        }
        Customer customer = _orderHandler.FindPicker(potionTID); // 포션을 가져갈 손님 찾기
        if (ReferenceEquals(customer, null)) 
        {
            return;
        }
        customer.CustomerMove.MoveTo(pickupTable.transform.position); // 손님을 판매대 위치로 이동
        customer.SetCurrentState(ECustomerStateType.PickingUp);
        _orderHandler.PickingCustomers[pickupTable] = customer; // 손님과 판매대 매핑 저장
        _orderHandler.PotionOrderMap[potionTID].Remove(customer);
    }

    public void OnServedSuccess(Customer customer,Furniture pickupTable) // 손님이 판매대에 도착하면 호출 
    {
        if (PhotonNetwork.IsMasterClient == false)
        {
            return;
        }
        //TODO : 구매 성공, Currency 증가
        Debug.Log($"Potion served successfully");
        _orderHandler.PickingCustomers[pickupTable] = null;
        GameObject potion = pickupTable.GetComponent<IGridItemHandler>().TryPickUp(); // 판매대 위치에서 포션 오브젝트 가져오기
        potion.transform.SetParent(customer.PotionHandler.transform);
        potion.transform.localPosition = Vector3.zero;
        _lineHandler.PutOutCustomer(customer); // 손님을 나가게 하기
        customer.SetCurrentState(ECustomerStateType.Leaving); // 손님 상태를 나가는 상태로 변경

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
            customer.SetCurrentState(ECustomerStateType.Leaving); // 손님 상태를 나가는 상태로 변경
        }
    }

    public void ReturnCustomer(Customer customer) // 손님이 출구에 도착하면 호출
    {
        if(PhotonNetwork.IsMasterClient == false)
        {
            return;
        }
        CustomerFactory.Instance.Return(customer.gameObject); // TODO : PoolManager완성 후 수정
        //CustomerPool.Instance.ReturnObject(customer.gameObject,ENPCType.Customer);
        RemainCustomers--;
    }
    public void ForceReturn() // 인내심 바닥나서 끝나면 전부 강제로 내보냄, 또는 버그로 큐에 남아있는 손님도 내보냄
    {
        if (PhotonNetwork.IsMasterClient == false)
        {
            return;
        }
        Debug.Log("Force returning all customers.");
        while (_orderHandler.PotionOrderLine.Count > 0)
        {
            Customer customer = _orderHandler.PotionOrderLine.Dequeue();
            ReturnCustomer(customer);
        }
        foreach (var potionQueue in _orderHandler.PotionOrderMap.Values)
        {
            while (potionQueue.Count > 0)
            {
                Customer customer = potionQueue.First.Value;
                potionQueue.RemoveFirst();
                ReturnCustomer(customer);
            }
        }
    }
}
