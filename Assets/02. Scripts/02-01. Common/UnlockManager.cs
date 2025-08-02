using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.SceneManagement;

// MonoBehaviourSingleton을 상속받아 씬이 바뀌어도 파괴되지 않는 싱글톤 매니저로 만듭니다.
public class UnlockManager : MonoBehaviourSingleton<UnlockManager>
{
    private ReadOnlyDictionary<EUnlockType, ReadOnlyList<int>> _previousUnlockedTIDDict;

    private ReadOnlyDictionary<EUnlockType, ReadOnlyList<int>> _newUnlockedTIDDict;
    public ReadOnlyDictionary<EUnlockType, ReadOnlyList<int>> NewUnlockedTIDDict => _newUnlockedTIDDict;

    public bool HasPreviousData => _previousUnlockedTIDDict != null;

    protected override void Awake()
    {
        base.Awake();
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if(PotionHouse.Instance != null)
        {
            PotionHouse.Instance.OnInitialized -= CheckForNewUnlocks;
            PotionHouse.Instance.OnInitialized += CheckForNewUnlocks;
        }
    }

    public void SaveUnlockedData(ReadOnlyDictionary<EUnlockType, ReadOnlyList<int>> dataToSave)
    {
        _previousUnlockedTIDDict = dataToSave;
    }

    private void CheckForNewUnlocks()
    {
        ReadOnlyDictionary<EUnlockType, ReadOnlyList<int>> currentDataDict = PotionHouse.Instance.UnlockedTIDDict;
        Dictionary<EUnlockType, List<int>> tempNewUnlockDict = new Dictionary<EUnlockType, List<int>>();

        if (!HasPreviousData)
        {
            foreach (var kvp in currentDataDict)
            {
                tempNewUnlockDict.Add(kvp.Key, kvp.Value.ToList());
            }
        }
        else
        {
            foreach (var currentKvp in currentDataDict)
            {
                EUnlockType currentUnlockType = currentKvp.Key;
                ReadOnlyList<int> currentTIDList = currentKvp.Value;

                if (_previousUnlockedTIDDict.TryGetValue(currentUnlockType, out ReadOnlyList<int> previousTIDList))
                {
                    List<int> newlyUnlockedTIDList = currentTIDList.Except(previousTIDList).ToList();

                    if (newlyUnlockedTIDList.Count > 0)
                    {
                        tempNewUnlockDict.Add(currentUnlockType, newlyUnlockedTIDList);
                    }
                }
                else
                {
                    tempNewUnlockDict.Add(currentUnlockType, currentTIDList.ToList());
                }
            }
        }

        _newUnlockedTIDDict = new ReadOnlyDictionary<EUnlockType, ReadOnlyList<int>>(
            tempNewUnlockDict.ToDictionary(
                kvp => kvp.Key,
                kvp => new ReadOnlyList<int>(kvp.Value)
            )
        );

        PotionHouse.Instance.OnInitialized -= CheckForNewUnlocks;
    }
}
