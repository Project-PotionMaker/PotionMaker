using Mirror;
//using Photon.Pun;
using System;
using UnityEngine;

public class RentManager : NetworkBehaviourSingleton<RentManager>
{
    private Rent _rent;
    public RentDTO Rent => _rent.ToDTO();

    public override void OnStartClient()
    {
        Global.Instance.OnDataLoaded += InitRentManager;
        InitRentManager();
    }

    private void InitRentManager()
    {
        if (!Global.Instance.IsDataLoaded)
        {
            return;
        }

        // 레이아웃 데이터를 들고 있는 매니저로부터 레이아웃 TID 가져옴
        LayoutData data = DataTable.Instance.GetLayoutData(10000);
        _rent = new Rent(1, data.InitialRentCost, data.RentIncrement);
        CmdRequestUpdateRent();
    }

    // 네트워크 매니저에서 처리
    //public override void OnJoinedRoom()
    //{
    //    InitRentManager();
    //}

    [Command(requiresAuthority = false)]
    public void CmdRequestUpdateRent()
    {
        RentRPCData rentRPCData = new RentRPCData(Rent);
        string rentJson = JsonUtility.ToJson(rentRPCData);
        UpdateRent(rentJson);
    }

    [ClientRpc]
    public void UpdateRent(string rentJson)
    {
        RentRPCData rentRPCData = JsonUtility.FromJson<RentRPCData>(rentJson);
        _rent.SetRent(rentRPCData.RentDayCounter, rentRPCData.CurrentRentCost, rentRPCData.RentIncrement);
    }

    [Command(requiresAuthority = false)]
    public void CmdRequestPayRent()
    {
        PayRent();
    }

    [Server]
    private void PayRent()
    {
        if (!isServer)
        {
            throw new InvalidOperationException($"{nameof(PayRent)}() is server-only. Use {nameof(CmdRequestPayRent)}() from client.");
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
        UpdateRent(rentJson);
    }
}
