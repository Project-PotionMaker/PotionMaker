using UnityEngine;
using System.Collections.Generic;
using System;
using Photon.Pun;
using UnityEditor;
using VInspector;
using Mirror;
public class CustomerManager : NetworkBehaviour
{
    public static CustomerManager Instance { get; private set; } // 싱글톤 인스턴스

    //접수 받기, 포션 제공하기만 클라이언트에서 마스터에게 요청 가능
    //ServerOnly켜야함

    private CustomerLineHandler _lineHandler; // 손님 줄을 물리적으로 관리하는 컴포넌트
    public CustomerLineHandler LineHandler { get => _lineHandler; set => _lineHandler = value; }
    private CustomerOrderHandler _orderHandler; // 주문을 처리하는 컴포넌트
    public CustomerOrderHandler OrderHandler { get => _orderHandler; set => _orderHandler = value; }
    private bool _canOrdered = false; 
    public bool CanOrdered { get => _canOrdered; set => _canOrdered = value; }

    [Foldout("Inspector")]
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
    private Transform _enterDoor;
    public Transform EnterDoor { get => _enterDoor; set => _enterDoor = value; }
    private Transform _casherLocation; // 접수대 위치
    public Transform CasherLocation { get => _casherLocation; set => _casherLocation = value; }
    [SerializeField]
    private Transform _exitDoor; // 손님이 나가는 문 위치
    public Transform ExitDoor { get => _exitDoor; set => _exitDoor = value; }

