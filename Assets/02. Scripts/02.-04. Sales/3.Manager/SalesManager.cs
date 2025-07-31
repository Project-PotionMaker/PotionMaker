using Mirror;
using Photon.Pun;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SalesManager : NetworkBehaviourSingleton<SalesManager>
{
    public event Action OnSummaryReady;

    private Sales _sales;
    public SalesDTO Sales => _sales.ToDTO();

    private void Start()
    {
        PhaseManager.Instance.OnDayPassed += OnDayChanged;
        InitSalesManager();
    }

    // 네트워크 매니저에서 처리
    //public override void OnJoinedRoom()
    //{
    //    InitSalesManager();
    //}

    private void InitSalesManager()
    {
        _sales = new Sales(0);
        CmdRequestUpdateSales(false);
    }

    [Command]
    public void CmdRequestSell(EPotionType potionType, int price)
    {
        Sell(potionType, price);
    }

    [Server]
    private void Sell(EPotionType potionType, int price)
    {
        if (!isServer)
        {
            throw new InvalidOperationException($"{nameof(Sell)}() is server-only. Use {nameof(CmdRequestSell)}() from client.");
        }
        _sales.Sell(potionType, price);

        SalesRPCData salesRPCData = new SalesRPCData(Sales);
        string salesJson = JsonUtility.ToJson(salesRPCData);
        UpdateSales(salesJson, false);

        CurrencyManager.Instance.CmdRequestAddCurrency(price);
    }

    [ClientRpc]
    public void UpdateSales(string salesJson, bool isForSummary)
    {
        SalesRPCData salesRPCData = JsonUtility.FromJson<SalesRPCData>(salesJson);
        Dictionary<EPotionType, int> totalSalesVolumeDict = salesRPCData.TotalSalesVolumeKeyValueList.ToDictionary(keyValuePair => keyValuePair.Key, keyValuePair => keyValuePair.Value);
        Dictionary<EPotionType, int> dailySalesVolumeDict = salesRPCData.DailySalesVolumeKeyValueList.ToDictionary(keyValuePair => keyValuePair.Key, keyValuePair => keyValuePair.Value);

        _sales.SetSales(salesRPCData.TotalSales, salesRPCData.DailySales, totalSalesVolumeDict, dailySalesVolumeDict);
        if (isForSummary)
        {
            OnSummaryReady?.Invoke();
        }
    }

    [Command]
    public void CmdRequestUpdateSales(bool isForSummary)
    {
        SalesRPCData salesRPCData = new SalesRPCData(Sales);
        string salesJson = JsonUtility.ToJson(salesRPCData);
        UpdateSales(salesJson, isForSummary);
    }

    [Server]
    public void OnDayChanged()
    {
        if (!isServer)
        {
            throw new InvalidOperationException($"{nameof(OnDayChanged)}() is server-only.");
        }
        _sales.OnDayChanged();
        
        SalesRPCData salesRPCData = new SalesRPCData(Sales);
        string salesJson = JsonUtility.ToJson(salesRPCData);
        UpdateSales(salesJson, false);
    }
}
