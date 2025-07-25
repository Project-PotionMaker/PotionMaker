using Mono.Cecil;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MovingHouse
{
    private List<int> _structureTIDList;

    public void InitMovingHouse()
    {
        _structureTIDList = new List<int>();
    }

    public void MoveHouse(int layoutTID)
    {
        string sceneName = DataTable.Instance.GetLayoutData(layoutTID).SceneName;

        // GridManager에서 받아올거임
        _structureTIDList = GridManager.Instance.GetPlacedStructureTIDList().ToList();


        SceneManager.sceneLoaded += OnHouseMoved;
        SceneManager.LoadScene(sceneName);
    }

    public void OnHouseMoved(Scene scene, LoadSceneMode mode)
    {
        SceneManager.sceneLoaded -= OnHouseMoved;
        // 재배치
        RelocateStructure();
        List<int> unlockedIngredientTIDList = new List<int>() { 10000, 10001, 10002, 10003 }; // 임시 이미 해금
        RelocateStorage(unlockedIngredientTIDList);

        // 새로운 해금 배달        
        List<int> newlyUnlockedStructureTIDList = new List<int>() { 10009, 10017 }; // 임시 해금 가구/조리기구
        List<int> newlyUnlockedIngredientTIDList = new List<int>() { 10004, 10005, 20000, 30000 }; // 임시 해금 재료

        DeliverNewStructure(newlyUnlockedStructureTIDList);
        DeliverNewStorage(newlyUnlockedIngredientTIDList);
    }
    private void RelocateStructure ()
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
                GridManager.Instance.CreateStructure(structureTID, locatePosition[index++]);
            }
        }
    }
    private void DeliverNewStructure(List<int> structureTIDList)
    {
        ReadOnlyList<Vector3Int> positionList = GridManager.Instance.GetPositionByAreaType(EAreaType.Delivery);

    }

    public void RelocateStorage(List<int> ingredientList) => LocateStorage(ingredientList, EAreaType.Storage);
    public void DeliverNewStorage(List<int> ingredientList) => LocateStorage(ingredientList, EAreaType.Delivery);
    private void LocateStorage(List<int> ingredientTIDList, EAreaType areaType )
    {
        ReadOnlyList<Vector3Int> positionList = GridManager.Instance.GetPositionByAreaType(areaType);

        int index = 0;

        foreach (int TID in ingredientTIDList)
        {
            int storageStructureTID;
            switch (DataTable.Instance.GetIngredientData(TID).IngredientType)
            {
                case EIngredientType.Plants:
                {
                    storageStructureTID = 10018;
                    break;
                }
                case EIngredientType.Animals:
                {
                    storageStructureTID = 10019;
                    break;
                }
                case EIngredientType.Crystals:
                {
                    storageStructureTID = 10020;
                    break;
                }
                default:
                {
                    storageStructureTID = -1;
                    break;
                }
            }
            GridManager.Instance.CreateStructure(storageStructureTID, positionList[index++], TID);
        }
    }
}
