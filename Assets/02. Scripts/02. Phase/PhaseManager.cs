using System;
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
    private BasePhase _preparingPhase;
    public BasePhase PreparingPhase { get => _preparingPhase; set => _preparingPhase = value; }
    private BasePhase _servingPhase;
    public BasePhase ServingPhase { get => _servingPhase; set => _servingPhase = value; }
    private BasePhase _endingPhase;
    public BasePhase EndingPhase { get => _endingPhase; set => _endingPhase = value; }

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
        _preparingPhase = new PreparingPhase();
        _servingPhase = new ServingPhase();
        _endingPhase = new EndingPhase();
        _currentPhase = _preparingPhase;
        _currentPhase.EnterPhase();
    }

    public void TransitionPhase(BasePhase nextPhase)
    {
        if (_currentPhase != null)
        {
            _currentPhase.ExitPhase();
        }
        if (_currentPhase is EndingPhase && nextPhase is PreparingPhase)
        {
            _day++;
            OnDayPassed?.Invoke();
        }
        _currentPhase = nextPhase;
        _currentPhase.EnterPhase();
    }
}
