using UnityEngine;
using System.Collections.Generic;
using System;

using Photon.Pun;
public class CustomerManager : MonoBehaviourSingleton<CustomerManager>
{
    private Dictionary<int, LinkedList<GameObject>> _potionOrderMap; // 주문표
    private Queue<GameObject> _potionOrderLine; // 손님이 줄을 서는 대기열
    private PhotonView _photonView;

    [SerializeField]
    private int _lostCustomerCount;
    public int LostCustomerCount { get => _lostCustomerCount; set => _lostCustomerCount = value; }

    private const int MAX_CUSTOMER_LOST = 5;
    private float _inviteTimer = 0f; // 손님 초대 타이머
    [SerializeField]
    private float _inviteCoolTime;
    public float InviteCoolTime{ get => _inviteCoolTime; set => _inviteCoolTime = value; }
    private int _remainCustomers;
    public int RemainCustomers { get => _remainCustomers; set => _remainCustomers = value; }


    public event Action<GameObject> OnCustomerLost; // 인내심 바닥날 때 호출
    public event Action<int> OnPotionServed; // 포션을 내놓았을 때
    public event Action<GameObject> OnCustomerBuyed; // 손님이 포션을 구매했을 때 호출
    public event Action OnCustomerIn; // 플레이어가 접수를 받았을 때 호출
    public event Action OnCustomerOut; // 손님이 건물 밖으로 나갔을 때 호출

    private void Start()
    {
        _potionOrderMap = new Dictionary<int, LinkedList<GameObject>>();
        _potionOrderLine = new Queue<GameObject>();
        //TODO : CustomerPool초기화

        _lostCustomerCount = 0;
        PhaseManager.Instance.PhaseDictionary[EPhaseType.ServingPhase].OnPhaseEntered += SetLists;
    }
    public void SetLists()
    {
        //TODO : 풀 초기화
        _potionOrderMap.Clear();
        _potionOrderLine.Clear();
        _inviteTimer = _inviteCoolTime;
        _remainCustomers = 0;
    }

    public async void InviteCustomer(float deltaTime)
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
        GameObject customer = await CustomerPoolManager.Instance.GetObjectAsync(ENPCType.Customer);
        //TODO : NPC 시스템과 연동해 손님이 접수대에 오게 만들기
        _potionOrderLine.Enqueue(customer);
        RemainCustomers++;
        OnCustomerIn?.Invoke();
    }
    public void OnArrivedLine(GameObject customer)
    {
        if (!PhotonNetwork.IsMasterClient)
        {
            return;
        }
        if (customer == null)
        {
            return;
        }
        
        PhotonView customerView = _potionOrderLine.Peek().GetComponent<PhotonView>();
        _photonView.RPC(nameof(RPC_EnableCustomerInteraction), RpcTarget.All, customerView.ViewID);
    }
    [PunRPC]
    private void RPC_EnableCustomerInteraction(int viewID)
    {
        GameObject customer = PhotonView.Find(viewID)?.gameObject;
        if (customer == null) return;

        Customer head = customer.GetComponent<Customer>();
        head.SetCanInteract(true);
    }

    public void RegisterOrder() // 손님이 주문을 요청할 때 호출
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
        if (_potionOrderLine.Count == 0)
        {
            return;
        }
        GameObject customer = _potionOrderLine.Dequeue();
        int potionTID = customer.GetComponent<Customer>().RequestedPotionTID;

        if (!_potionOrderMap.ContainsKey(potionTID))
        {
            _potionOrderMap[potionTID] = new LinkedList<GameObject>();
        }
        _potionOrderMap[potionTID].AddLast(customer);

        // 연출 동기화
        PhotonView pv = customer.GetComponent<PhotonView>();
        _photonView.RPC(nameof(RPC_OnOrderRegistered), RpcTarget.Others, pv.ViewID, potionTID);
    }

    [PunRPC]
    private void RPC_OnOrderRegistered(int viewID, int potionTID)
    {
        GameObject customer = PhotonView.Find(viewID).gameObject;
        customer.GetComponent<Customer>().SetCanInteract(true);
        //TODO : NPC Hall로 돌려보내기
    }

    public void LostCustomer(GameObject customer) // 인내심이 바닥나면 호출
    {
        if(PhotonNetwork.IsMasterClient == false)
        {
            return;
        }
        OnCustomerLost?.Invoke(customer);
        if (_potionOrderLine != null && _potionOrderLine.Contains(customer))
        {
            _potionOrderLine.Dequeue(); // 손님이 줄에 있다면 줄에서 제거
        }
        else
        {
            _potionOrderMap[customer.GetComponent<Customer>().RequestedPotionTID].RemoveFirst();// 손님이 홀에 있다면 포션 큐에서 제거
        }
        _photonView.RPC(nameof(RPC_PutOutCustomer), RpcTarget.All, customer.GetComponent<PhotonView>().ViewID);
        _lostCustomerCount++;
        if(_lostCustomerCount >= MAX_CUSTOMER_LOST)
        {
            PhaseManager.Instance.TransitionPhase(EPhaseType.EndingPhase);
        }
    }


    public void ServePotion(int potionTID) // 판매대에 올려놓으면 호출
    {
        if (PhotonNetwork.IsMasterClient == false)
        {
            return;
        }
        if (!_potionOrderMap.ContainsKey(potionTID) || _potionOrderMap[potionTID].Count == 0)
        {
            Debug.Log($"No customers in hall for potion TID: {potionTID}");
            return; // 해당 TID의 손님이 없으면 실패
        }
        OnPotionServed?.Invoke(potionTID);
        //TODO : 손님을 판매대로 이동시키기
        //TODO : 가져가기 전까지 포션 상호작용 불가로 만들기
    }

    public void ServedSuccess(int potionTID) // 손님이 판매대에 도착하면 호출 
    {
        if (PhotonNetwork.IsMasterClient == false)
        {
            return;
        }
        //TODO : 구매 성공, Currency 증가
        OnCustomerBuyed?.Invoke(_potionOrderMap[potionTID].First.Value);
        GameObject customer = _potionOrderMap[potionTID].First.Value;
        _potionOrderMap[potionTID].RemoveFirst(); // 손님 제거
        _photonView.RPC(nameof(RPC_PutOutCustomer), RpcTarget.All, customer.GetComponent<PhotonView>().ViewID);
    }

    public void ReturnAllCustomerFromLine() //영업시간 종료되면 호출
    {
        if (PhotonNetwork.IsMasterClient == false)
        {
            return;
        }
        while (_potionOrderLine != null && _potionOrderLine.Count > 0)
        {
            GameObject customer = _potionOrderLine.Dequeue();
            _photonView.RPC(nameof(RPC_PutOutCustomer), RpcTarget.All, customer.GetComponent<PhotonView>().ViewID);
        }
    }

    [PunRPC]
    public void RPC_PutOutCustomer(int viewID) // (마스터만 호출)
    {
        GameObject customer = PhotonView.Find(viewID)?.gameObject;
        //TODO : 손님 오브젝트를 건물 밖으로 내보내기
        // 건물 밖에 나가면 NPC가 직접 ReturnCustomer(this.gameObject);
    }

    public void ReturnCustomer(GameObject customer)
    {
        if(PhotonNetwork.IsMasterClient == false)
        {
            return;
        }
        CustomerPoolManager.Instance.ReturnObject(customer,ENPCType.Customer);
        RemainCustomers--;
    }
}
