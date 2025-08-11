using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


[Serializable]
public class ShopInfoDTO
{
    public readonly string ShopName;
    public readonly CurrencyDTO Currency;
    public readonly ReputationDTO Reputation;
    public readonly SalesDTO Sales;
    public readonly List<GridSaveDataDTO> GridSaveDataList; // 그리드 가구 배치정보
    public readonly int Day;

    public ShopInfoDTO(string shopName, CurrencyDTO currency, 
        ReputationDTO reputation, SalesDTO sales, int day)
    {
        ShopName = shopName;
        Currency = currency;
        Reputation = reputation;
        Sales = sales;
        Day = day;
    }

    public ShopInfoDTO(ShopInfo shopInfo)
    {
        ShopName = shopInfo.ShopName;
        Currency = shopInfo.Currency.ToDTO();
        Reputation = shopInfo.Reputation.ToDTO();
        Sales = shopInfo.Sales.ToDTO();
        GridSaveDataList = shopInfo.GridSaveDataList.Select(gridSaveData => gridSaveData.ToDTO()).ToList();
        Day = shopInfo.Day;
    }
}