    // 이벤트는 필요시 추가
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // 씬 전환 시에도 유지
        }
        else
        {
            Debug.LogWarning("중복된 CustomerManager 인스턴스 발견됨. 기존 인스턴스를 유지합니다.");
            Destroy(gameObject);
            return;
        }
        _orderHandler = new CustomerOrderHandler();
        _lineHandler = new CustomerLineHandler();
        _orderHandler.Init();
    }
    private void Start()
    {
        Dictionary<EPhaseType, BasePhase> phaseDictionary = PhaseManager.Instance.PhaseDictionary;
        phaseDictionary[EPhaseType.ServingPhase].OnPhaseEntered += PreService;
        phaseDictionary[EPhaseType.ServingPhase].OnPhaseExited += ForceReturn;
        phaseDictionary[EPhaseType.PracticingPhase].OnPhaseEntered += PreService;
        phaseDictionary[EPhaseType.PracticingPhase].OnPhaseExited += ForceReturn;
        
        //CustomerPool.Instance.ObjectSpawnedActions.TryAdd(ENPCType.Customer, null);
        //CustomerPool.Instance.ObjectSpawnedActions[ENPCType.Customer] += OnCustomerIn;

    }
    public void PreService()
    {
        _orderHandler.SetLists();
        _casherLocation = GridManager.Instance.Casher.transform;
        _inviteTimer = _inviteCoolTime;
        _remainCustomers = 0;
        _inviteIndex = 0;
    }

    public void InviteCustomer(float deltaTime)
    {
        if (!isServer)
        {
            return;
        }
        _inviteTimer -= deltaTime;
        if (_inviteTimer > 0)
        {
            return;
        } 
        //TODO : Layout에서 최대 줄 길이 가져와서 적용하기
        _inviteTimer = _inviteCoolTime;
        Debug.Log("손님 초대");
        GameObject customer = CustomerFactory.Instance.Create(ENPCType.Customer,Vector3.zero,Quaternion.identity); // TODO : PoolManager완성 후 수정
        customer.transform.position = _enterDoor.position;
        _orderHandler.PotionOrderLine.Enqueue(customer.GetComponent<Customer>());
        _lineHandler.ReLining();
        //CustomerPool.Instance.GetObjectAsync(0);
        RemainCustomers++;
        _inviteIndex++;
    }

    [Command]
    public void CommandRegisterOrder()
    {
        if (_orderHandler.PotionOrderLine.Count == 0)
        {
            return;
        }
        if(_canOrdered == false)
        {
            return;
        }
        uint chairNetworkId = _orderHandler.FindAvailableChair(); // 사용 가능한 의자 찾기
        if (chairNetworkId == 0)
        {
            Debug.Log("No available chair found for the customer.");
            return; // 사용 가능한 의자가 없으면 주문을 받지 않음
        }
        _canOrdered = false; // 주문을 받은 후에는 다시 주문을 받을 수 없도록 설정
        Customer customer = _orderHandler.PotionOrderLine.Dequeue();
        int potionTID = customer.GetComponent<Customer>().RequestedPotionTID;
        _orderHandler.AddOrder(potionTID, customer);
        customer.TransitionState(ECustomerStateType.Sitting); 
        SitOnChair(chairNetworkId, customer);
        customer.CustomerEndurance.ResetEndurance(); 
        _lineHandler.ReLining(); // 줄 다시 세우기
        ServePotionOnTakeOrder();
    }

    public void LostCustomer(Customer customer) // 인내심이 바닥나면 호출
    {
        if (_orderHandler.RemoveAnywhere(customer))// 주문 목록에서 손님 제거
        {
            _lineHandler.ReLining(); // 줄 다시 세우기
        }
        LeaveChair(customer);
        _lineHandler.PutOutCustomer(customer); // 손님을 나가게 하기
        if(PhaseManager.Instance.CurrentPhase.PhaseType == EPhaseType.ServingPhase)
        {
            PhaseManager.Instance.DeathCount++;
            if (PhaseManager.Instance.DeathCount >= PhaseManager.Instance.MaxDeathCount)
            {
                //TODO : 게임종료 씬
            }
        }
    }

    private void ServePotionOnTakeOrder()
    {
        foreach (KeyValuePair<uint, FurnitureUsingStat> pair in _orderHandler.PickupTableDict)
        {
            if (pair.Value.IsUsing == true && pair.Value.UsingCustomer == null)
            {
                CommandServePotion(pair.Value.HeldItemTID,pair.Key);
            }
        }
    }

    [Command]
    public void CommandServePotion(int potionTID,uint pickupTableNetworkId)
    {
        Customer customer = _orderHandler.FindPicker(potionTID); // 포션을 가져갈 손님 찾기
        if (ReferenceEquals(customer, null)) 
        {
            return;
        }
        Vector3 position = NetworkServer.spawned[pickupTableNetworkId].transform.position; // 판매대 위치 찾기
        customer.TransitionState(ECustomerStateType.PickingUp);
        customer.CustomerMove.MoveTo(position); // 손님을 판매대 위치로 이동
        customer.PickupTableNetworkId = pickupTableNetworkId; // 손님이 판매대의 PhotonView ID 저장
        _orderHandler.PickupTableDict[pickupTableNetworkId].UsingCustomer = customer; // 손님과 판매대 매핑 저장
        _orderHandler.PotionOrderMap[potionTID].Remove(customer);
        LeaveChair(customer);

    }

    public void OnServedSuccess(Customer customer) // 손님이 판매대에 도착하면 호출 
    {
        if(PhaseManager.Instance.CurrentPhase.PhaseType == EPhaseType.ServingPhase)
        {
            //TODO : 구매 성공, Currency 증가
        }
        Debug.Log($"Potion served successfully");
        GameObject potion = NetworkServer.spawned[customer.PickupTableNetworkId].GetComponent<IGridItemHandler>().TryPickUp(); // 판매대 위치에서 포션 오브젝트 가져오기
        potion.transform.SetParent(customer.PotionHandler.transform);
        potion.transform.localPosition = Vector3.zero;
        _lineHandler.PutOutCustomer(customer); // 손님을 나가게 하기
    }

    public void OnLastOrderTime() //영업시간 종료되면 호출
    {
        while (_orderHandler.PotionOrderLine.Count > 0)
        {
            Customer customer = _orderHandler.PotionOrderLine.Dequeue();
            _lineHandler.PutOutCustomer(customer);
            customer.TransitionState(ECustomerStateType.Leaving); // 손님 상태를 나가는 상태로 변경
        }
    }

    public void ReturnCustomer(Customer customer) // 손님이 출구에 도착하면 호출
    {
        customer.ReturnPotion();
        CustomerFactory.Instance.Return(customer.gameObject); // TODO : PoolManager완성 후 수정
        //CustomerPool.Instance.ReturnObject(customer.gameObject,ENPCType.Customer);
        RemainCustomers--;
    }
    public void ForceReturn() // 인내심 바닥나서 끝나면 전부 강제로 내보냄, 또는 버그로 큐에 남아있는 손님도 내보냄
    {
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
        foreach(var pair in _orderHandler.PickupTableDict)
        {
            if (pair.Value.IsUsing && pair.Value.UsingCustomer != null)
            {
                ReturnCustomer(pair.Value.UsingCustomer);
            }
        }
    }

    public void PlaceOnTable(int potionTID, uint pickupTableNetworkId)
    {
        _orderHandler.PickupTableDict[pickupTableNetworkId].IsUsing = true;
        _orderHandler.PickupTableDict[pickupTableNetworkId].HeldItemTID = potionTID;
    }
    public void RemoveOnTable(uint pickupTableNetworkId)
    {
        _orderHandler.PickupTableDict[pickupTableNetworkId].IsUsing = false;
        _orderHandler.PickupTableDict[pickupTableNetworkId].HeldItemTID = 0;
        _orderHandler.PickupTableDict[pickupTableNetworkId].UsingCustomer = null; 
    }
    private void SitOnChair(uint chairNetworkId,Customer customer)
    {
        if(_orderHandler.LuxuryChairDict.ContainsKey(chairNetworkId))
        {
            _orderHandler.LuxuryChairDict[chairNetworkId].IsUsing = true;
            _orderHandler.LuxuryChairDict[chairNetworkId].UsingCustomer = customer; // 손님과 의자 매핑 저장
        }
        else if (_orderHandler.OldChairDict.ContainsKey(chairNetworkId))
        {
            _orderHandler.OldChairDict[chairNetworkId].IsUsing = true;
            _orderHandler.OldChairDict[chairNetworkId].UsingCustomer = customer; // 손님과 의자 매핑 저장
        }
        var chair = NetworkServer.spawned[chairNetworkId];
        customer.CustomerMove.MoveTo(chair.transform.position);
        chair.GetComponent<Furniture>().TryEffect(customer); // 의자 효과 적용
    }
    private void LeaveChair(Customer customer)
    {
        FurnitureUsingStat usedChair = _orderHandler.FindUsingChair(customer);
        if (usedChair != null)
        {
            usedChair.IsUsing = false;
            usedChair.UsingCustomer = null; // 손님과 의자 매핑 해제
        }
    }
}
