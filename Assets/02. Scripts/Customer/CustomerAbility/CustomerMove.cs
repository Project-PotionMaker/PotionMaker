using Photon.Pun;
using UnityEngine;
using UnityEngine.AI;

public class CustomerMove : MonoBehaviour
{
    private NavMeshAgent _agent;
    public NavMeshAgent Agent { get => _agent; set => _agent = value; } // NavMeshAgent 컴포넌트

    private Customer _owner;
    private bool _hasArrived = true; // 도착 여부
    private int _priorityOffset;
    public int PriorityOffset { get => _priorityOffset; set => _priorityOffset = value; } // 우선순위 편향

    private void Awake()
    {
        _agent = GetComponent<NavMeshAgent>();
        _owner = GetComponent<Customer>();
    }
    private void Update()
    {
        if (_hasArrived == false)
        {
            ArriveCheck();
        }
    }

    private void ArriveCheck()
    {
        if(!PhotonNetwork.IsMasterClient)
        {
            return; // 마스터 클라이언트만 도착 여부 확인
        }
        if (_agent.remainingDistance <= 1f && !_agent.pathPending)
        {
            if(_owner.CurrentState == ECustomerStateType.Leaving || _agent.remainingDistance <= 0.1f)
            {
                _hasArrived = true; // 도착했음을 표시
                _agent.isStopped = true; // 이동 중지
                Debug.Log("Customer has arrived at the destination: " + _owner.CurrentState);
                OnArrived(); // 도착 시 호출
            }
        }
    }

    public void MoveTo(Vector3 target)
    {
        if (PhotonNetwork.IsMasterClient == false)
        {
            return; // 마스터 클라이언트만 이동 가능
        }

        _hasArrived = false; // 이동 시작 시 도착 여부 초기화
        _agent.SetDestination(target); // NavMeshAgent를 사용하여 이동
        _agent.isStopped = false; // 이동을 시작
        SetPriority(target); // 우선순위 설정
        Debug.Log("Customer moved to: " + target);
    }
    private void SetPriority(Vector3 target)
    {
        if (_owner.CurrentState == ECustomerStateType.Waiting)
        {
            _agent.avoidancePriority = 100; // 대기 중인 손님은 우선순위가 가장 높음
            return; // 대기 중인 손님은 우선순위만 설정하고 종료
        }
        if (_owner.CurrentState == ECustomerStateType.Lining)
        {
            _agent.avoidancePriority = 60; // 줄에 서 있는 손님은 우선순위가 낮음
        }
        else if (_owner.CurrentState == ECustomerStateType.PickingUp)
        {
            _agent.avoidancePriority = 30; // 포션 제공대에 있는 손님은 우선순위가 높음
        }
        else if (_owner.CurrentState == ECustomerStateType.Leaving)
        {
            _agent.avoidancePriority = 0; // 나가는 문에 있는 손님은 우선순위가 가장 높음
        }
        _agent.avoidancePriority += _priorityOffset; // 우선순위 편향 적용
    }

    public void OnArrived()
    {
        if(!PhotonNetwork.IsMasterClient)
        {
            return; // 마스터 클라이언트만 도착 시 호출 가능
        }
        //TODO : 이동이 끝났을 때 호출
        //TODO : Layout에서 목적지 정보 가져오게 수정
        if (!PhotonNetwork.IsMasterClient)
        {
            return; // 마스터 클라이언트만 호출 가능
        }
        if (_owner.CurrentState == ECustomerStateType.Lining)
        {
            CustomerManager.Instance.OnArrivedLine(_owner); // 손님이 줄에 도착했을 때 호출
        }
        else if (_owner.CurrentState == ECustomerStateType.PickingUp)
        {
            CustomerManager.Instance.OnServedSuccess(_owner.RequestedPotionTID); // 손님이 포션 제공대에 도착했을 때 호출
        }
        else if (_owner.CurrentState == ECustomerStateType.Leaving)
        {
            CustomerManager.Instance.ReturnCustomer(_owner); // 손님이 나가는 문에 도착했을 때 호출
        };

    }
}
