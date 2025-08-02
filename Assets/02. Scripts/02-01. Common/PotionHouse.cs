using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PotionHouse : MonoBehaviourSingleton<PotionHouse>
{
    [SerializeField]
    private int PotionHouseTier;

    // 레이아웃 정보
    [SerializeField]
    private Layout _layout;
    public Layout Layout => _layout;

    // 해금된 전체 정보
    private ReadOnlyDictionary<EUnlockType, ReadOnlyList<int>> _unlockedTIDDict;
    public ReadOnlyDictionary<EUnlockType, ReadOnlyList<int>> UnlockedTIDDict => _unlockedTIDDict;

    // 티어별 해금된 포션 정보
    private ReadOnlyDictionary<int, ReadOnlyList<int>> _unlockedPotionTierDict;
    public ReadOnlyDictionary<int, ReadOnlyList<int>> UnlockedPotionTierDict => _unlockedPotionTierDict;

    public Action OnInitialized;

    private void Start()
    {
        _layout = GameObject.FindGameObjectWithTag("Layout").GetComponent<Layout>();
        Global.Instance.OnDataLoaded += InitPotionHouse;
    }

    private void InitPotionHouse()
    {
        List<UnlockData> unlockDataList = DataTable.Instance.GetUnlockDataList().Where(data => data.Tier == PotionHouseTier).ToList();
        Dictionary<EUnlockType, ReadOnlyList<int>> tempDict = new();
        
        // 전체 데이터 삽입
        foreach(UnlockData unlockData in unlockDataList)
        {
            List<int> tidList = unlockData.TargetTIDs.Split(',')
                                                     .Select(s => int.Parse(s))
                                                     .ToList();
            ReadOnlyList<int> tempList = new ReadOnlyList<int>(tidList);
            tempDict.TryAdd(unlockData.UnlockType, tempList);
        }
        _unlockedTIDDict = new ReadOnlyDictionary<EUnlockType, ReadOnlyList<int>>(tempDict);

        // 포션 데이터 삽입
        Dictionary<int, List<int>> potionDict = new();
        for(int i = 1; i <= PotionHouseTier; i++)
        {
            potionDict.Add(i, new List<int>());
        }
        
        foreach(int potionTID in _unlockedTIDDict[EUnlockType.Potion])
        {
            int tier = DataTable.Instance.GetPotionData(potionTID).Tier;
            potionDict[tier].Add(potionTID);
        }

        Dictionary<int, ReadOnlyList<int>> tempUnlockedPotionTierDict = new();
        for(int i = 1; i <= PotionHouseTier; i++)
        {
            tempUnlockedPotionTierDict.Add(i, new ReadOnlyList<int>(potionDict[i]));
        }

        _unlockedPotionTierDict = new ReadOnlyDictionary<int, ReadOnlyList<int>>(tempUnlockedPotionTierDict);

        OnInitialized?.Invoke();
    }
}
