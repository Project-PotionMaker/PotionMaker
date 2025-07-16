using UnityEngine;
using Photon.Pun;
using UnityEngine.AI;
using System.Collections.Generic;

public class Customer : MonoBehaviour
{
    private Dictionary<ECustomerStateType, CustomerBaseState> _stateDictionary;
    public Dictionary<ECustomerStateType, CustomerBaseState> StateDictionary { get => _stateDictionary; set => _stateDictionary = value; } // 손님의 상태 딕셔너리
    private CustomerBaseState _currentState;
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
    private float _endurancLoseSpeed = 1f;
    private int _priorityOffset;
    public int PriorityOffset { get => _priorityOffset; set => _priorityOffset = value; } // 우선순위 편향

    private void Awake()
    {
        _photonView = GetComponent<PhotonView>();
        _agent = GetComponent<NavMeshAgent>();
        _stateDictionary = new Dictionary<ECustomerStateType, CustomerBaseState>
        {
            { ECustomerStateType.Moving, new CustomerMovingState(this) },
            { ECustomerStateType.AtLine, new CustomerAtLineState(this) },
            { ECustomerStateType.AtHall, new CustomerAtHallState(this) },
            { ECustomerStateType.AtCounter, new CustomerAtCounterState(this) },
            { ECustomerStateType.Out, new CustomerOutState(this) }
        };
    }
    private void OnEnable()
    {
        _lastTarget = Vector3.zero;
        //_requestedPotionTID = RandomPotion();
    }

    private void Update()
    {
        Waiting();
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
        else if (target == CustomerManager.Instance.LineLocation.position)
        {
            _endureanceLosing = true;
            _enduranceGauge = LINE_ENDURANCE; // 줄에 도착하면 인내심 게이지 초기화
        }

        _lastTarget = target;
        Debug.Log("Customer moved to: " + target);
    }
    public void TransitionState(ECustomerStateType nextPhase)
    {
        if (PhotonNetwork.IsMasterClient == false)
        {
            return;
        }
        _photonView.RPC(nameof(RPC_TransitionState), RpcTarget.All, nextPhase);
    }
    [PunRPC]
    public void RPC_TransitionState(ECustomerStateType nextState)
    {
        _currentState?.ExitState();
        _currentState = _stateDictionary[nextState];
        _currentState.EnterState();
    }
}
