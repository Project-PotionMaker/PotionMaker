using Mirror;
using System;

public class CurrencyManager : NetworkBehaviourSingleton<CurrencyManager>
{
    public static event Action OnDataChanged;

    private Currency _coin;
    public CurrencyDTO Coin => _coin.ToDTO();


    public override void OnStartClient()
    {
        base.OnStartClient();
        InitCurrencyManager();
    }
    
    private void InitCurrencyManager()
    {
        if (!NetworkClient.ready)
        {
            return;
        }
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
