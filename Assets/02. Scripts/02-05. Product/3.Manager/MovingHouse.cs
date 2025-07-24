using Mono.Cecil;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MovingHouse
{
    private List<int> _structureTIDList;

    private void Awake()
    {
        _structureTIDList = new List<int>();
    }

    public void MoveHouse(int layoutTID)
    {
        string sceneName = DataTable.Instance.GetLayoutData(layoutTID).SceneName;
            
        // GridManager에서 받아올거임
        _structureTIDList.Add(10000);
        SceneManager.sceneLoaded += OnHouseMoved;
        SceneManager.LoadScene(sceneName);
    }

    public void OnHouseMoved(Scene scene, LoadSceneMode mode)
    {
        SceneManager.sceneLoaded -= OnHouseMoved;
        RelocateStructure();
    }
    public void RelocateStructure()
    {
        Dictionary<EAreaType, List<int>> structrueDict = new Dictionary<EAreaType, List<int>>()
        {
            { EAreaType.Hall, new List<int>()},
            { EAreaType.Kitchen, new List<int>()},
            { EAreaType.Storage, new List<int>()},
        };
        foreach (int structureTID in _structureTIDList)
        {
            EAreaType areaType = DataTable.Instance.GetStructureData(structureTID).AreaType;
            structrueDict[areaType].Add(structureTID);
        }

        foreach (EAreaType areaType in Enum.GetValues(typeof(EAreaType)))
        {
            if(areaType == EAreaType.None)
            {
                continue;
            }
            List<Vector3Int> locatePosition = GridManager.Instance.GetPositionByAreaType(areaType);

            if(locatePosition == null)
            {
                throw new Exception($"{nameof(areaType)} is not set in the Layout");
            }

            int index = 0;
            foreach(int structureTID in structrueDict[areaType])
            {
                GridManager.Instance.CreateStructure(structureTID, locatePosition[index++]);
            }
        }
    }
}
