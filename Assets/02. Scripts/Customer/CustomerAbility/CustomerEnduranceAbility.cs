using Photon.Pun;
using UnityEngine;
using ExitGames.Client.Photon;

public class CustomerEnduranceAbility : MonoBehaviour
{
    Customer _owner; // Customer 컴포넌트
    private const float LINE_ENDURANCE = 10f;
    private const float HALL_ENDURANCE = 10f;
    private float _currentEndurance; // 현재 인내심
    public float CurrentEndurance    {get => _currentEndurance; set => _currentEndurance = value; } // 현재 인내심
    private float _enduranceRate;
    public float EnduranceRate { get => _enduranceRate; set => _enduranceRate = value; } // 인내심 회복 속도

    private float _loseEnduranceSpeed = 1f; // 인내심 감소 속도
    public float LoseEnduranceSpeed { get => _loseEnduranceSpeed; set => _loseEnduranceSpeed = value; } // 인내심 감소 속도

    private void Awake()
    {
        _owner = GetComponent<Customer>();
        _owner.OnStateChanged += ResetEndurance; // 상태 변경 시 인내심 초기화
    }
    private void OnEnable()
    {
        _currentEndurance = LINE_ENDURANCE; // 줄 서는 상태에서 인내심 초기화
        _loseEnduranceSpeed = 1f; // 인내심 감소 속도 초기화
    }

    private void Update()
    {
        if (_owner.CurrentState == ECustomerStateType.Leaving || _owner.CurrentState == ECustomerStateType.PickingUp)
        {
            return;
        }
        if (PhotonNetwork.IsMasterClient)
        {
            LosingEndurance(); // 인내심 감소
            if (_currentEndurance <= 0f && _owner.CurrentState != ECustomerStateType.Leaving)
            {
                CustomerManager.Instance.LostCustomer(_owner);
            }
        }
        else
        {
            TrySyncFromProperties(); // 커스텀 프로퍼티에서 인내심 동기화
        }
    }
    private void ResetEndurance()
    {
        if (_owner.CurrentState == ECustomerStateType.Lining)
        {
            _currentEndurance = LINE_ENDURANCE; // 줄 서는 상태에서 인내심 초기화
        }
        else if (_owner.CurrentState == ECustomerStateType.Waiting)
        {
            _currentEndurance = HALL_ENDURANCE; // 대기실 상태에서 인내심 초기화
        }
    }

    private void LosingEndurance()
    {
        if (_owner.CurrentState == ECustomerStateType.Lining || _owner.CurrentState == ECustomerStateType.Waiting)
        {
            _currentEndurance = Mathf.Max(_currentEndurance -_loseEnduranceSpeed * Time.deltaTime,0); // 인내심 감소
        }

        if(_owner.CurrentState == ECustomerStateType.Lining)
        {
            _enduranceRate = _currentEndurance /LINE_ENDURANCE; // 인내심 비율 계산
        }else if (_owner.CurrentState == ECustomerStateType.Waiting)
        {
            _enduranceRate = _currentEndurance / HALL_ENDURANCE; // 인내심 비율 계산
        }

        SyncToCustomProperties(); // 인내심을 커스텀 프로퍼티에 동기화
    }
    private void SyncToCustomProperties()
    {
        if (!PhotonNetwork.IsMasterClient) return;

        var hash = new Hashtable
        {
            { "EnduranceRate", _enduranceRate }
        };
        _owner.PhotonView.Owner.SetCustomProperties(hash);
    }

    private void TrySyncFromProperties()
    {
        if (_owner.PhotonView.Owner.CustomProperties.TryGetValue("EnduranceRate", out object value))
        {
            _enduranceRate = (float)(double)value;
        }
    }
}
