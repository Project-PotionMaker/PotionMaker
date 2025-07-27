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

    private Vector3 _lastTarget;

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
            return; 
        }
        if (_agent.remainingDistance <= 1f && !_agent.pathPending)
        {
            if(_owner.CurrentState == ECustomerStateType.Leaving || _agent.remainingDistance <= 0.1f)
            {
                _hasArrived = true; 
                _agent.isStopped = true;
                OnArrived(); 
            }
        }
    }

    public void MoveTo(Vector3 target)
    {
        if (PhotonNetwork.IsMasterClient == false)
        {
            return; 
        }

        _hasArrived = false; 
        _agent.SetDestination(target); 
        _agent.isStopped = false;
        SetPriority();
        _lastTarget = target;
    }
    private void SetPriority()
    {
        //if (_owner.CurrentState == ECustomerStateType.Waiting)
        //{
        //    _agent.avoidancePriority = 100;
        //    return;
        //}
        if (_owner.CurrentState == ECustomerStateType.Lining)
        {
            _agent.avoidancePriority = 60;
        }
        else if (_owner.CurrentState == ECustomerStateType.PickingUp)
        {
            _agent.avoidancePriority = 30;
        }
        else if (_owner.CurrentState == ECustomerStateType.Leaving)
        {
            _agent.avoidancePriority = 0;
        }
        _agent.avoidancePriority += _priorityOffset;
    }

    public void OnArrived()
    {
        if(!PhotonNetwork.IsMasterClient)
        {
            return; 
        }
        if (!PhotonNetwork.IsMasterClient)
        {
            return; // 마스터 클라이언트만 호출 가능
        }
        if (_owner.CurrentState == ECustomerStateType.Lining)
        {
            CustomerManager.Instance.OnArrivedLine(_owner); 
        }
        else if (_owner.CurrentState == ECustomerStateType.PickingUp)
        {
            int pickupTableViewID = GridManager.Instance.GetObjectOnGrid(_lastTarget).GetComponent<Furniture>().PhotonView.ViewID;
            CustomerManager.Instance.OnServedSuccess(_owner,pickupTableViewID); 
        }
        else if (_owner.CurrentState == ECustomerStateType.Leaving)
        {
            CustomerManager.Instance.ReturnCustomer(_owner);
        };

    }
}
