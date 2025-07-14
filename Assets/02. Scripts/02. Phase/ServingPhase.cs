using UnityEngine;
using System;

public class ServingPhase : BasePhase
{
    private const float INIT_TIMER = 20f;
    [SerializeField]
    private float _currentTime;
    public float CurrentTime { get => _currentTime; set => _currentTime = value; }

    private bool _timesUp = false;

    public ServingPhase()
    {
        _phaseType = EPhaseType.ServingPhase;
    }
    public override void EnterPhase()
    {
        _currentTime = INIT_TIMER;
        _timesUp = false;
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
                _timesUp = true;
                CustomerManager.Instance.ReturnAllCustomerFromLine(); // 대기열에 있는 손님들을 모두 반환
            }
            if (CustomerManager.Instance.RemainCustomers == 0) 
            {
                if(PhaseManager.Instance.CurrentPhase.PhaseType == EPhaseType.ServingPhase)
                {
                    PhaseManager.Instance.TransitionPhase(EPhaseType.EndingPhase);
                }
                return;
            }
        }
        else
        {
            CustomerManager.Instance.InviteCustomer(deltaTime); // 손님 초대
        }

    }

    public override void ExitPhase()
    {
        base.ExitPhase();
    }
}
