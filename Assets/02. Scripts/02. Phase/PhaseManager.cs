using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using VInspector;
using Mirror;
using System.Collections;

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

    private List<int> _potionTIDList = new();
    public List<int> PotionTIDList => _potionTIDList;

    [SyncVar]
    private bool _isGameOver = false;
    public bool IsGameOver { get => _isGameOver; set => _isGameOver = value; }

    public override void OnStartClient()
    {
        base.OnStartClient();
        _deathCount = _maxDeathCount;
        InitPhase();
        if (isServer)
        {
            if(DataTable.Instance.GetPotionDataList() == null)
            {
                Global.Instance.OnDataLoaded += ServerInitializePotionDataList;
            }
            else
            {
                ServerInitializePotionDataList();
            }
        }
        else
        {
            CmdSyncPotionList();
        }
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
        if(PotionHouse.Instance == null)
        {
            Debug.LogWarning("PotionHouse가 아직 초기화되지 않았습니다. 잠시 기다립니다.");
            StartCoroutine(WaitPotionHouse());
            return;
        }

        Debug.Log("서버에서 포션 리스트를 포션 하우스에서 선택합니다.");
        _potionTIDList.Clear();
        List<PotionData> potionDataList = _dailyPotionPicker.PickDailyPotion(PotionHouse.Instance.PotionHouseTier);
        for(int i = 0; i < potionDataList.Count; i++)
        {
            PotionData data = potionDataList[i];
            _potionTIDList.Add(data.TID);
        }
        RpcSyncPotionDataList(_potionTIDList);
        
    }
    [Command]
    private void CmdSyncPotionList()
    {
        RpcSyncPotionDataList(_potionTIDList);
    }

    [ClientRpc]
    private void RpcSyncPotionDataList(List<int> potionTIDList)
    {
        _potionTIDList = potionTIDList;
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

}
