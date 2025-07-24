using System;
using System.Collections.Generic;
using UnityEngine;

public class DailyPotionPicker
{
    // [티어][포션 인덱스]
    private List<List<PotionData>> _potionDataList;

    public DailyPotionPicker()
    {
        InitDailyPotionPicker();
    }

    private void InitDailyPotionPicker()
    {
        int tierCount = Enum.GetValues(typeof(ETierType)).Length;
        _potionDataList = new List<List<PotionData>>(tierCount + 1);
        InitTierPotionList(tierCount);
    }

    private void InitTierPotionList(int maxTier)
    {
        var potionDataList = DataTable.Instance.GetPotionDataList();
        for (int currentTier = 1; currentTier <= maxTier; currentTier++)
        {
            foreach (var potionData in potionDataList)
            {
                if (potionData.Tier == currentTier)
                {
                    _potionDataList[currentTier].Add(potionData);
                }
            }
        }
    }

    public List<PotionData> PickDailyPotion(List<int> potionTierList)
    {
        List<PotionData> dailyPotionDataList = new List<PotionData>();
        foreach (int potionTier in potionTierList)
        {
            dailyPotionDataList.Add(PickPotion(potionTier));
        }
        return dailyPotionDataList;
    }

    public List<PotionData> PickDailyPotion(List<ETierType> potionTierList)
    {
        List<PotionData> dailyPotionDataList = new List<PotionData>();
        foreach (ETierType potionTier in potionTierList)
        {
            dailyPotionDataList.Add(PickPotion((int)potionTier + 1));
        }
        return dailyPotionDataList;
    }


    private PotionData PickPotion(int tier)
    {
        int randomPotionIndex = UnityEngine.Random.Range(0, _potionDataList[tier].Count);
        return _potionDataList[tier][randomPotionIndex];
    }
}
