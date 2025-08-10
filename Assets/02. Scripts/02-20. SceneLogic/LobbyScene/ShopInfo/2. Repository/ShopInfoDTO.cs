using System;
using System.Collections.Generic;


[Serializable]
public class ShopInfoDTO
{
    public readonly string ShopName;
    public readonly Currency Currency;
    public readonly Reputation Reputation;
    public readonly Sales Sales;
    public int Day;

    // 추가적으로 필요한 정보
    // 현재 포션 상점에서의 그리드 가구 배치정보

    public ShopInfoDTO(string shopName, Currency currency, 
        Reputation reputation, Sales sales, int day)
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
        Currency = shopInfo.Currency;
        Reputation = shopInfo.Reputation;
        Sales = shopInfo.Sales;
        Day = shopInfo.Day;
    }
}
