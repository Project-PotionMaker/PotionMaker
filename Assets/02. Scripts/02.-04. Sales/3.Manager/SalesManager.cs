using Photon.Pun;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SalesManager : MonoBehaviourPunCallbacksSingleton<SalesManager>
{
    private PhotonView _photonView;
    public PhotonView PhotonView => _photonView;
    private Sales _sales;
    public SalesDTO Sales => _sales.ToDTO();


    protected override void Awake()
    {
        base.Awake();
        _photonView = GetComponent<PhotonView>();
    }
    private void Start()
    {
        PhaseManager.Instance.OnDayPassed += OnDayChanged;
        InitSalesManager();
    }

    // 없애도 됨
    public override void OnJoinedRoom()
    {
        InitSalesManager();
    }

    private void InitSalesManager()
    {
        //없애도 됨
        if (!PhotonNetwork.InRoom)
        {
            return;
        }
        _sales = new Sales(0);
        RequestUpdateSales();
    }

    [PunRPC]
    public void RequestSell(EPotionType potionType, int price)
    {
        if (!PhotonNetwork.IsMasterClient)
        {
            _photonView.RPC(nameof(RequestSell), RpcTarget.MasterClient, potionType, price);
            return;
        }
        Sell(potionType, price);
    }

    private void Sell(EPotionType potionType, int price)
    {
        if (!PhotonNetwork.IsMasterClient)
        {
            throw new InvalidOperationException("Sale must be processed only by the Master Client.");
        }
        _sales.Sell(potionType, price);

        SalesRPCData salesRPCData = new SalesRPCData(Sales);
        string salesJson = JsonUtility.ToJson(salesRPCData);
        _photonView.RPC(nameof(SetSales), RpcTarget.Others, salesJson);

        CurrencyManager.Instance.RequestAddCurrency(price);
    }

    [PunRPC]
    public void SetSales(string salesJson,PhotonMessageInfo info)
    {
        if (!info.Sender.IsMasterClient)
        {
            throw new InvalidOperationException("Sales must be Set by the Master Client");
        }

        SalesRPCData salesRPCData = JsonUtility.FromJson<SalesRPCData>(salesJson);
        Dictionary<EPotionType, int> totalSalesVolumeDict = salesRPCData.TotalSalesVolumeKeyValueList.ToDictionary(keyValuePair => keyValuePair.Key, keyValuePair => keyValuePair.Value);
        Dictionary<EPotionType, int> dailySalesVolumeDict = salesRPCData.DailySalesVolumeKeyValueList.ToDictionary(keyValuePair => keyValuePair.Key, keyValuePair => keyValuePair.Value);

        _sales.SetSales(salesRPCData.TotalSales, salesRPCData.DailySales, totalSalesVolumeDict, dailySalesVolumeDict);
    }

    public void RequestUpdateSales()
    {
        _photonView.RPC(nameof(RequestUpdateSales), RpcTarget.MasterClient);
    }

    [PunRPC]
    public void RequestUpdateSales(PhotonMessageInfo info)
    {
        if (!PhotonNetwork.IsMasterClient)
        {
            _photonView.RPC(nameof(RequestUpdateSales), RpcTarget.MasterClient);
            return;
        }

        SalesRPCData salesRPCData = new SalesRPCData(Sales);
        string salesJson = JsonUtility.ToJson(salesRPCData);
        _photonView.RPC(nameof(SetSales), info.Sender, salesJson);
    }

    public void OnDayChanged()
    {
        if (!PhotonNetwork.IsMasterClient)
        {
            return;
        }
        _sales.OnDayChanged();
        
        SalesRPCData salesRPCData = new SalesRPCData(Sales);
        string salesJson = JsonUtility.ToJson(salesRPCData);
        _photonView.RPC(nameof(SetSales), RpcTarget.Others, salesJson);
    }
}
