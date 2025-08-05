using System.Collections.Generic;
using UnityEngine;
using VInspector;
using Mirror;
using UnityEngine.SceneManagement;
public class CustomerManager : NetworkBehaviourSingleton<CustomerManager>
{
    //접수 받기, 포션 제공하기만 클라이언트에서 마스터에게 요청 가능
    //나머지는 마스터에서만 호출 가능

    private CustomerLineHandler _lineHandler; // 손님 줄을 물리적으로 관리하는 컴포넌트
    public CustomerLineHandler LineHandler { get => _lineHandler; set => _lineHandler = value; }
    private CustomerOrderHandler _orderHandler; // 주문을 처리하는 컴포넌트
    public CustomerOrderHandler OrderHandler { get => _orderHandler; set => _orderHandler = value; }
    //private PhotonView _photonView;
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
    protected override void Awake()
    {
        base.Awake(); 
        _orderHandler = new CustomerOrderHandler();
        _lineHandler = new CustomerLineHandler();
        _orderHandler.Init();
    }
    public override void OnStartClient()
    {
        Dictionary<EPhaseType, BasePhase> phaseDictionary = PhaseManager.Instance.PhaseDictionary;
        phaseDictionary[EPhaseType.ServingPhase].OnPhaseEntered += PreService;
        phaseDictionary[EPhaseType.ServingPhase].OnPhaseExited += ForceReturn; 
        phaseDictionary[EPhaseType.PracticingPhase].OnPhaseEntered += PreService;
        phaseDictionary[EPhaseType.PracticingPhase].OnPhaseExited += ForceReturn;
        //CustomerPool.Instance.ObjectSpawnedActions.TryAdd(ENPCType.Customer, null);
        //CustomerPool.Instance.ObjectSpawnedActions[ENPCType.Customer] += OnCustomerIn;

        _enterDoor = GameObject.FindGameObjectWithTag(nameof(ETags.EnterDoor))?.transform;
        _exitDoor = GameObject.FindGameObjectWithTag(nameof(ETags.ExitDoor))?.transform;

        SceneManager.sceneLoaded += OnSceneLoad;
    }

    public void OnSceneLoad(Scene scene, LoadSceneMode mode)
    {
        _enterDoor = GameObject.FindGameObjectWithTag(nameof(ETags.EnterDoor))?.transform;
        _exitDoor = GameObject.FindGameObjectWithTag(nameof(ETags.ExitDoor))?.transform;
    }

    [Server]
    public void PreService()
    {
        _orderHandler.SetLists();
        _casherLocation = GridManager.Instance.Casher.transform;
        _inviteTimer = _inviteCoolTime;
        _remainCustomers = 0;
        _inviteIndex = 0;
    }
    
    [Server]
    public void InviteCustomer(float deltaTime)
    {
        _inviteTimer -= deltaTime;
        if (_inviteTimer > 0)
        {
            return;
        } 
        //TODO : Layout에서 최대 줄 길이 가져와서 적용하기
        _inviteTimer = _inviteCoolTime;
        Debug.Log("손님 초대");
        GameObject customer = CustomerFactory.Instance.CreateObject(ENPCType.Customer,Vector3.zero,Quaternion.identity); // TODO : PoolManager완성 후 수정
        //OnCustomerIn(customer.GetComponent<PhotonView>().ViewID); //TODO : PoolManager완성 후 수정
        //CustomerPool.Instance.GetObjectAsync(0);
        customer.transform.position = _enterDoor.position; // 손님을 상점 입구에 생성
        _orderHandler.PotionOrderLine.Enqueue(customer.GetComponent<Customer>());
        RemainCustomers++;
        _inviteIndex++;
        _lineHandler.ReLining();
    }

