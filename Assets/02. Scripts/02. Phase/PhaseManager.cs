using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using VInspector;
using Mirror;

public class PhaseManager : NetworkBehaviourSingleton<PhaseManager>
{
    public event Action OnDayPassed;
    public event Action OnPhaseChanged;
    public event Action OnTimerRunning;
    public event Action OnDeathCountChanged;

    private BasePhase _currentPhase;
    public BasePhase CurrentPhase { get => _currentPhase; set => _currentPhase = value; }
    private Dictionary<EPhaseType, BasePhase> _phaseDictionary;
    public Dictionary<EPhaseType, BasePhase> PhaseDictionary { get => _phaseDictionary; set => _phaseDictionary = value; }
    [SyncVar(hook = nameof(SyncDeathCount))]
    private int _deathCount;
    public int DeathCount { get => _deathCount; set => _deathCount = value; }
    [SerializeField]
    private int _maxDeathCount = 5;
    public int MaxDeathCount { get => _maxDeathCount; set => _maxDeathCount = value; }
    [SyncVar]
    private int _day;
    public int Day { get => _day; set => _day = value; }
    [SyncVar(hook = nameof(SyncTimer))]
    private float _currentTimeRate;
    public float CurrentTimeRate { get => _currentTimeRate; }

    private DailyPotionPicker _dailyPotionPicker;
    public DailyPotionPicker DailyPotionPicker => _dailyPotionPicker;

    private List<PotionData> _potionDataList = new();
    public List<PotionData> PotionDataList => _potionDataList;

    [SyncVar]
    private bool _isGameOver = false;
    public bool IsGameOver { get => _isGameOver; set => _isGameOver = value; }

    protected override void Awake()
    {
        base.Awake();
        _deathCount = _maxDeathCount;
        InitPhase();
        Global.Instance.OnDataLoaded += InitializePotionDataList;
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
    }

    private void InitializePotionDataList()
    {
        _dailyPotionPicker = new DailyPotionPicker();
        // _potionDataList = _dailyPotionPicker.PickDailyPotion(int currentPotionHouseTier);
    }

    [Server]
    public void TransitionPhase(EPhaseType nextPhase)
    {
        RpcTransitionPhase(nextPhase);
    }

    [ClientRpc]
    public void RpcTransitionPhase(EPhaseType nextPhase)
    {
        _currentPhase?.ExitPhase();
        if (_currentPhase is EndingPhase && _phaseDictionary[nextPhase] is PreparingPhase)
        {
            _day++;
            OnDayPassed?.Invoke();
            if (isServer)
            {
                // _potionDataList = _dailyPotionPicker.PickDailyPotion(int currentPotionHouseTier);
                SyncDailyPotionsToClients();
            }
        }
        _currentPhase = _phaseDictionary[nextPhase];
        _currentPhase.EnterPhase();
        OnPhaseChanged?.Invoke();
    }

    [Server]
    public void SyncDailyPotionsToClients()
    {
        int[] potionIDs = new int[_potionDataList.Count];
        for (int i = 0; i < _potionDataList.Count; i++)
        {
            potionIDs[i] = _potionDataList[i].TID;
        }

        RpcSyncPotionList(potionIDs);
    }

    [ClientRpc]
    private void RpcSyncPotionList(int[] potionIDs)
    {
        _potionDataList = new List<PotionData>();

        for (int i = 0; i < potionIDs.Length; i++)
        {
            PotionData data = DataTable.Instance.GetPotionData(potionIDs[i]);
            if (data != null)
            {
                _potionDataList.Add(data);
            }
            else
            {
                Debug.LogWarning($"ID {potionIDs[i]}에 해당하는 포션 데이터를 찾을 수 없습니다.");
            }
        }
    }
    [Server]
    public void SetCurrnetTime(float value)
    {
        _currentTimeRate = value;
    }
    private void SyncTimer(float oldValue, float newValue)
    {
        OnTimerRunning?.Invoke();
    }
    private void SyncDeathCount(int oldValue, int newValue)
    {
        if(newValue <= 0)
        {
            _isGameOver = true;
        }
        OnDeathCountChanged?.Invoke();
    }
    [Server]
    public void ResetDeathCount()
    {
        _deathCount = _maxDeathCount;
    }

}
