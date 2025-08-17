using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using VInspector;
using Mirror;
using System.Collections;
using System.Linq;

public class PhaseManager : NetworkBehaviourSingleton<PhaseManager>, IShopInfoSaveable
{
    public event Action OnDayPassed;
    public event Action OnPhaseChanged;
    public event Action OnTimerRunning;
    public event Action OnDeathCountChanged;
    public event Action<List<int>> OnPickCompleted;

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
    }

    public override void OnStartClient()
    {
        base.OnStartClient();
        _deathCount = _maxDeathCount;
        InitPhase();
        _potionTIDList.Callback += OnPotionTIDListUpdated;
        if (isServer)
        {
            if (DataTable.Instance.GetPotionDataList() == null)
            {
                Global.Instance.OnDataLoaded += ServerInitializePotionDataList;
            }
            else
            {
                ServerInitializePotionDataList();
            }
        }
    }

    private void Update()
    {
        _currentPhase?.Update(Time.deltaTime);
    }

    public void InitPhase()
    {
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
    private void ServerInitializePotionDataList()
    {
        Debug.Log("서버에서 포션 데이터 리스트 초기화 시작");
        _dailyPotionPicker = new DailyPotionPicker();

        ServerPickPotionListFromHouse();
    }

    private IEnumerator WaitPotionHouse()
    {
        while (PotionHouse.Instance == null)
            yield return null;

        ServerPickPotionListFromHouse(); // 재시도
    }

    [Server]
    private void ServerPickPotionListFromHouse()
    {
        if (PotionHouse.Instance == null)
        {
            Debug.LogWarning("PotionHouse가 아직 초기화되지 않았습니다. 잠시 기다립니다.");
            StartCoroutine(WaitPotionHouse());
            return;
        }

        Debug.Log("서버에서 포션 리스트를 포션 하우스에서 선택합니다.");
        List<PotionData> potionDataList = _dailyPotionPicker.PickDailyPotion(PotionHouse.Instance.PotionHouseTier);
        List<int> potionTIDList = potionDataList.Select(data => data.TID).ToList();
        _potionTIDList.Clear();
        _potionTIDList.AddRange(potionTIDList);
    }

    private void OnPotionTIDListUpdated(SyncList<int>.Operation op, int index, int oldItem, int newItem)
    {
        Debug.Log("OnPotionTIDListUpdated");
        if (op == SyncList<int>.Operation.OP_CLEAR)
        {
            return;
        }
        OnPickCompleted?.Invoke(new List<int>(_potionTIDList));
    }

    [ClientRpc]
    public void RpcOnPotionPickCompleted()
    {
        Debug.Log("RpcOnPotionPickCompleted");
        OnPickCompleted?.Invoke(new List<int>(_potionTIDList));
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
                ServerPickPotionListFromHouse(); 
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
