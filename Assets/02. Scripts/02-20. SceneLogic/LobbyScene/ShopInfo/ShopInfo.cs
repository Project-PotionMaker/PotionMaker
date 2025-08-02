using System;
using UnityEngine;


[Serializable]
// 저장 데이터가 굉장히 많이 추가될 예정
public class ShopInfo
{
    public string ShopName;
    public Currency Currency;
    public int Day;
    

    public ShopInfo()
    {
        ShopName = string.Empty;
        Currency = new Currency();
        Day = 0;
    }

    public ShopInfo(string shopName)
    {
        ShopName = shopName;
        Currency = new Currency();
        Day = 0;
    }

    public ShopInfo(string shopName, Currency currency, int day)
    {
        ShopName = shopName;
        Currency = currency;
        Day = day;
    }
}
