using UnityEngine;
using Photon.Pun;
using Unity.VisualScripting;
using UnityEngine.AI;

public class Customer : MonoBehaviour
{
    private int _requestedPotionTID = 0;
    public int RequestedPotionTID { get => _requestedPotionTID; set=> _requestedPotionTID = value; } // 요청한 포션 ID

    private PhotonView _photonView;
    private NavMeshAgent _agent;
    public NavMeshAgent Agent { get => _agent; set => _agent = value; } // NavMeshAgent 컴포넌트

    private Vector3 _lastTarget;

    private float _enduranceGauge;
    public float EnduranceGauge { get => _enduranceGauge; set => _enduranceGauge = value; } // 인내심 게이지
    private const float HALL_ENDURANCE = 30f;
    private const float LINE_ENDURANCE = 30f;
    private bool _endureanceLosing = false; // 인내심 감소 중인지 여부
    private bool _hasArrived = true; // 도착 여부
    private float _endurancLoseSpeed = 1f;
    private int _priorityOffset;
    public int PriorityOffset { get => _priorityOffset; set => _priorityOffset = value; } // 우선순위 편향

    private void Awake()
    {
        _photonView = GetComponent<PhotonView>();
        _agent = GetComponent<NavMeshAgent>();
    }
    private void OnEnable()
    {
        _lastTarget = Vector3.zero;
        //_requestedPotionTID = RandomPotion();
    }

    private void Update()
    {
        Waiting();
        if(_hasArrived == false)
        {
            ArriveCheck();
        }
    }

    private void Waiting()
    {
        //TODO : if 홀에 있다면
        //TODO : 인내심 타이머 감소
        //TODO : 정해진 구역 내에서 랜덤하게 움직이기
        //TODO : 빈 의자가 있으면 가서 앉기
        //TODO : if 줄에 있다면
        //TODO : 인내심만 줄고 아무것도 안함
    }

    public void MoveTo(Vector3 target)
    {
        if(PhotonNetwork.IsMasterClient == false)
        {
            return; // 마스터 클라이언트만 이동 가능
        }
        if (target == CustomerManager.Instance.HallEntry.position)
        {
            _endureanceLosing = true;
            _enduranceGauge = HALL_ENDURANCE; // 홀에 도착하면 인내심 게이지 초기화
        }
        else if (target == CustomerManager.Instance.CounterLocation.position)
        {
            _endureanceLosing = true;
            _enduranceGauge = LINE_ENDURANCE; // 줄에 도착하면 인내심 게이지 초기화
        }

        _hasArrived = false; // 이동 시작 시 도착 여부 초기화
        _agent.SetDestination(target); // NavMeshAgent를 사용하여 이동
        _agent.isStopped = false; // 이동을 시작
        SetPriority(target); // 우선순위 설정
        _lastTarget = target;
        Debug.Log("Customer moved to: " + target);
    }

    private void SetPriority(Vector3 target)
    {
        //TODO : Layout에서 우선순위 설정 정보 가져오기
        if (_lastTarget == CustomerManager.Instance.CounterLocation.position)
        {
            _agent.avoidancePriority = 60; // 줄에 서 있는 손님은 우선순위가 낮음
        }
        else if (_lastTarget == CustomerManager.Instance.ServingCounter.position)
        {
            _agent.avoidancePriority = 30; // 포션 제공대에 있는 손님은 우선순위가 높음
        }
        else if (_lastTarget == CustomerManager.Instance.ExitDoor.position)
        {
            _agent.avoidancePriority = 0; // 나가는 문에 있는 손님은 우선순위가 가장 높음
        }
        _agent.avoidancePriority += _priorityOffset; // 우선순위 편향 적용
    }

    private void ArriveCheck()
    {
        if(!PhotonNetwork.IsMasterClient)
        {
            return; // 마스터 클라이언트만 이동 가능
        }
        if(!_agent.pathPending && _agent.remainingDistance<1f)
        {
            if(_lastTarget == CustomerManager.Instance.ExitDoor.position || _agent.remainingDistance < 0.1f)
            {//출구는 1f로, 나머지는 0.1f로 도착 체크
                _hasArrived = true; // 도착 여부를 true로 설정
                _agent.isStopped = true; // 이동을 멈춤
                _agent.ResetPath(); // 경로를 초기화
                OnArrived(); // 목표 위치에 도착했을 때 호출
            }
        }
    }

    public void OnArrived()
    {
        //TODO : 이동이 끝났을 때 호출
        //TODO : Layout에서 목적지 정보 가져오게 수정
        if (!PhotonNetwork.IsMasterClient)
        {
            return; // 마스터 클라이언트만 호출 가능
        } 
        Debug.Log($"손님이 {_lastTarget}에 도착했습니다.");
        Debug.Log(Equals(_lastTarget, CustomerManager.Instance.CounterLocation.position) ? "손님이 줄에 도착했습니다." :
            Equals(_lastTarget, CustomerManager.Instance.ServingCounter.position) ? "손님이 포션 제공대에 도착했습니다." :
            Equals(_lastTarget, CustomerManager.Instance.ExitDoor.position) ? "손님이 나가는 문에 도착했습니다." : "손님이 이동 완료");
        if (_lastTarget == CustomerManager.Instance.CounterLocation.position)
        {
            CustomerManager.Instance.OnArrivedLine(this); // 손님이 줄에 도착했을 때 호출
        }
        else if (_lastTarget == CustomerManager.Instance.ServingCounter.position) 
        {
            CustomerManager.Instance.OnServedSuccess(_requestedPotionTID);// 손님이 포션 제공대에 도착했을 때 호출
        }
        else if (_lastTarget == CustomerManager.Instance.ExitDoor.position)
        {
            CustomerManager.Instance.ReturnCustomer(this); // 손님이 나가는 문에 도착했을 때 호출
        }

    }
}
