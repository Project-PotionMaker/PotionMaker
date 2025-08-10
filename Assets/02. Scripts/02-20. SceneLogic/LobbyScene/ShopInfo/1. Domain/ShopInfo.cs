using System;
using UnityEngine;


[Serializable]
// 저장 데이터가 굉장히 많이 추가될 예정
// 클래스들 모두 직렬화 가능해야함
public class ShopInfo
{
    public string ShopName;
    public Currency Currency;
    public Reputation Reputation;
    public Sales Sales;
    public int Day;

    // 추가적으로 필요한 정보
    // 그리드 가구 배치정보
    // 누적 판매량

    public ShopInfo(string shopName)
    {
        ShopInfoSpecification shopInfoSpecification = new ShopInfoSpecification();
        if (!shopInfoSpecification.IsSatisfied(shopName, new Currency(), new Reputation(), 1))
        {
            throw new Exception(shopInfoSpecification.ErrorMessage);
        }

        ShopName = shopName;
        Currency = new Currency();
        Reputation = new Reputation();
        Day = 1;
    }

    public ShopInfo(string shopName, Currency currency, Reputation reputation, int day)
    {
        ShopInfoSpecification shopInfoSpecification = new ShopInfoSpecification();
        if (!shopInfoSpecification.IsSatisfied(shopName, currency, reputation, day))
        {
            throw new Exception(shopInfoSpecification.ErrorMessage);
        }

        ShopName = shopName;
        Currency = currency;
        Reputation = reputation;
        Day = day;
    }
}
