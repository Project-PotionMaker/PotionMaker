using DG.Tweening;
using UnityEngine;
using UnityEngine.AI;
using Mirror;
using System;
using System.Collections.Generic;
public class CustomerMove : NetworkBehaviour
{
    private NavMeshAgent _agent;
    public NavMeshAgent Agent { get => _agent; set => _agent = value; } // NavMeshAgent 컴포넌트
    private Rigidbody _rigidbody;
    private Collider _collider;

    private Customer _owner;
    private Animator _animator; // 애니메이터 컴포넌트
    private NetworkAnimator _networkAnimator; // 네트워크 애니메이터 컴포넌트
    private Vector3 _lastTarget;

    private bool _hasArrived = true;
    private const float GRID_OFFSET = 0f;

    public event Action<ECustomerEmojiType> OnSuccess;

    private void OnEnable()
    {

        _agent = GetComponent<NavMeshAgent>();
        _collider = GetComponent<Collider>();
        if (!isServer)
        {
            _agent.enabled = false; // 클라이언트에서는 NavMeshAgent 비활성화
            //_collider.enabled = false; // 클라이언트에서는 충돌체 비활성화
        }
        _rigidbody = GetComponent<Rigidbody>();
        _rigidbody.isKinematic = false;
        _owner = GetComponent<Customer>();
        _animator = GetComponentInChildren<Animator>();
        _networkAnimator = GetComponentInChildren<NetworkAnimator>();
    }
    private void Update()
    {
        if (!isServer) 
        {
            return;
        }
        if (_agent.isActiveAndEnabled == true)
        {
            ArriveCheck();
        }
    }
    [Server]
    private void ArriveCheck()
    {
        if (IsStayOn())
        {
            if (_hasArrived == false)
            {
                _hasArrived = true;
                OnArrived(); // 도착 시 호출
            }
        }
        else
        {
            _hasArrived = false;
            StartMoving();
        }
    }
    [Server]
    public void MoveTo(Vector3 target) // 목적지만 바꾸는 함수
    {
        if (!isServer)
        {
            return;
        }
        _agent.enabled = true; 
        if(_owner.CurrentState == ECustomerStateType.PickingUp)
        {
            StandingAction();
        }

        target = new Vector3(target.x+GRID_OFFSET, target.y, target.z+GRID_OFFSET); // Y축은 현재 위치 유지
        _lastTarget = target; // 마지막 목적지 저장
        _agent.SetDestination(target);
    }
    [Server]
    public void MoveTo(List<Vector3> target)
    {
        if(target.Count == 0)
        {
            Debug.LogWarning("Target list is empty. Cannot move to an empty list.");
            return;
        }
        float minDistance = float.MaxValue;
        Vector3? dest = null;
        foreach (Vector3 pos in target)
        {
            Vector3 newPos = new Vector3(pos.x + GRID_OFFSET, 0, pos.z + GRID_OFFSET); // Y축은 현재 위치 유지
            if (NavMesh.SamplePosition(newPos, out NavMeshHit hit, 0.4f, NavMesh.AllAreas))
            {
                float distance = Vector3.Distance(transform.position, hit.position);
                if (minDistance > distance)
                {
                    minDistance = distance;
                    dest = hit.position;
                }
            }
        }
        if (dest == null)
        {
            Debug.LogWarning("No valid destination found in the target list.");
            return;
        }
        MoveTo((Vector3)dest);
    }


    private void StartMoving()
    {
        _animator.SetBool("Move", true);
    }
    private void OnArrived()
    {
        _animator.SetBool("Move", false);
        if (_owner.CurrentState == ECustomerStateType.Lining)
        {
            _networkAnimator.SetTrigger("Stand");
            if (ReferenceEquals(_owner, CustomerManager.Instance.OrderHandler.PotionOrderLine.Peek()))
            {
                CustomerManager.Instance.CanOrdered = true;
            }
        } else if (_owner.CurrentState == ECustomerStateType.Sitting)
        {
            _networkAnimator.SetTrigger("Sit");
            SittingAction(); // 의자에 앉는 동작 실행
        }
        else if (_owner.CurrentState == ECustomerStateType.PickingUp)
        {
            BroadcastEmoji(); // 이모지 브로드캐스트
            CustomerManager.Instance.OnServedSuccess(_owner, _owner.PickupTableId);
        }
        else if (_owner.CurrentState == ECustomerStateType.Leaving)
        {
            CustomerManager.Instance.ReturnCustomer(_owner); // 삭제
        }
    }
    public bool IsStayOn()
    {
        float distance = Vector3.Distance(transform.position, _lastTarget);
        if (_owner.CurrentState == ECustomerStateType.Sitting)
        {
            if(distance < _agent.stoppingDistance)
            {
                return true;
            }
        }
        else if (distance < 2 * _agent.stoppingDistance)
        {
            return true;
        }
        return false;
    }
    private void SittingAction()
    {
        _agent.enabled = false;
        //_collider.enabled = false; // 충돌체 비활성화

        Sequence sitSeq = DOTween.Sequence();
        Vector3 sit = new Vector3(_owner.ChairPosition.x, _owner.ChairPosition.y, _owner.ChairPosition.z);
        sitSeq.Append(transform.DOMove(sit, 1f).SetEase(Ease.OutSine));
        sitSeq.Join(transform.DORotate(new Vector3(0,_owner.ChairRotate+90,0), 1f).SetEase(Ease.InOutSine));

    }
    private void StandingAction()
    {
        _agent.enabled = true;
        _collider.enabled = true; // 충돌체 활성화
    }

    [ClientRpc]
    private void BroadcastEmoji()
    {
        OnSuccess?.Invoke(ECustomerEmojiType.Happy);
    }
}
