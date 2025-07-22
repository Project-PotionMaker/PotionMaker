using UnityEngine;
using System;

public class ServingPhase : BasePhase
{
    private const float INIT_TIMER = 100f;
    private float _currentTime;
    public float CurrentTime { get => _currentTime; set => _currentTime = value; }

    private float _currentTimeRate;
    private bool _timesUp = false;
    public event Action<float> OnTimerRunning;
    public ServingPhase()
    {
        _phaseType = EPhaseType.ServingPhase;
    }
    public override void EnterPhase()
    {
        _currentTime = INIT_TIMER;
        _timesUp = false;

        // 임시 코드
        CustomerManager.Instance.CounterLocation = GridManager.Instance.Casher.transform;
        CustomerManager.Instance.ServingCounter = GridManager.Instance.PickUpTableList[0].transform;
        base.EnterPhase();
    }

    public override void Update(float deltaTime)
    {
        base.Update(deltaTime);
        _currentTime = Mathf.Max(0,_currentTime-deltaTime);
        //TODO : HUD 상단의 타이머와 연동
        if (_currentTime <= 0)
        {
            if(_timesUp == false)
            {
                Debug.Log("타임업! 손님들을 모두 반환합니다.");
                _timesUp = true;
                CustomerManager.Instance.OnLastOrderTime(); // 대기열에 있는 손님들을 모두 반환
            }
            if (CustomerManager.Instance.RemainCustomers == 0) 
            {
                PhaseManager.Instance.TransitionPhase(EPhaseType.EndingPhase);
                return;
            }
        }
        else
        {
            _currentTimeRate = _currentTime / INIT_TIMER; // 타이머 비율 계산
            OnTimerRunning?.Invoke(_currentTimeRate); // 타이머가 작동 중일 때 호출
            CustomerManager.Instance.InviteCustomer(deltaTime); // 손님 초대
        }

    }

    public override void ExitPhase()
    {
        base.ExitPhase();
    }
}
