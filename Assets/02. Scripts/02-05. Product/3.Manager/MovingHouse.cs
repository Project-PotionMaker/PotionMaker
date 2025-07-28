using Mono.Cecil;
using NUnit.Framework;
using NUnit.Framework.Constraints;
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


        SceneManager.sceneLoaded += OnHouseMoved;
        SceneManager.LoadScene(sceneName);
    }

    private void OnHouseMoved(Scene scene, LoadSceneMode mode)
    {
        SceneManager.sceneLoaded -= OnHouseMoved;
        // 재배치
        List<int> unlockedIngredientTIDList = new List<int>() { 10000, 10001 }; // 임시 이미 해금
        RelocateStorages(unlockedIngredientTIDList);
        RelocateStructures();

        // 새로운 해금 배달
        List<int> newlyUnlockedIngredientTIDList = new List<int>() { 10002, 10003, 20000, 30000 }; // 임시 해금 재료
        List<int> newlyUnlockedStructureTIDList = new List<int>() { 10009, 10017 }; // 임시 해금 가구/조리기구

        DeliverUnlockedStorages(newlyUnlockedIngredientTIDList);
        DeliverUnlockedStructures(newlyUnlockedStructureTIDList);
    }
    private void RelocateStructures ()
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

            int index = 0;
            foreach (int structureTID in structureDict[areaType])
            {
                _delivery.DeliverStructure(structureTID, areaType, index, out index);
            }
        }
    }
    private void DeliverUnlockedStructures(List<int> structureTIDList)
    {
        int index = 0;
        foreach(int structureTID in structureTIDList)
        {
            _delivery.DeliverStructure(structureTID, EAreaType.FrontYard, index, out index);
        }
    }

    public void RelocateStorages(List<int> ingredientList) => _delivery.DeliverStorages(ingredientList, EAreaType.Storage);
    public void DeliverUnlockedStorages(List<int> ingredientList) => _delivery.DeliverStorages(ingredientList, EAreaType.FrontYard);
   
}
