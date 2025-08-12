using System;
using System.Collections.Generic;
using System.Linq;

[Serializable]
// 저장 데이터가 굉장히 많이 추가될 예정
public class ShopInfo
{
    public string ShopName; // 포션 상점명
    public int SlotIndex; // 포션 상점이 저장될 슬롯 인덱스
    public int Day; // 영업일 수
    public int PotionHouseTier; // 포션 상점 티어
    public Currency Currency; // 재화
    public Reputation Reputation; // 평판
    public Sales Sales; // 판매량
    public List<GridSaveData> GridSaveDataList; // 그리드 가구 배치 정보

    public ShopInfo(string shopName, int slotIndex)
    {
        ShopInfoSpecification shopInfoSpecification = new ShopInfoSpecification();
        if (!shopInfoSpecification.IsSatisfied(shopName, 1))
        {
            throw new Exception(shopInfoSpecification.ErrorMessage);
        }

        ShopName = shopName;
        SlotIndex = slotIndex;
        Day = 1;
        PotionHouseTier = 1;
        Currency = new Currency();
        Reputation = new Reputation();
        Sales = new Sales(0);
        GridSaveDataList = new();
    }

    public ShopInfo(string shopName, int slotIndex, int day, int potionHouseTier, Currency currency, Reputation reputation, Sales sales,
         List<GridSaveData> gridSaveDataList)
    {
        ShopInfoSpecification shopInfoSpecification = new ShopInfoSpecification();
        if (!shopInfoSpecification.IsSatisfied(shopName, day))
        {
            throw new Exception(shopInfoSpecification.ErrorMessage);
        }

        ShopName = shopName;
        SlotIndex = slotIndex;
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
        SlotIndex = shopInfoDto.SlotIndex;
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