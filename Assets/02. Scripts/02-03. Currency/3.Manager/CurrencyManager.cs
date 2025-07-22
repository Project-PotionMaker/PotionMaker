using Photon.Pun;
using System;
using System.Diagnostics;

public class CurrencyManager : MonoBehaviourPunCallbacksSingleton<CurrencyManager>
{
    public event Action OnDataChanged;

    private PhotonView _photonView;
    public PhotonView PhotonView => _photonView;

    private Currency _coin;
    public CurrencyDTO Coin => _coin.ToDTO();

    protected override void Awake()
    {
        base.Awake();
        _photonView = GetComponent<PhotonView>();
    }

    private void Start()
    {
        InitCurrencyManager();
    }

    // 없애도 됨
    public override void OnJoinedRoom()
    {
        InitCurrencyManager();
    }

    private void InitCurrencyManager()
    {
        //없애도 됨
        if (!PhotonNetwork.InRoom)
        {
            return;
        }
        _coin = new Currency(0);
        RequestUpdateCurrency();
        OnDataChanged?.Invoke();
        // Todo: Save총괄로부터 데이터 받아온 후 초기화
    }

    public void RequestAddCurrency(int addendValue)
    {
        if (!PhotonNetwork.IsMasterClient)
        {
            _photonView.RPC(nameof(RPC_AddCurrency), RpcTarget.MasterClient, addendValue);
            return;
        }

        RPC_AddCurrency(addendValue);
    }

    [PunRPC]
    public void RPC_AddCurrency(int addendValue)
    {
        if (!PhotonNetwork.IsMasterClient)
        {
            throw new InvalidOperationException("Only the Master Client may Add currency directly. Use 'RequestAddCurrency' instead.");
        }
        _coin.AddCurrency(addendValue);
        OnDataChanged?.Invoke();

        _photonView.RPC(nameof(RPC_SetCurrency), RpcTarget.Others, _coin.Value);
    }

    public bool TrySubtractCurrency(int subtrahendValue)
    {
        if (!PhotonNetwork.IsMasterClient)
        {
            throw new InvalidOperationException("Only the Master Client may Subtract currency directly. Request Subtract via a high level action method." +
                                "\ne.g., \n[RPC] MarketManager.RequestBuy()" +
                                "\n ->   MarketManager.TryBuy()" +
                                "\n ->   CurrencyManager.TrySubtractCurrency()");
        }

        bool result = _coin.TrySubtractCurrency(subtrahendValue);

        if (result)
        {
            OnDataChanged?.Invoke();

            _photonView.RPC(nameof(RPC_SetCurrency), RpcTarget.Others, _coin.Value);
            UnityEngine.Debug.Log("Subtract succed");
            return true;
        }
        UnityEngine.Debug.Log("Subtract failed");
        return false;
    }

    // 마스터 클라이언트에서 클라이언트 갱신시키는 용도
    [PunRPC]
    public void RPC_SetCurrency(int value, PhotonMessageInfo info)
    {
        if (!info.Sender.IsMasterClient)
        {
            throw new InvalidOperationException("Currency must be Set by the Master Client");
        }
        _coin.SetCurrency(value);
        OnDataChanged?.Invoke();
    }

    // 갱신 요청
    public void RequestUpdateCurrency()
    {
        if (!PhotonNetwork.IsMasterClient)
        {
            _photonView.RPC(nameof(RPC_UpdateCurrency), RpcTarget.MasterClient);
            return;
        }
        RPC_UpdateCurrency();
    }

    [PunRPC]
    public void RPC_UpdateCurrency()
    {
        if (!PhotonNetwork.IsMasterClient)
        {
            throw new InvalidOperationException("Only the Master Client may Update currency directly. Use 'RequestUpdateCurrency' instead.");
        }
        _photonView.RPC(nameof(RPC_SetCurrency), RpcTarget.Others, _coin.Value);
    }
}
