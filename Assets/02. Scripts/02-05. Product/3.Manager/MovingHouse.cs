using Mirror;
using System;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MovingHouse
{
    private Delivery _delivery;
    private List<int> _structureTIDList;

    public void InitMovingHouse(Delivery delivery)
    {
        _structureTIDList = new List<int>();
        _delivery = delivery;
    }

    public void MoveHouse(int layoutTID)
    {
        string sceneName = DataTable.Instance.GetLayoutData(layoutTID).SceneName;

        // GridManager에서 받아올거임
        _structureTIDList = GridManager.Instance.GetPlacedStructureTIDList().ToList();
        UnlockManager.Instance.SaveUnlockedData();

        UnlockManager.Instance.OnListUpdated += OnHouseMoved;
        MirrorNetworkManager.Instance.ServerChangeScene(sceneName);
    }

    private void OnHouseMoved()
    {
        UnlockManager.Instance.OnListUpdated -= OnHouseMoved;

        // 재배치
        //List<int> unlockedIngredientTIDList = new List<int>() { 10000, 10001 }; // 임시 이미 해금
        RelocateStructures();

        if (UnlockManager.Instance.PreviousUnlockedTIDDict.TryGetValue(EUnlockType.Ingredient, out ReadOnlyList<int> unlockedIngredientTIDList))
        {
            RelocateStorages(unlockedIngredientTIDList);
        }

        // 새로운 해금 배달
        //List<int> newlyUnlockedStructureTIDList = new List<int>() { 10009, 10017 }; // 임시 해금 가구/조리기구
        //List<int> newlyUnlockedIngredientTIDList = new List<int>() { 10002, 10003, 20000, 30000 }; // 임시 해금 재료

        if(UnlockManager.Instance.NewUnlockedTIDDict.TryGetValue(EUnlockType.Structure, out ReadOnlyList<int> newlyUnlockedStructureTIDList))
        {
            DeliverUnlockedStructures(newlyUnlockedStructureTIDList);
        }
        if (UnlockManager.Instance.NewUnlockedTIDDict.TryGetValue(EUnlockType.Ingredient, out ReadOnlyList<int> newlyUnlockedIngredientTIDList))
        {
            DeliverUnlockedStorages(newlyUnlockedIngredientTIDList);
        }
    }
    private void RelocateStructures()
    {
        Dictionary<EAreaType, List<int>> structureDict = new Dictionary<EAreaType, List<int>>();

        foreach (int structureTID in _structureTIDList)
        {
            EAreaType areaType = DataTable.Instance.GetStructureData(structureTID).AreaType;
            if (!structureDict.ContainsKey(areaType))
            {
                structureDict[areaType] = new List<int>();
            }
            structureDict[areaType].Add(structureTID);
        }

        foreach (EAreaType areaType in structureDict.Keys)
        {
            if (areaType == EAreaType.None || areaType == EAreaType.Storage)
            {
                continue;
            }
            ReadOnlyList<Vector3Int> locatePosition = GridManager.Instance.GetPositionByAreaType(areaType);

            if (locatePosition == null)
            {
                throw new Exception($"{nameof(areaType)} is not set in the Layout");
            }

            _delivery.DeliverStructures(new ReadOnlyList<int>(structureDict[areaType]), areaType);
        }
    }
    private void DeliverUnlockedStructures(ReadOnlyList<int> structureTIDList) => _delivery.DeliverStructures(structureTIDList, EAreaType.FrontYard);
    private void RelocateStorages(ReadOnlyList<int> ingredientTIDList) => _delivery.DeliverStorages(ingredientTIDList, EAreaType.Storage);
    private void DeliverUnlockedStorages(ReadOnlyList<int> ingredientTIDList) => _delivery.DeliverStorages(ingredientTIDList, EAreaType.FrontYard);
}
