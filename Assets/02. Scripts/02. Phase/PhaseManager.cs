using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
//using Photon.Pun;
using VInspector;

public class PhaseManager : MonoBehaviourSingleton<PhaseManager>    
{
    public event Action OnDayPassed;
    public event Action OnPhaseChanged;

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

    private DailyPotionPicker _dailyPotionPicker;
    public DailyPotionPicker DailyPotionPicker => _dailyPotionPicker;

    //PhotonView _photonView;

    protected override void Awake()
    {
        base.Awake();
        //_photonView = GetComponent<PhotonView>();
        _deathCount = 0;
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
            { EPhaseType.PracticingPhase, new PracticingPhase() }
        };
        _currentPhase = _phaseDictionary[EPhaseType.PreparingPhase];
        _currentPhase.EnterPhase();
        _dailyPotionPicker = new DailyPotionPicker();
    }

    public void TransitionPhase(EPhaseType nextPhase)
    {
        //if(PhotonNetwork.IsMasterClient == false)
        //{
        //    return;
        //}
        //_photonView.RPC(nameof(RPC_TransitionPhase), RpcTarget.All, nextPhase);
    }
    //[PunRPC]
    public void RPC_TransitionPhase(EPhaseType nextPhase)
    {
        _currentPhase?.ExitPhase();
        if (_currentPhase is EndingPhase && _phaseDictionary[nextPhase] is PreparingPhase)
        {
            _day++;
            OnDayPassed?.Invoke();
            // _dailyPotionPicker.PickDailyPotion(int currentPotionHouseTier);
        }
        _currentPhase = _phaseDictionary[nextPhase];
        _currentPhase.EnterPhase();
        OnPhaseChanged?.Invoke();
    }
}