    [Command(requiresAuthority = false)]
    public void CmdRegisterOrder() // 플레이어가 접수를 받으면 호출
    {
        if (_orderHandler.PotionOrderLine.Count == 0)
        {
            return;
        }
        if (_canOrdered == false)
        {
            return;
        }
        uint chairNetId = _orderHandler.FindAvailableChair(); // 사용 가능한 의자 찾기
        if (chairNetId == 0)
        {
            Debug.Log("No available chair found for the customer.");
            return; // 사용 가능한 의자가 없으면 주문을 받지 않음
        }
        _canOrdered = false; // 주문을 받은 후에는 다시 주문을 받을 수 없도록 설정
        Customer customer = _orderHandler.PotionOrderLine.Dequeue();
        int potionTID = customer.GetComponent<Customer>().RequestedPotionTID;
        _orderHandler.AddOrder(potionTID, customer);
        customer.TransitionState(ECustomerStateType.Sitting);
        SitOnChair(chairNetId, customer);
        customer.CustomerEndurance.ResetEndurance();
        _lineHandler.ReLining(); // 줄 다시 세우기
        ServePotionOnTakeOrder();
    }
    [Server]
    public void LostCustomer(Customer customer) // 인내심이 바닥나면 호출
    {
        if (_orderHandler.RemoveAnywhere(customer))// 주문 목록에서 손님 제거
        {
            _lineHandler.ReLining(); // 줄 다시 세우기
        }
        ReputationManager.Instance.SubtractReputation();
        LeaveChair(customer);
        _lineHandler.PutOutCustomer(customer); // 손님을 나가게 하기
        if(PhaseManager.Instance.CurrentPhase.PhaseType == EPhaseType.ServingPhase)
        {
            PhaseManager.Instance.DeathCount--;
            if (PhaseManager.Instance.DeathCount <= 0 )
            {
                //TODO : 게임종료 씬
            }
        }
    }
    [Server]
    private void ServePotionOnTakeOrder()
    {
        foreach (KeyValuePair<uint, FurnitureUsingStat> pair in _orderHandler.PickupTableDict)
        {
            if (pair.Value.IsUsing == true && pair.Value.UsingCustomer == null)
            {
                CmdServePotion(pair.Value.HeldItemTID,pair.Key);
            }
        }
    }
    [Command(requiresAuthority = false)]
    public void CmdServePotion(int potionTID, uint pickupTableNetId)// 판매대에 올려놓으면 호출
    {
        Customer customer = _orderHandler.FindPicker(potionTID); // 포션을 가져갈 손님 찾기
        if (ReferenceEquals(customer, null))
        {
            return;
        }

        //CmdPlaceOnTable(potionTID, pickupTableNetId);
        Vector3 position = NetworkServer.spawned[pickupTableNetId].transform.position; // 판매대 위치 찾기
        customer.TransitionState(ECustomerStateType.PickingUp);
        customer.CustomerMove.MoveTo(position); // 손님을 판매대 위치로 이동
        customer.PickupTableId = pickupTableNetId;
        _orderHandler.PickupTableDict[pickupTableNetId].UsingCustomer = customer; // 손님과 판매대 매핑 저장
        _orderHandler.PotionOrderMap[potionTID].Remove(customer);
        LeaveChair(customer);
    }

    [Server]
    public void OnServedSuccess(Customer customer,uint pickupTableViewID) // 손님이 판매대에 도착하면 호출 
    {
        if(PhaseManager.Instance.CurrentPhase.PhaseType == EPhaseType.ServingPhase)
        {
            SalesManager.Instance.RequestSell(customer.RequestedPotionTID);
        }
        Debug.Log($"Potion served successfully");
        customer.HandlePotion();
        ReputationManager.Instance.AddReputation();
        NetworkServer.spawned[pickupTableViewID].GetComponent<Furniture>().TryCustomerPickup();
        _lineHandler.PutOutCustomer(customer); // 손님을 나가게 하기
    }

    [Server]
    public void OnLastOrderTime() //영업시간 종료되면 호출
    {
        while (_orderHandler.PotionOrderLine.Count > 0)
        {
            Customer customer = _orderHandler.PotionOrderLine.Dequeue();
            _lineHandler.PutOutCustomer(customer);
            customer.TransitionState(ECustomerStateType.Leaving); // 손님 상태를 나가는 상태로 변경
        }
    }
    [Server]
    public void ReturnCustomer(Customer customer) // 손님이 출구에 도착하면 호출
    {
        customer.ReturnPotion();
        CustomerFactory.Instance.ReturnObject(customer.gameObject); 
        RemainCustomers--;
    }
    [Server]
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

    [Server]
    public void CmdPlaceOnTable(int potionTID, uint pickupTableNetId)
    {
        _orderHandler.PickupTableDict[pickupTableNetId].IsUsing = true;
        _orderHandler.PickupTableDict[pickupTableNetId].HeldItemTID = potionTID;
    }
    [Server]
    public void CmdRemoveOnTable(uint pickupTableNetId)
    {
        _orderHandler.PickupTableDict[pickupTableNetId].IsUsing = false;
        _orderHandler.PickupTableDict[pickupTableNetId].HeldItemTID = 0;
        _orderHandler.PickupTableDict[pickupTableNetId].UsingCustomer = null; 
    }
    [Server]
    private void SitOnChair(uint chairNetId,Customer customer)
    {
        if(_orderHandler.LuxuryChairDict.ContainsKey(chairNetId))
        {
            _orderHandler.LuxuryChairDict[chairNetId].IsUsing = true;
            _orderHandler.LuxuryChairDict[chairNetId].UsingCustomer = customer; // 손님과 의자 매핑 저장
        }
        else if (_orderHandler.OldChairDict.ContainsKey(chairNetId))
        {
            _orderHandler.OldChairDict[chairNetId].IsUsing = true;
            _orderHandler.OldChairDict[chairNetId].UsingCustomer = customer; // 손님과 의자 매핑 저장
        }
        GameObject chair = NetworkServer.spawned[chairNetId].gameObject;
        //customer.CustomerMove.MoveTo(chair.transform.position);
        customer.CustomerMove.MoveTo(chair.GetComponent<Furniture>().InputPosition.position);

        // Mirror 임시
        chair.GetComponent<Furniture>().TryCustomerEffect(customer.netId); // 의자 효과 적용
    }
    [Server]
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
