using Photon.Pun;
using UnityEngine;
using UnityEngine.AI;
public class CustomerMovingState : CustomerBaseState
{
    ECustomerStateType _nextState;
    private NavMeshAgent _agent; // NavMeshAgent 컴포넌트

    public CustomerMovingState(Customer owner) : base(owner)
    {
        _stateType = ECustomerStateType.Moving;
        _agent = owner.Agent;
    }
    public void EnterState(ECustomerStateType nextState)
    {
        _nextState = nextState;
        _agent.isStopped = false;
        _agent.SetDestination(CustomerManager.Instance.LocationDictionary[nextState]);
        SetPriority();
    }
    public override void Update()
    {
        ArriveCheck();
    }
    public override void ExitState()
    {
        _agent.isStopped = true; // 이동을 멈춤
        _agent.ResetPath(); // 경로를 초기화
    }
    private void ArriveCheck()
    {
        if (_agent.pathPending && _agent.remainingDistance < 1f)
        {
            if (_nextState == ECustomerStateType.Out || _agent.remainingDistance < 0.1f)
            {//출구는 1f로, 나머지는 0.1f로 도착 체크
                _agent.isStopped = true; // 이동을 멈춤
                _agent.ResetPath(); // 경로를 초기화
                OnArrived(); // 목표 위치에 도착했을 때 호출
            }
        }
    }
    private void SetPriority()
    {
        //TODO : Layout에서 우선순위 설정 정보 가져오기
        if (_nextState == ECustomerStateType.AtLine)
        {
            _owner.Agent.avoidancePriority = 60;
        }
        if (_nextState == ECustomerStateType.AtHall)
        {
            _owner.Agent.avoidancePriority = 30; 
        }
        if (_nextState == ECustomerStateType.Out)
        {
            _owner.Agent.avoidancePriority = 0; 
        }
        _owner.Agent.avoidancePriority += _owner.PriorityOffset; // 우선순위 편향 적용
    }
    public void OnArrived()
    {
        Debug.Log(Equals(_nextState, ECustomerStateType.Out) ? "손님이 나가는 문에 도착했습니다." :
            Equals(_nextState, ECustomerStateType.AtLine) ? "손님이 줄에 도착했습니다." :
            Equals(_nextState, ECustomerStateType.AtHall) ? "손님이 홀에 도착했습니다." :
            Equals(_nextState, ECustomerStateType.AtCounter) ? "손님이 포션 제공대에 도착했습니다." : "손님이 이동 완료");

        _owner.TransitionState(_nextState); // 상태 전환 호출
    }
}
