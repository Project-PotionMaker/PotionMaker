using System;
using System.Collections.Generic;
using System.Linq;
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
    public GridData GridData; // 그리드 가구 배치정보
    public int Day;

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
        GridData = new GridData();
        Sales = new Sales(0);
        Day = 1;
    }

    public ShopInfo(string shopName, Currency currency, Reputation reputation, Sales sales,
         GridData gridData, int day)
    {
        ShopInfoSpecification shopInfoSpecification = new ShopInfoSpecification();
        if (!shopInfoSpecification.IsSatisfied(shopName, currency, reputation, day))
        {
            throw new Exception(shopInfoSpecification.ErrorMessage);
        }

        ShopName = shopName;
        Currency = currency;
        Reputation = reputation;
        GridDeploymentDict = gridDeploymentDict;
        Sales = sales;
        Day = day;
    }

    public ShopInfo(ShopInfoDTO shopInfoDto)
    {
        ShopName = shopInfoDto.ShopName;
        Currency = new Currency(shopInfoDto.Currency);
        Reputation = new Reputation(shopInfoDto.Reputation);
        Sales = new Sales(shopInfoDto.Sales);
        GridDeploymentDict = shopInfoDto.GridDeploymentDict;
        Day = shopInfoDto.Day;
    }

    public ShopInfoDTO ToDTO()
    {
        return new ShopInfoDTO(this);
    }
}
