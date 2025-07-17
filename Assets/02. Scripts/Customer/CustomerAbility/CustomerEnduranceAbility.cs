using UnityEngine;

public class CustomerEnduranceAbility : MonoBehaviour
{
    Customer _owner; // Customer 컴포넌트
    private const float LINE_ENDURANCE = 10f;
    private const float HALL_ENDURANCE = 10f;
    private float _currentEndurance; // 현재 인내심
    public float CurrentEndurance
    {
        get => _currentEndurance;
        set
        {
            _currentEndurance = value;
            if (_currentEndurance <= 0f)
            {
                _owner.SetCurrentState(ECustomerStateType.Leaving); // 인내심이 0 이하가 되면 상태를 Leaving으로 변경
            }
        }
    }
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
        LosingEndurance(); // 인내심 감소
        if(_currentEndurance <= 0f)
        {
            CustomerManager.Instance.LostCustomer(_owner);
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
    }
}
