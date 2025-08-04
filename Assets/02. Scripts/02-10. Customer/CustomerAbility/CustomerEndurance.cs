using System;
using UnityEngine;
using Mirror;
public class CustomerEndurance : NetworkBehaviour
{
    Customer _owner; // Customer 컴포넌트
    private const float LINE_ENDURANCE = 10f;
    private const float HALL_ENDURANCE = 10f;
    private float _currentEndurance; // 현재 인내심
    public float CurrentEndurance    {get => _currentEndurance; set => _currentEndurance = value; } // 현재 인내심
    [SyncVar (hook = nameof(SyncEndurence))]
    private float _enduranceRate;
    public float EnduranceRate { get => _enduranceRate; set => _enduranceRate = value; }

    private float _loseEnduranceSpeed = 1f; // 인내심 감소 속도
    public float LoseEnduranceSpeed { get => _loseEnduranceSpeed; set => _loseEnduranceSpeed = value; } // 인내심 감소 속도

    public event Action OnEnduranceChanged; // 인내심 변경 이벤트

    private void Awake()
    {
        _owner = GetComponent<Customer>();
    }
    private void OnEnable()
    {
        _currentEndurance = LINE_ENDURANCE; // 줄 서는 상태에서 인내심 초기화
        _loseEnduranceSpeed = 1f; // 인내심 감소 속도 초기화
    }

    private void Update()
    {
        if (!isServer)
        {
            return;
        }
        if (_owner.CurrentState == ECustomerStateType.Leaving || _owner.CurrentState == ECustomerStateType.PickingUp)
        {
            return;
        }
        LosingEndurance(); // 인내심 감소
        if (_currentEndurance <= 0f && _owner.CurrentState != ECustomerStateType.Leaving)
        {
            CustomerManager.Instance.LostCustomer(_owner);
        }
        
    }
    public void ResetEndurance()
    {
        if (!isServer)
        {
            return;
        }
        _currentEndurance = HALL_ENDURANCE;   
    }
    [Server]
    private void LosingEndurance()
    {
        if (_owner.CurrentState != ECustomerStateType.Leaving && _owner.CurrentState != ECustomerStateType.PickingUp)
        {
            _currentEndurance = Mathf.Max(_currentEndurance -_loseEnduranceSpeed * Time.deltaTime,0); // 인내심 감소
        }

        if(_owner.CurrentState == ECustomerStateType.Lining)
        {
            _enduranceRate = _currentEndurance /LINE_ENDURANCE; // 인내심 비율 계산
        }else if (_owner.CurrentState == ECustomerStateType.Sitting)
        {
            _enduranceRate = _currentEndurance / HALL_ENDURANCE; // 인내심 비율 계산
        }
    }

    private void SyncEndurence(float OldValue, float NewValue)
    {
        OnEnduranceChanged?.Invoke(); // 인내심 변경 이벤트 호출
    }
}
