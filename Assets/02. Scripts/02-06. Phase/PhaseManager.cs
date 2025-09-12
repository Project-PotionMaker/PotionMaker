using System;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using System.Collections;
using System.Linq;

public class PhaseManager : NetworkBehaviourSingleton<PhaseManager>, IShopInfoSaveable
{
    public event Action OnDayPassed;
    public event Action OnPhaseChanged;
    public event Action OnTimerRunning;
    public event Action OnDeathCountChanged;
    public event Action<List<PotionData>> OnPickCompleted;

    public static event Action OnInitialized;

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

    private SyncList<int> _potionTIDList = new();
    public IReadOnlyList<int> PotionTIDList => _potionTIDList;

    [SyncVar]
    private bool _isGameOver = false;
    public bool IsGameOver { get => _isGameOver; set => _isGameOver = value; }

    public override void OnStartServer()
    {
        base.OnStartServer();
        if (DataTable.Instance.GetPotionDataList() == null)
        {
            Global.Instance.OnDataLoaded += ServerInitialize;
        }
        else
        {
            ServerInitialize();
        }
    }

    public override void OnStartClient()
    {
        base.OnStartClient();
        InitPhase();
    }

    private void Update()
    {
        _currentPhase?.Update(Time.deltaTime);
    }

    public void InitPhase()
    {
        // _potionTIDList.Callback += OnPotionTIDListUpdated;
        _deathCount = _maxDeathCount;
        _day = ShopInfoManager.Instance.ShopInfo.Day;
        _phaseDictionary = new Dictionary<EPhaseType, BasePhase>
        {
            { EPhaseType.PreparingPhase, new PreparingPhase() },
            { EPhaseType.ServingPhase, new ServingPhase() },
            { EPhaseType.EndingPhase, new EndingPhase() },
            { EPhaseType.PracticingPhase, new PracticingPhase() }
        };
        _currentPhase = _phaseDictionary[EPhaseType.PreparingPhase];
        _currentPhase.EnterPhase();
        OnInitialized?.Invoke();
    }

    [Server]
    public void ServerInitialize()
    {
        _dailyPotionPicker = new DailyPotionPicker();
        StartCoroutine(Coroutine_ServerPickPotionListFromHouse());
    }

    [Server]
    public IEnumerator Coroutine_ServerPickPotionListFromHouse()
    {
        yield return new WaitUntil(() => PotionHouse.Instance != null);
        yield return new WaitUntil(() => NetworkServer.connections.Values.All(conn => conn.isReady));

        List<PotionData> potionDataList = 
            _dailyPotionPicker.PickDailyPotion(PotionHouse.Instance.PotionHouseTier);
        _potionTIDList.Clear();
        foreach (PotionData potionData in potionDataList)
        {
            _potionTIDList.Add(potionData.TID);
        }

        RpcOnPotionPickCompleted(potionDataList);
    }

    [ClientRpc]
    public void RpcOnPotionPickCompleted(List<PotionData> potionDataList)
    {
        OnPickCompleted?.Invoke(potionDataList);
    }

    [Server]
    public void ServerTransitionPhase(EPhaseType nextPhase)
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
                StartCoroutine(Coroutine_ServerPickPotionListFromHouse()); 
            }
        }
        _currentPhase = _phaseDictionary[nextPhase];
        _currentPhase.EnterPhase();
        OnPhaseChanged?.Invoke();
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

    public void ApplyLoadedData(ShopInfo shopInfo)
    {
        _day = shopInfo.Day;
    }

    public void ProvideSaveData(ShopInfo shopInfo)
    {
        shopInfo.Day = _day;
    }
}
