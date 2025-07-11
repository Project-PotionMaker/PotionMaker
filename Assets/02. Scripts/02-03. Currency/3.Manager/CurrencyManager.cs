using Photon.Pun;
using System;
using System.Diagnostics;


public class CurrencyManager : MonoBehaviourSingleton<CurrencyManager>
{
    public event Action OnDataChanged;

    private PhotonView _photonView;

    private Currency _coin;
    public CurrencyDTO Coin => _coin.ToDTO();

    protected override void Awake()
    {
        base.Awake();
        _photonView = GetComponent<PhotonView>();
    }

    private void Start()
    {
        _coin = new Currency(0);
        OnDataChanged?.Invoke();
        // Todo: Save총괄로부터 데이터 받아온 후 초기화
    }

    [PunRPC]
    public void AddCurrency(int value)
    {
        //if (!PhotonNetwork.IsMasterClient)
        //{
        //    return;
        //}
        _coin.AddCurrency(value);
        OnDataChanged?.Invoke();

        //_photonView.RPC(nameof(SetCurrency), RpcTarget.Others, _coin.Value);
    }

    [PunRPC]
    public bool TrySubtractCurrency(int value)
    {
        //if (!PhotonNetwork.IsMasterClient)
        //{
        //    return false;
        //}

        bool result = _coin.TrySubtractCurrency(value);

        if (result)
        {
            OnDataChanged?.Invoke();

            //_photonView.RPC(nameof(SetCurrency), RpcTarget.Others, _coin.Value);
            UnityEngine.Debug.Log("Subtract succed");
            return true;
        }
        UnityEngine.Debug.Log("Subtract failed");
        return false;
    }

    [PunRPC]
    public void SetCurrency(int value)
    {
        _coin.SetCurrency(value);
        OnDataChanged?.Invoke();
    }
}
