using Photon.Pun;
using Steamworks;
using UnityEngine;
using UnityEngine.AI;
using static UnityEngine.GraphicsBuffer;

public class CustomerMove : MonoBehaviour
{
    private NavMeshAgent _agent;
    public NavMeshAgent Agent { get => _agent; set => _agent = value; } // NavMeshAgent 컴포넌트
    private Rigidbody _rigidbody;

    private Customer _owner;
    private Animator _animator; // 애니메이터 컴포넌트
    public Animator Animator { get => _animator; set => _animator = value; } // 애니메이터 컴포넌트
    private Vector3 _lastTarget;

    private bool _hasArrived = true;

    private void Awake()
    {
        _agent = GetComponent<NavMeshAgent>();
        _rigidbody = GetComponent<Rigidbody>();
        _owner = GetComponent<Customer>();
        _animator = GetComponentInChildren<Animator>();

        _agent.enabled = true; 
        _rigidbody.isKinematic = false; 
    }
    private void Update()
    {
        if (_agent.isActiveAndEnabled == true)
        {
            ArriveCheck();
        }
        else
        {
            LocalMoveing();
        }
    }

    private void ArriveCheck()
    {
        if(!PhotonNetwork.IsMasterClient)
        {
            return; // 마스터 클라이언트만 도착 여부 확인
        }
        if (IsStayOn())
        {
            if (_hasArrived == false)
            {
                OnArrived(); // 도착 시 호출
                _hasArrived=true;
            }
        }
        else
        {
            _hasArrived = false;
            StartMoving();
        }
    }

    public void MoveTo(Vector3 target) // 목적지만 바꾸는 함수
    {
        if (PhotonNetwork.IsMasterClient == false)
        {
            return; // 마스터 클라이언트만 이동 가능
        }
        _agent.enabled = true; 
        target = new Vector3(target.x+0.5f, target.y, target.z+0.5f); // Y축은 현재 위치 유지
        _lastTarget = target; // 마지막 목적지 저장
        _agent.SetDestination(target);
        Debug.Log("Customer moved to: " + target);
    }

    private void StartMoving()
    {
        if (!PhotonNetwork.IsMasterClient)
        {
            return; // 마스터 클라이언트만 이동 시작
        }
        _animator.SetBool("Move", true);
    }
    private void OnArrived()
    {
        if(!PhotonNetwork.IsMasterClient)
        {
            return; 
        }
        _animator.SetBool("Move", false);
        if (_owner.CurrentState == ECustomerStateType.Lining)
        {
            _animator.SetTrigger("Stand");
            if (ReferenceEquals(_owner, CustomerManager.Instance.OrderHandler.PotionOrderLine.Peek()))
            {
                CustomerManager.Instance.CanOrdered = true;
            }
        } else if (_owner.CurrentState == ECustomerStateType.Sitting)
        {
            _animator.SetTrigger("Sit");
            SittingAction(); // 의자에 앉는 동작 실행
        }
        else if (_owner.CurrentState == ECustomerStateType.PickingUp)
        {
            CustomerManager.Instance.OnServedSuccess(_owner, _owner.RequestedPotionTID);
        }
        else if (_owner.CurrentState == ECustomerStateType.Leaving)
        {
            CustomerManager.Instance.ReturnCustomer(_owner); // 삭제
        }
    }
    public bool IsStayOn()
    {
        float distance = Vector3.Distance(transform.position, _lastTarget);
        if (distance < 1f)
        {
            return true;    
        }
        return false;
    }
    private void SittingAction()
    {
        _agent.enabled = false;
    }
    private void LocalMoveing()
    {

    }
}
