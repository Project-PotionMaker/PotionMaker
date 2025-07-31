using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using VInspector;
using Mirror;

public class PhaseManager : NetworkBehaviour
{
    // TODO :NetworkBehaviourSingleton으로 수정
    public static PhaseManager Instance { get; private set; }

    private BasePhase _currentPhase;
    public BasePhase CurrentPhase { get => _currentPhase; set => _currentPhase = value; }
    private Dictionary<EPhaseType, BasePhase> _phaseDictionary;
    public Dictionary<EPhaseType, BasePhase> PhaseDictionary { get => _phaseDictionary; set => _phaseDictionary = value; }

    private int _deathCount;
    public int DeathCount { get => _deathCount; set => _deathCount = value; }
    [SerializeField]
    private int _maxDeathCount = 5;
    public int MaxDeathCount { get => _maxDeathCount; set => _maxDeathCount = value; }
    private int _day;
    public int Day { get => _day; set => _day = value; }
    public event Action OnDayPassed;
    public event Action OnPhaseChanged;

    public void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Debug.LogWarning("중복된 PhaseManager 인스턴스 발견됨. 기존 인스턴스를 유지합니다.");
            Destroy(gameObject);
        }
        _deathCount = 0;
        InitPhase();
    }

    private void Update()
    {
        if (isServer)
        {
            _currentPhase?.Update(Time.deltaTime);
        }
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
            { EPhaseType.PracticingPhase, new PracticingPhase() }
        };
        _currentPhase = _phaseDictionary[EPhaseType.PreparingPhase];
        _currentPhase.EnterPhase();
    }

    public void TransitionPhase(EPhaseType nextPhase)
    {
        if (isServer)
        {
            RpcTransitionPhase(nextPhase);
        }
    }
    [ClientRpc]
    public void RpcTransitionPhase(EPhaseType nextPhase)
    {
        _currentPhase?.ExitPhase();
        if (_currentPhase is EndingPhase && _phaseDictionary[nextPhase] is PreparingPhase)
        {
            _day++;
            OnDayPassed?.Invoke();
        }
        _currentPhase = _phaseDictionary[nextPhase];
        _currentPhase.EnterPhase();
        OnPhaseChanged?.Invoke();
    }
}
