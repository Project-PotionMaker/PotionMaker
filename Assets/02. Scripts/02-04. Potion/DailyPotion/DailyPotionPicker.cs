using NUnit.Framework;
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
    public event Action<List<PotionData>> OnPickCompleted;

    private Dictionary<ETierType, List<PotionPickInfo>> _potionDataDict;

    private Dictionary<EReputationGrade, List<List<int>>> _dailyPotionsInfoByReputation;

    private const int _pickCountLimit = 100;
    public DailyPotionPicker()
    {
        InitDailyPotionPicker();
    }

    private void InitDailyPotionPicker()
    {
        InitTierPotionDict();
    }

    private void InitDailyPotionsInfoByReputation()
    {
        // 평판 등급별로 각 상점 티어(1~3)에 등장할 포션 티어 정보
        _dailyPotionsInfoByReputation = new Dictionary<EReputationGrade, List<List<int>>>
        {
            [EReputationGrade.VeryBad] = new List<List<int>>
        {
            new List<int>(),
            new List<int> {1, 1, 1},          // 1티어 상점
            new List<int> {1, 1, 1, 2},       // 2티어 상점
            new List<int> {1, 1, 1, 2, 3},    // 3티어 상점
        },
            [EReputationGrade.Bad] = new List<List<int>>
        {
            new List<int>(),
            new List<int> {1, 1, 1},
            new List<int> {1, 1, 2, 2},
            new List<int> {1, 1, 2, 2, 3},
        },
            [EReputationGrade.Normal] = new List<List<int>>
        {
            new List<int>(),
            new List<int> {1, 1, 1},
            new List<int> {1, 1, 2, 2},
            new List<int> {1, 1, 2, 3, 3},
        },
            [EReputationGrade.Good] = new List<List<int>>
        {
            new List<int>(),
            new List<int> {1, 1, 1},
            new List<int> {1, 1, 2, 2},
            new List<int> {1, 2, 2, 3, 3},
        },
            [EReputationGrade.Excellent] = new List<List<int>>
        {
            new List<int>(),
            new List<int> {1, 1, 1},
            new List<int> {1, 2, 2, 2},
            new List<int> {1, 2, 3, 3, 3},
        },
        };
    }

    private void InitTierPotionDict()
    {
        _potionDataDict = new Dictionary<ETierType, List<PotionPickInfo>>();
        var potionDataList = DataTable.Instance.GetPotionDataList();
        foreach (ETierType currentTier in Enum.GetValues(typeof(ETierType)))
        {
            _potionDataDict.Add(currentTier, new List<PotionPickInfo>());
            foreach (var potionData in potionDataList)
            {
                if (potionData.Tier == (int)currentTier + 1)
                {
                    _potionDataDict[currentTier].Add(new PotionPickInfo(potionData));
                }
            }
        }
    }

    public List<PotionData> PickDailyPotion(int tier)
    {
        ResetIsPicked();

        List<int> potionTierList = GetPotionTierList(tier);
        List<PotionData> dailyPotionDataList = new List<PotionData>();
        foreach (int potionTier in potionTierList)
        {
            if (potionTier < 1 || Enum.GetValues(typeof(ETierType)).Length < potionTier)
            {
                Debug.LogWarning($"올바르지 않은 티어 값입니다. 1 ~ 3 사이의 티어값인지 확인해주세요.");
            }
            dailyPotionDataList.Add(PickPotion((ETierType)potionTier - 1));
        }
        OnPickCompleted?.Invoke(dailyPotionDataList);
        return dailyPotionDataList;
    }

    public List<PotionData> PickDailyPotion(ETierType tier)
    {
        ResetIsPicked();

        List<int> potionTierList = GetPotionTierList((int)tier + 1);
        List<PotionData> dailyPotionDataList = new List<PotionData>();
        foreach (int potionTier in potionTierList)
        {
            if (potionTier < 1 || Enum.GetValues(typeof(ETierType)).Length < potionTier)
            {
                Debug.LogWarning($"올바르지 않은 티어 값입니다. 1 ~ 3 사이의 티어값인지 확인해주세요.");
            }
            dailyPotionDataList.Add(PickPotion((ETierType)potionTier - 1));
        }
        OnPickCompleted?.Invoke(dailyPotionDataList);
        return dailyPotionDataList;
    }

    private List<int> GetPotionTierList(int tier)
    {
        return _dailyPotionsInfoByReputation
            [ReputationManager.Instance.Reputation.ReputationGrade][tier];
    }

    private PotionData PickPotion(ETierType tier)
    {
        var potionPickInfoList = _potionDataDict[tier];

        int currentPickCount = 0;
        while (currentPickCount++ < _pickCountLimit)
        {
            int randomIndex = UnityEngine.Random.Range(0, potionPickInfoList.Count);
            var pickInfo = potionPickInfoList[randomIndex];

            if (!pickInfo.IsPicked)
            {
                pickInfo.IsPicked = true;
                return pickInfo.Data;
            }
        }

        foreach (var pickInfo in _potionDataDict[tier])
        {
            if (!pickInfo.IsPicked)
            {
                pickInfo.IsPicked = true;
                return pickInfo.Data;
            }
        }
        Debug.LogWarning("뽑을 수 있는 포션이 존재하지 않습니다.");
        return null;
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
