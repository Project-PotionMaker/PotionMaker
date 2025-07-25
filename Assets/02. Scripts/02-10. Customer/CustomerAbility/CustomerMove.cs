using Photon.Pun;
using UnityEngine;
using UnityEngine.AI;
using static UnityEngine.GraphicsBuffer;

public class CustomerMove : MonoBehaviour
{
    private NavMeshAgent _agent;
    public NavMeshAgent Agent { get => _agent; set => _agent = value; } // NavMeshAgent 컴포넌트
    private NavMeshObstacle _obstacle;
    public NavMeshObstacle Obstacle { get => _obstacle; set => _obstacle = value; } // NavMeshObstacle 컴포넌트
    private Rigidbody _rigidbody;

    private Customer _owner;
    private Animator _animator; // 애니메이터 컴포넌트
    public Animator Animator { get => _animator; set => _animator = value; } // 애니메이터 컴포넌트
    private Vector3 _lastTarget;

    private void Awake()
    {
        _agent = GetComponent<NavMeshAgent>();
        _obstacle = GetComponent<NavMeshObstacle>();
        _rigidbody = GetComponent<Rigidbody>();
        _owner = GetComponent<Customer>();
        _animator = GetComponentInChildren<Animator>();

        _agent.enabled = false; 
        _obstacle.enabled = true; 
        _rigidbody.isKinematic = false; 
    }
    private void Update()
    {
        ArriveCheck();
    }

    private void ArriveCheck()
    {
        if(!PhotonNetwork.IsMasterClient)
        {
            return; // 마스터 클라이언트만 도착 여부 확인
        }
        if (IsStayOn())
        {
            OnArrived(); // 도착 시 호출
        }
        else
        {
            StartMoving();
        }
    }

    public void MoveTo(Vector3 target) // 목적지만 바꾸는 함수
    {
        if (PhotonNetwork.IsMasterClient == false)
        {
            return; // 마스터 클라이언트만 이동 가능
        }
        _lastTarget = target; // 마지막 목적지 저장
        SwitchNavmeshToAgent();
        _agent.SetDestination(target);
        Debug.Log("Customer moved to: " + target);
    }

    private void StartMoving()
    {
        if (!PhotonNetwork.IsMasterClient)
        {
            return; // 마스터 클라이언트만 이동 시작
        }
        if (_owner.CurrentState.StateType == ECustomerStateType.Sitting)
        {
            _owner.TransitionState(ECustomerStateType.ReturningChair);
        }
        else if (_owner.CurrentState.StateType == ECustomerStateType.Lining)
        {
            _owner.TransitionState(ECustomerStateType.ReturningLine);
        }
    }
    private void OnArrived()
    {
        if(!PhotonNetwork.IsMasterClient)
        {
            return; 
        }
        if (_owner.CurrentState.StateType == ECustomerStateType.ReturningLine)
        {
            Debug.Log("줄서기 상태로 전환");
            _owner.TransitionState(ECustomerStateType.Lining);
        } else if (_owner.CurrentState.StateType == ECustomerStateType.ReturningChair)
        {
            _owner.TransitionState(ECustomerStateType.Sitting);
        }
        else if (_owner.CurrentState.StateType == ECustomerStateType.PickingUp)
        {
            _owner.TransitionState(ECustomerStateType.Leaving);
        }
        else if (_owner.CurrentState.StateType == ECustomerStateType.Leaving)
        {
            CustomerManager.Instance.ReturnCustomer(_owner); // 삭제
        }
    }
    public bool IsStayOn()
    {
        float distance = Vector3.Distance(transform.position, _lastTarget);
        if (distance < 1f)
        {
            if (_owner.CurrentState.StateType == ECustomerStateType.Leaving || distance <1f) // 출구는 넉넉하게 1 나머지는 0.1
            {
                return true;
            }
        }
        return false;
    }

    public void SwitchNavmeshToAgent()
    {
        _owner.CustomerMove.Obstacle.enabled = false;
        _owner.CustomerMove.Agent.enabled = true;
    }
    public void SwitchNavMeshToObstacle()
    {
        _owner.CustomerMove.Agent.ResetPath();
        _owner.CustomerMove.Agent.enabled = false;
        _owner.CustomerMove.Obstacle.enabled = true;
    }
}
