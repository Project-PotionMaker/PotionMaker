using UnityEngine;
using System.Collections.Generic;
using System;
//using Photon.Pun;
using UnityEditor;
using VInspector;
using Mirror;
public class CustomerManager : MonoBehaviourSingleton<CustomerManager>
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
    private void Start()
    {
        //_photonView = GetComponent<PhotonView>();
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
        //if (!PhotonNetwork.IsMasterClient)
        //{
        //    return;
        //}
        _inviteTimer -= deltaTime;
        if (_inviteTimer > 0)
        {
            return;
        } 
        //TODO : Layout에서 최대 줄 길이 가져와서 적용하기
        _inviteTimer = _inviteCoolTime;
        Debug.Log("손님 초대");
        GameObject customer = CustomerFactory.Instance.Create(ENPCType.Customer,Vector3.zero,Quaternion.identity); // TODO : PoolManager완성 후 수정
        //OnCustomerIn(customer.GetComponent<PhotonView>().ViewID); //TODO : PoolManager완성 후 수정
        //CustomerPool.Instance.GetObjectAsync(0);
        RemainCustomers++;
        _inviteIndex++;
    }

    public void OnCustomerIn(int viewID)
    {
        Debug.Log($"손님 생성: {viewID}");
        //PhotonView photonView = PhotonView.Find(viewID);
        //Customer customer = photonView.GetComponent<Customer>();
        //customer.transform.position = _enterDoor.position; // 손님을 상점 입구에 생성
        //_orderHandler.PotionOrderLine.Enqueue(customer);
        _lineHandler.ReLining();
    }

    public void RegisterOrder() // 플레이어가 접수를 받으면 호출
    {
        //if (PhotonNetwork.IsMasterClient)
        //{
        //    RegisterOrderInternal();
        //}
        //else
        //{
        //    _photonView.RPC(nameof(RPC_RegisterOrder), RpcTarget.MasterClient);
        //}
    }
    //[PunRPC]
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
        int chairViewID = _orderHandler.FindAvailableChair(); // 사용 가능한 의자 찾기
        if (chairViewID == 0)
        {
            Debug.Log("No available chair found for the customer.");
            return; // 사용 가능한 의자가 없으면 주문을 받지 않음
        }
        _canOrdered = false; // 주문을 받은 후에는 다시 주문을 받을 수 없도록 설정
        Customer customer = _orderHandler.PotionOrderLine.Dequeue();
        int potionTID = customer.GetComponent<Customer>().RequestedPotionTID;
        _orderHandler.AddOrder(potionTID, customer);
        customer.TransitionState(ECustomerStateType.Sitting); 
        SitOnChair(chairViewID, customer);
        customer.CustomerEndurance.ResetEndurance(); 
        _lineHandler.ReLining(); // 줄 다시 세우기
        ServePotionOnTakeOrder();
    }

    public void LostCustomer(Customer customer) // 인내심이 바닥나면 호출
    {
        //if(PhotonNetwork.IsMasterClient == false)
        //{
        //    return;
        //}
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
        foreach (KeyValuePair<int, FurnitureUsingStat> pair in _orderHandler.PickupTableDict)
        {
            if (pair.Value.IsUsing == true && pair.Value.UsingCustomer == null)
            {
                ServePotion(pair.Value.HeldItemTID,pair.Key);
            }
        }
    }

    public void ServePotion(int potionTID, int pickupTableViewID)// 판매대에 올려놓으면 호출
    {
        //if (PhotonNetwork.IsMasterClient)
        //{
        //    ServePotionInternal(potionTID,pickupTableViewID); 
        //}
        //else
        //{
        //    _photonView.RPC(nameof(RPC_ServePotion), RpcTarget.MasterClient, potionTID);
        //}
    }
    //[PunRPC]
    public void RPC_ServePotion(int potionTID, int pickupTableViewID)
    {
        ServePotionInternal(potionTID,pickupTableViewID);
    }

    public void ServePotionInternal(int potionTID,int pickupTableViewID)
    {
        //if (PhotonNetwork.IsMasterClient == false)
        //{
        //    return;
        //}
        Customer customer = _orderHandler.FindPicker(potionTID); // 포션을 가져갈 손님 찾기
        if (ReferenceEquals(customer, null)) 
        {
            return;
        }

        PlaceOnTable(potionTID, pickupTableViewID);
        Vector3 position = FindPickupTableByViewID(pickupTableViewID).transform.position; // 판매대 위치 찾기
        customer.TransitionState(ECustomerStateType.PickingUp);
        customer.CustomerMove.MoveTo(position); // 손님을 판매대 위치로 이동
        _orderHandler.PickupTableDict[pickupTableViewID].UsingCustomer = customer; // 손님과 판매대 매핑 저장
        _orderHandler.PotionOrderMap[potionTID].Remove(customer);
        LeaveChair(customer);

    }

    public void OnServedSuccess(Customer customer,int pickupTableViewID) // 손님이 판매대에 도착하면 호출 
    {
        //if (PhotonNetwork.IsMasterClient == false)
        //{
        //    return;
        //}
        if(PhaseManager.Instance.CurrentPhase.PhaseType == EPhaseType.ServingPhase)
        {
            //TODO : 구매 성공, Currency 증가
        }
        Debug.Log($"Potion served successfully");
        RemoveOnTable(pickupTableViewID); // 판매대에서 포션 제거
        GameObject potion = FindPickupTableByViewID(pickupTableViewID).GetComponent<IGridItemHandler>().TryPickUp(customer.connectionToClient); // 판매대 위치에서 포션 오브젝트 가져오기


        potion.transform.SetParent(customer.PotionHandler.transform);
        potion.transform.localPosition = Vector3.zero;
        _lineHandler.PutOutCustomer(customer); // 손님을 나가게 하기
    }

    public void OnLastOrderTime() //영업시간 종료되면 호출
    {
        //if (PhotonNetwork.IsMasterClient == false)
        //{
        //    return;
        //}
        while (_orderHandler.PotionOrderLine.Count > 0)
        {
            Customer customer = _orderHandler.PotionOrderLine.Dequeue();
            _lineHandler.PutOutCustomer(customer);
            customer.TransitionState(ECustomerStateType.Leaving); // 손님 상태를 나가는 상태로 변경
        }
    }

    public void ReturnCustomer(Customer customer) // 손님이 출구에 도착하면 호출
    {
        //if(PhotonNetwork.IsMasterClient == false)
        //{
        //    return;
        //}
        customer.ReturnPotion();
        CustomerFactory.Instance.CmdReturn(customer.gameObject); // TODO : PoolManager완성 후 수정
        //CustomerPool.Instance.ReturnObject(customer.gameObject,ENPCType.Customer);
        RemainCustomers--;
    }
    public void ForceReturn() // 인내심 바닥나서 끝나면 전부 강제로 내보냄, 또는 버그로 큐에 남아있는 손님도 내보냄
    {
        //if (PhotonNetwork.IsMasterClient == false)
        //{
        //    return;
        //}
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

    private GameObject FindPickupTableByViewID(int pickupTableViewID)
    {
        foreach (GameObject pickupTable in GridManager.Instance.PickUpTableList)
        {
            //if (pickupTable.GetComponent<PhotonView>().ViewID == pickupTableViewID)
            //{
            //    return pickupTable;
            //}
        }
        Debug.LogError("Pickup table not found.");
        return null;
    }

    private GameObject FindChairByViewID(int chairViewID)
    {
        foreach (GameObject luxuryChair in GridManager.Instance.LuxuryChairList)
        {
            //if (luxuryChair.GetComponent<PhotonView>().ViewID == chairViewID)
            //{
            //    return luxuryChair;
            //}
        }
        foreach (GameObject oldChair in GridManager.Instance.OldChairList)
        {
            //if (oldChair.GetComponent<PhotonView>().ViewID == chairViewID)
            //{
            //    return oldChair;
            //}
        }
        return null;
    }

    private void PlaceOnTable(int potionTID, int pickupTableViewID)
    {
        _orderHandler.PickupTableDict[pickupTableViewID].IsUsing = true;
        _orderHandler.PickupTableDict[pickupTableViewID].HeldItemTID = potionTID;
    }
    public void RemoveOnTable(int pickupTableViewID)
    {
        _orderHandler.PickupTableDict[pickupTableViewID].IsUsing = false;
        _orderHandler.PickupTableDict[pickupTableViewID].HeldItemTID = 0;
        _orderHandler.PickupTableDict[pickupTableViewID].UsingCustomer = null; 
    }
    private void SitOnChair(int chairViewID,Customer customer)
    {
        if(_orderHandler.LuxuryChairDict.ContainsKey(chairViewID))
        {
            _orderHandler.LuxuryChairDict[chairViewID].IsUsing = true;
            _orderHandler.LuxuryChairDict[chairViewID].UsingCustomer = customer; // 손님과 의자 매핑 저장
        }
        else if (_orderHandler.OldChairDict.ContainsKey(chairViewID))
        {
            _orderHandler.OldChairDict[chairViewID].IsUsing = true;
            _orderHandler.OldChairDict[chairViewID].UsingCustomer = customer; // 손님과 의자 매핑 저장
        }
        GameObject chair = FindChairByViewID(chairViewID);
        customer.CustomerMove.MoveTo(chair.transform.position);

        // Mirror 임시
        chair.GetComponent<Furniture>().TryEffect(customer.netId); // 의자 효과 적용
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
