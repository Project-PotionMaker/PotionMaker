using UnityEngine;
using System;
using Mirror;

public class ServingPhase : BasePhase
{
    private float _initTimer = 120f;
    public float InitTimer => _initTimer;

    private float _currentTime;
    public float CurrentTime { get => _currentTime; set => _currentTime = value; }

    private bool _timesUp = false;
    public ServingPhase()
    {
        _phaseType = EPhaseType.ServingPhase;
    }
    public override void EnterPhase()
    {
        base.EnterPhase();
        _currentTime = _initTimer;
        _timesUp = false;
        AudioManager.Instance.PlaySFX(EPhaseAudioType.EnterServingPhase);
    }

    public override void Update(float deltaTime)
    {
        base.Update(deltaTime);
        if(!NetworkServer.active)
        {
            return;
        }

        _currentTime = Mathf.Max(0, _currentTime - deltaTime);
        if (_currentTime <= 0)
        {
            if(_timesUp == false)
            {
                Debug.Log("타임업! 손님들을 모두 반환합니다.");
                _timesUp = true;
                //CustomerManager.Instance.OnLastOrderTime(); // 대기열에 있는 손님들을 모두 반환
            }
            if (CustomerManager.Instance.RemainCustomers == 0) 
            {
                PhaseManager.Instance.ServerTransitionPhase(EPhaseType.EndingPhase);
                return;
            }
        }
        else
        {
            PhaseManager.Instance.SetCurrnetTime(_currentTime / _initTimer); // 타이머 비율 계산
            CustomerManager.Instance.InviteCustomer(deltaTime); // 손님 초대
        }

    }

    public override void ExitPhase()
    {
        base.ExitPhase();
    }
}
