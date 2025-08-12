using System;
using System.Collections.Generic;
using System.Linq;

[Serializable]
// 저장 데이터가 굉장히 많이 추가될 예정
public class ShopInfo
{
    public string ShopName;
    public int Day;
    public int PotionHouseTier;
    public Currency Currency;
    public Reputation Reputation;
    public Sales Sales;
    public List<GridSaveData> GridSaveDataList;

    public ShopInfo(string shopName)
    {
        ShopInfoSpecification shopInfoSpecification = new ShopInfoSpecification();
        if (!shopInfoSpecification.IsSatisfied(shopName, new Currency(), new Reputation(), 1))
        {
            throw new Exception(shopInfoSpecification.ErrorMessage);
        }

        ShopName = shopName;
        Day = 1;
        PotionHouseTier = 1; // PotionHouseTier 초기값 추가
        Currency = new Currency();
        Reputation = new Reputation();
        Sales = new Sales(0);
        GridSaveDataList = new();
    }

    public ShopInfo(string shopName, int day, int potionHouseTier, Currency currency, Reputation reputation, Sales sales,
          List<GridSaveData> gridSaveDataList)
    {
        ShopInfoSpecification shopInfoSpecification = new ShopInfoSpecification();
        if (!shopInfoSpecification.IsSatisfied(shopName, currency, reputation, day))
        {
            throw new Exception(shopInfoSpecification.ErrorMessage);
        }

        ShopName = shopName;
        Day = day;
        PotionHouseTier = potionHouseTier;
        Currency = currency;
        Reputation = reputation;
        Sales = sales;
        GridSaveDataList = gridSaveDataList;
    }

    public ShopInfo(ShopInfoDTO shopInfoDto)
    {
        ShopName = shopInfoDto.ShopName;
        Day = shopInfoDto.Day;
        PotionHouseTier = shopInfoDto.PotionHouseTier;
        Currency = new Currency(shopInfoDto.Currency);
        Reputation = new Reputation(shopInfoDto.Reputation);
        Sales = new Sales(shopInfoDto.Sales);
        GridSaveDataList = shopInfoDto.GridSaveDataList.Select(gridSaveData => new GridSaveData(gridSaveData)).ToList();
    }

    public ShopInfoDTO ToDTO()
    {
        return new ShopInfoDTO(this);
    }
}