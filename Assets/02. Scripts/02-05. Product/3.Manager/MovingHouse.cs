using Mono.Cecil;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
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
        RelocateStructure();
        LoadStorage();
    }
    public void RelocateStructure()
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
            List<Vector3Int> locatePosition = GridManager.Instance.GetPositionByAreaType(areaType);

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

    public void LoadStorage()
    {
        List<Vector3Int> relocatePostion = GridManager.Instance.GetPositionByAreaType(EAreaType.Storage);
        List<Vector3Int> newPostion = GridManager.Instance.GetPositionByAreaType(EAreaType.Delivery);

        int index = 0;

        List<int> relocateTIDList = new List<int>() { 10000, 10001, 10002, 10003 };
        List<int> newTIDList = new List<int>() { 10004, 10005, 20000, 30000 };

        foreach (int TID in relocateTIDList)
        {
            switch (DataTable.Instance.GetIngredientData(TID).IngredientType)
            {
                case EIngredientType.Plants:
                {
                    GridManager.Instance.CreateStructure(10018, relocatePostion[index++], TID);
                    break;
                }
                case EIngredientType.Animals:
                {
                    GridManager.Instance.CreateStructure(10019, relocatePostion[index++], TID);
                    break;
                }
                case EIngredientType.Crystals:
                {
                    GridManager.Instance.CreateStructure(10020, relocatePostion[index++], TID);
                    break;
                }
            }
        }

        foreach (int TID in newTIDList)
        {
            switch (DataTable.Instance.GetIngredientData(TID).IngredientType)
            {
                case EIngredientType.Plants:
                {
                    GridManager.Instance.CreateStructure(10018, newPostion[index++], TID);
                    break;
                }
                case EIngredientType.Animals:
                {
                    GridManager.Instance.CreateStructure(10019, newPostion[index++], TID);
                    break;
                }
                case EIngredientType.Crystals:
                {
                    GridManager.Instance.CreateStructure(10020, newPostion[index++], TID);
                    break;
                }
            }
        }
    }
}
