using Mirror;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SalesManager : NetworkBehaviourSingleton<SalesManager>, IShopInfoSaveable
{
    public event Action OnSummaryReady;

    private Sales _sales;
    public SalesDTO Sales => _sales.ToDTO();

    private void InitSalesManager()
    {
        if (!NetworkClient.ready)
        {
            return;
        }
        _sales = ShopInfoManager.Instance.ShopInfo.Sales;
        CmdRequestUpdateSales(false);
    }
    public override void OnStartClient()
    {
        base.OnStartClient();
        PhaseManager.Instance.OnDayPassed += OnDayChanged;

        InitSalesManager();
    }

    public void RequestSell(int TID)
    {
        CmdRequestSell(TID);
    }
    [Command(requiresAuthority = false)]
    public void CmdRequestSell(int TID)
    {
        Sell(TID);
    }

    [Server]
    private void Sell(int TID)
    {
        if (!isServer)
        {
            throw new InvalidOperationException($"{nameof(Sell)}() is server-only. Use {nameof(CmdRequestSell)}() from client.");
        }
        int price = DataTable.Instance.GetPotionData(TID).Price;

        _sales.Sell(TID, price);

        SalesRPCData salesRPCData = new SalesRPCData(Sales);
        string salesJson = JsonUtility.ToJson(salesRPCData);
        UpdateSales(salesJson, false);

        CurrencyManager.Instance.CmdRequestAddCurrency(price);
    }

    [ClientRpc]
    public void UpdateSales(string salesJson, bool isForSummary)
    {
        SalesRPCData salesRPCData = JsonUtility.FromJson<SalesRPCData>(salesJson);
        Dictionary<int, int> totalSalesVolumeDict = salesRPCData.TotalSalesVolumeKeyValueList.ToDictionary(keyValuePair => keyValuePair.Key, keyValuePair => keyValuePair.Value);
        Dictionary<int, int> dailySalesVolumeDict = salesRPCData.DailySalesVolumeKeyValueList.ToDictionary(keyValuePair => keyValuePair.Key, keyValuePair => keyValuePair.Value);

        _sales.SetSales(salesRPCData.TotalSales, salesRPCData.DailySales, totalSalesVolumeDict, dailySalesVolumeDict);
        if (isForSummary)
        {
            OnSummaryReady?.Invoke();
        }
    }

    [Command(requiresAuthority = false)]
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

    public void ApplyLoadedData(ShopInfo shopInfo)
    {
        _sales = shopInfo.Sales;
    }

    public void ProvideSaveData(ShopInfo shopInfo)
    {
        shopInfo.Sales = _sales;
    }
}
