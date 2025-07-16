using UnityEngine;
using Photon.Pun;
using Unity.VisualScripting;

public class Customer : MonoBehaviour
{
    private int _requestedPotionTID = 0;
    public int RequestedPotionTID { get => _requestedPotionTID; set=> _requestedPotionTID = value; } // 요청한 포션 ID

    private PhotonView _photonView;

    private Vector3 _currentTarget;
    private Vector3 _lastTarget;

    private float _enduranceGauge;
    public float EnduranceGauge { get => _enduranceGauge; set => _enduranceGauge = value; } // 인내심 게이지
    private const float HALL_ENDURANCE = 30f;
    private const float LINE_ENDURANCE = 30f;
    private bool _endureanceLosing = false; // 인내심 감소 중인지 여부

    private float _endurancLoseSpeed = 1f;

    private void Awake()
    {
        _photonView = GetComponent<PhotonView>();
    }
    private void OnEnable()
    {
        _lastTarget = Vector3.zero;
        //_requestedPotionTID = RandomPotion();
    }

    private void Update()
    {
        Waiting();
        Move();
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
        //TODO : target의 Transform.position으로 천천히 이동
        // 정확히 도착 안 해도 근처 가면 OnArrived() 호출되게 하기

        _currentTarget = target; // 현재 목표 위치 업데이트
        _lastTarget = target;
        Debug.Log("Customer moved to: " + target);
    }

    private void Move()
    {
        if(!PhotonNetwork.IsMasterClient)
        {
            return; // 마스터 클라이언트만 이동 가능
        }
        if (_currentTarget == Vector3.zero)
        {
            return; // 목표 위치가 설정되지 않은 경우 이동하지 않음
        }
        transform.position = Vector3.MoveTowards(transform.position, _currentTarget, Time.deltaTime * 2f); // 2f는 이동 속도
        if(Vector3.Distance(transform.position, _currentTarget) < 0.1f)
        {
            _currentTarget = Vector3.zero; // 이동 완료 후 목표 위치 초기화
            OnArrived(); // 목표 위치에 도착했을 때 호출
        }
    }

    public void OnArrived()
    {
        //TODO : 이동이 끝났을 때 호출
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
