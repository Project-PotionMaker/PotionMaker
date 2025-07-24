using System;
using System.Collections.Generic;
using UnityEngine;

public class PotionPickInfo
{
    public PotionData Data;
    public bool IsPicked;

    public PotionPickInfo(PotionData data)
    {
        Data = data;
        IsPicked = false;
    }
}

public class DailyPotionPicker
{
    private Dictionary<ETierType, List<PotionPickInfo>> _potionDataDict = new Dictionary<ETierType, List<PotionPickInfo>>();

    public DailyPotionPicker()
    {
        InitDailyPotionPicker();
    }

    private void InitDailyPotionPicker()
    {
        int tierCount = Enum.GetValues(typeof(ETierType)).Length;
        InitTierPotionDict(tierCount);
    }

    private void InitTierPotionDict(int maxTier)
    {
        var potionDataList = DataTable.Instance.GetPotionDataList();
        for (int currentTier = 0; currentTier < maxTier; currentTier++)
        {
            var tier = (ETierType)currentTier;
            _potionDataDict.Add(tier, new List<PotionPickInfo>());

            foreach (var potionData in potionDataList)
            {
                if (potionData.Tier == currentTier)
                {
                    _potionDataDict[tier].Add(new PotionPickInfo(potionData));
                }
            }
        }
    }

    public List<PotionData> PickDailyPotion(List<int> potionTierList)
    {
        ResetIsPicked();
        List<PotionData> dailyPotionDataList = new List<PotionData>();
        
        foreach (int potionTier in potionTierList)
        {
            if (potionTier <= 0 || Enum.GetValues(typeof(ETierType)).Length <= potionTier)
            {
                Debug.LogWarning($"올바르지 않은 티어 값입니다. 1 ~ 3 사이의 티어값인지 확인해주세요.");
            }
            dailyPotionDataList.Add(PickPotion((ETierType)potionTier - 1));
        }
        return dailyPotionDataList;
    }

    public List<PotionData> PickDailyPotion(List<ETierType> potionTierList)
    {
        ResetIsPicked();
        List<PotionData> dailyPotionDataList = new List<PotionData>();
        foreach (ETierType potionTier in potionTierList)
        {
            dailyPotionDataList.Add(PickPotion(potionTier));
        }
        return dailyPotionDataList;
    }

    private PotionData PickPotion(ETierType tier)
    {
        var potionList = _potionDataDict[tier];
        while (true)
        {
            int randomIndex = UnityEngine.Random.Range(0, potionList.Count);
            var pickInfo = potionList[randomIndex];

            if (!pickInfo.IsPicked)
            {
                pickInfo.IsPicked = true;
                return pickInfo.Data;
            }
        }
    }

    private void ResetIsPicked()
    {
        foreach (var keyValuePair in _potionDataDict)
        {
            var potionPickInfoList = keyValuePair.Value;
            foreach (var pickInfo in potionPickInfoList)
            {
                pickInfo.IsPicked = false;
            }
        }
    }
}
