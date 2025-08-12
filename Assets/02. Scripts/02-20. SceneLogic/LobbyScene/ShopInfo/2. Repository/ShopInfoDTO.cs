using System;
using System.Collections.Generic;
using System.Linq;

[Serializable]
// ShopInfoDTO 내부의 커스텀 클래스 역시 직렬화가 가능해야 한다.
public class ShopInfoDTO
{
    public readonly string ShopName;
    public readonly int Day;
    public readonly int PotionHouseTier;
    public readonly CurrencyDTO Currency;
    public readonly ReputationDTO Reputation;
    public readonly SalesDTO Sales;
    public readonly List<GridSaveDataDTO> GridSaveDataList; // 그리드 가구 배치정보

    public ShopInfoDTO(string shopName, int day, int potionHouseTier, CurrencyDTO currency,
        ReputationDTO reputation, SalesDTO sales)
    {
        ShopName = shopName;
        Day = day;
        PotionHouseTier = potionHouseTier;
        Currency = currency;
        Reputation = reputation;
        Sales = sales;
    }

    public ShopInfoDTO(ShopInfo shopInfo)
    {
        ShopName = shopInfo.ShopName;
        Day = shopInfo.Day;
        PotionHouseTier = shopInfo.PotionHouseTier;
        Currency = shopInfo.Currency.ToDTO();
        Reputation = shopInfo.Reputation.ToDTO();
        Sales = shopInfo.Sales.ToDTO();
        GridSaveDataList = shopInfo.GridSaveDataList.Select(gridSaveData => gridSaveData.ToDTO()).ToList();
    }
}