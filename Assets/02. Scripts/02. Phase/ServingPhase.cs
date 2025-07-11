using UnityEngine;
using System;

public class ServingPhase : BasePhase
{
    public event Action OnServingPhaseEntered;
    public event Action OnServingPhaseExited;

    private const float INIT_TIMER = 20f;
    [SerializeField]
    private float _currentTime;
    public float CurrentTime { get => _currentTime; set => _currentTime = value; }
    [SerializeField]
    private int _remainCustomers;
    public int RemainCustomers { get => _remainCustomers; set => _remainCustomers = value; }

    public ServingPhase()
    {
        _phaseType = EPhaseType.ServingPhase;
    }
    public override void EnterPhase()
    {
        base.EnterPhase();
        _currentTime = INIT_TIMER;
        _remainCustomers = 0;
        OnServingPhaseEntered?.Invoke();
    }

    public override void Update(float deltaTime)
    {
        base.Update(deltaTime);
        _currentTime = Mathf.Max(0,_currentTime-deltaTime);
        //TODO : HUD 상단의 타이머와 연동

        if (_currentTime <= 0)
        {
            if(_remainCustomers == 0) 
            {
                PhaseManager.Instance.TransitionPhase(PhaseManager.Instance.EndingPhase);
                return;
            }
        }
        else
        {
            //TODO : NPC 시스템과 연동해 손님이 오게 만들기
        }

    }

    public override void ExitPhase()
    {
        base.ExitPhase();
        OnServingPhaseExited?.Invoke();
    }
}
