using Mirror;
using System;
using UnityEngine;

public class RentManager : NetworkBehaviourSingleton<RentManager>, IShopInfoSaveable
{
    private Rent _rent;
    public RentDTO Rent => _rent.ToDTO();

    public override void OnStartClient()
    {
        base.OnStartClient();
        InitRentManager();
    }

    private void InitRentManager()
    {
        if (!Global.Instance.IsDataLoaded)
        {
            return;
        }

        _rent = ShopInfoManager.Instance.ShopInfo.Rent;
        //// 레이아웃 데이터를 들고 있는 매니저로부터 레이아웃 TID 가져옴
        //LayoutData data = DataTable.Instance.GetLayoutData(10000);
        //_rent = new Rent(1, data.InitialRentCost, data.RentIncrement);
        CmdRequestUpdateRent();

        PhaseManager.Instance.OnDayPassed += IncreaseRentCount;
        PhaseManager.Instance.PhaseDictionary[EPhaseType.ServingPhase].OnPhaseExited += PayRent;
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
        if(!_rent.IsRentDay)
        {
            Debug.Log("아직 임대료를 지불할 때가 아닙니다.");
            RentRPCData rentRPCDataNotPaid = new RentRPCData(Rent);
            string rentJsonNotPaid = JsonUtility.ToJson(rentRPCDataNotPaid);
            UpdateRent(rentJsonNotPaid);
            return;
        }

        bool result = CurrencyManager.Instance.TrySubtractCurrency(_rent.CurrentRentCost);
        if (!result)
        {
            PhaseManager.Instance.IsGameOver = true;
            return;
        }
        _rent.OnRentPaid();
        RentRPCData rentRPCData = new RentRPCData(Rent);
        string rentJson = JsonUtility.ToJson(rentRPCData);
        UpdateRent(rentJson);
    }

    [Server]
    private void IncreaseRentCount()
    {
        _rent.IncreaseRentDayCounter();
    }

    public void ApplyLoadedData(ShopInfo shopInfo)
    {
        _rent = shopInfo.Rent;
    }

    public void ProvideSaveData(ShopInfo shopInfo)
    {
        shopInfo.Rent = _rent;
    }
}
