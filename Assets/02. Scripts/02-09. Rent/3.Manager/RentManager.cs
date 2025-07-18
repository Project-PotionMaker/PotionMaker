using Photon.Pun;
using System;
using UnityEngine;

public class RentManager : MonoBehaviourPunCallbacksSingleton<RentManager>
{
    private Rent _rent;
    public RentDTO Rent => _rent.ToDTO();

    private PhotonView _photonView;
    public PhotonView PhotonView => _photonView;

    protected override void Awake()
    {
        base.Awake();
        _photonView = GetComponent<PhotonView>();
    }
    private void Start()
    {
        Global.Instance.OnDataLoaded += InitRentManager;
        InitRentManager();
    }
    private void InitRentManager()
    {
        // InRoom은 나중에 만들어지면 없애도 될 듯? 이미 방 들어온 채로 이 씬으로 오니까
        if (!Global.Instance.IsDataLoaded || !PhotonNetwork.InRoom)
        {
            return;
        }
        LayoutData data = DataTable.Instance.GetLayoutData(10000);
        _rent = new Rent(1, data.InitialRentCost, data.RentIncrement);
        RequestUpdateRent();
    }

    // 이것도 없애도 되고
    public override void OnJoinedRoom()
    {
        InitRentManager();
    }
    public void RequestUpdateRent()
    {
        _photonView.RPC(nameof(RequestUpdateRent), RpcTarget.MasterClient);
    }

    [PunRPC]
    public void RequestUpdateRent(PhotonMessageInfo info)
    {
        if (!PhotonNetwork.IsMasterClient)
        {
            _photonView.RPC(nameof(RequestUpdateRent), RpcTarget.MasterClient);
            return;
        }

        RentRPCData rentRPCData = new RentRPCData(Rent);
        string rentJson = JsonUtility.ToJson(rentRPCData);
        _photonView.RPC(nameof(SetRent), info.Sender, rentJson);
    }

    [PunRPC]
    public void SetRent(string rentJson, PhotonMessageInfo info)
    {
        if (!info.Sender.IsMasterClient)
        {
            throw new InvalidOperationException("Rent must be Set by the Master Client");
        }
        RentRPCData rentRPCData = JsonUtility.FromJson<RentRPCData>(rentJson);
        _rent.SetRent(rentRPCData.RentDayCounter, rentRPCData.CurrentRentCost, rentRPCData.RentIncrement);
    }

    public void PayRent()
    {
        if (!PhotonNetwork.IsMasterClient)
        {
            throw new InvalidOperationException("Rent must be Paid by the Master Client");
        }
        bool result = CurrencyManager.Instance.TrySubtractCurrency(_rent.CurrentRentCost);
        if (!result)
        {
            Debug.Log("게임오버");
            return;
        }
        _rent.OnRentPaid();
        RentRPCData rentRPCData = new RentRPCData(Rent);
        string rentJson = JsonUtility.ToJson(rentRPCData);
        _photonView.RPC(nameof(SetRent), RpcTarget.Others, rentJson);
    }
}
