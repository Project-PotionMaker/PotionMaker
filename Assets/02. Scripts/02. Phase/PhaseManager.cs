using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum EPhaseType
{
    PreparingPhase,
    ServingPhase,
    EndingPhase
}

public class PhaseManager : MonoBehaviourSingleton<PhaseManager>    
{
    private BasePhase _currentPhase;
    public BasePhase CurrentPhase { get => _currentPhase; set => _currentPhase = value; }
    private Dictionary<EPhaseType, BasePhase> _phaseDictionary;

    [SerializeField]
    private int _day;
    public int Day { get => _day; set => _day = value; }
    public event Action OnDayPassed;

    private void Start()
    {
        InitPhase();
    }

    private void Update()
    {
        _currentPhase?.Update(Time.deltaTime);
    }

    public void InitPhase()
    {
        //if(저장 데이터가 null이면)
        {
            _day = 1;
        }//else
        {
            //저장 데이터에서 _day를 불러오기
        }
        _phaseDictionary = new Dictionary<EPhaseType, BasePhase>
        {
            { EPhaseType.PreparingPhase, new PreparingPhase() },
            { EPhaseType.ServingPhase, new ServingPhase() },
            { EPhaseType.EndingPhase, new EndingPhase() },
        };
        _currentPhase = _phaseDictionary[EPhaseType.PreparingPhase];
        _currentPhase.EnterPhase();
    }

    public void TransitionPhase(EPhaseType nextPhase)
    {
        _currentPhase?.ExitPhase();
        if (_currentPhase is EndingPhase && _phaseDictionary[nextPhase] is PreparingPhase)
        {
            _day++;
            OnDayPassed?.Invoke();
        }
        _currentPhase = _phaseDictionary[nextPhase];
        _currentPhase.EnterPhase();
    }
}
