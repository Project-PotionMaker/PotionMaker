using Mirror;
//using Photon.Pun;
using System;
using System.Diagnostics;

public class CurrencyManager : NetworkBehaviour
{
    public event Action OnDataChanged;
    public static Action OnInitialized;

    private Currency _coin;
    public CurrencyDTO Coin => _coin.ToDTO();

    private static CurrencyManager _instance;
    public static CurrencyManager Instance
    {
        get
        {
            if (_instance == null)
            {
                // 매우 위험한 로직. Awake가 이미 호출되어 있어야 함.
                // 만약 이 시점에 _instance가 null이라면, 네트워크 관련 초기화가 되지 않았을 가능성이 높습니다.
                // 따라서 이 프로퍼티는 'Awake에서 초기화된다'는 가정 하에 사용해야 합니다.
                UnityEngine.Debug.LogError("CurrencyManager.Instance가 아직 초기화되지 않았습니다. Awake() 또는 OnStartClient()가 호출될 때까지 기다리세요.");
            }
            return _instance;
        }
    }

    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(this); // 씬 전환 시에도 유지
            UnityEngine.Debug.Log("CurrencyManager 인스턴스 초기화 완료 (Awake).");
        }
        else if (_instance != this)
        {
            UnityEngine.Debug.LogWarning("중복된 CurrencyManager 인스턴스 발견. 파괴합니다.");
            Destroy(gameObject);
        }
    }

    public override void OnStartServer()
    {
        base.OnStartServer();
    }

    public override void OnStartClient()
    {
        base.OnStartClient();
        OnInitialized?.Invoke();
        InitCurrencyManager();
    }

    private void InitCurrencyManager()
    {
        _coin = new Currency(0);
        CmdRequestUpdateCurrency();
        OnDataChanged?.Invoke();
        // Todo: Save총괄로부터 데이터 받아온 후 초기화
    }

    [Command(requiresAuthority = false)]
    public void CmdRequestAddCurrency(int addendValue)
    {
        AddCurrency(addendValue);
    }

    [Server]
    private void AddCurrency(int addendValue)
    {
        if (!isServer)
        {
            throw new InvalidOperationException($"{nameof(AddCurrency)}() is server-only. Use {nameof(CmdRequestAddCurrency)}() from client.");
        }
        _coin.AddCurrency(addendValue);
        OnDataChanged?.Invoke();

        UpdateCurrency(_coin.Value);
    }

    [Server]
    public bool TrySubtractCurrency(int subtrahendValue)
    {
        if (!isServer)
        {
            throw new InvalidOperationException($"{nameof(TrySubtractCurrency)}() is sever-only. Request Subtract via a high level action method from client." +
                                $"\ne.g., \n {nameof(ProductManager)}.Instance.{nameof(ProductManager.CmdRequestBuy)}()");
        }

        bool result = _coin.TrySubtractCurrency(subtrahendValue);

        if (result)
        {
            OnDataChanged?.Invoke();

            UpdateCurrency(_coin.Value);
            UnityEngine.Debug.Log("Subtract succed");
            return true;
        }
        UnityEngine.Debug.Log("Subtract failed");
        return false;
    }

    // 마스터 클라이언트에서 클라이언트 갱신시키는 용도
    [ClientRpc]
    public void UpdateCurrency(int value)
    {
        _coin.SetCurrency(value);
        OnDataChanged?.Invoke();
    }

    // 갱신 요청
    [Command(requiresAuthority = false)]
    public void CmdRequestUpdateCurrency()
    {
        UpdateCurrency(_coin.Value);
    }
}
