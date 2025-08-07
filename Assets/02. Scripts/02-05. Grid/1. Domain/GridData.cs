using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// 그리드에 배치된 오브젝트의 정보를 관리하는 클래스입니다.
/// 이 클래스는 서버에서만 사용됩니다.
/// </summary>
public class GridData
{
    private Dictionary<Vector3Int, Placement> _placedObjectDict = new();
    public ReadOnlyList<int> PlacedObjectList => new ReadOnlyList<int>(_placedObjectDict.Values.Select(placement => placement.TID).ToList());
    private Dictionary<Vector3Int, EAreaType> _availableAreaDict;

    private List<Vector3Int> LRUDPositionList = new List<Vector3Int> { new Vector3Int(0,0,1)
                                                                       , new Vector3Int(1,0,0)
                                                                       , new Vector3Int(-1,0,0)
                                                                       , new Vector3Int(0,0,-1) };

    public GridData(Dictionary<Vector3Int, EAreaType> availableAreaDict)
    {
        if (ReferenceEquals(availableAreaDict, null) ||
           (ReferenceEquals(availableAreaDict, null) == false && availableAreaDict.Count == 0))
        {
            throw new Exception("가능한 구역이 존재해야 합니다.");
        }

        _availableAreaDict = availableAreaDict;
    }

    public void AddObjectAt(Vector3Int gridPosition, Vector2Int objectSize, int tid, EStructureType type, GameObject sturctureObject)
    {
        List<Vector3Int> positionToOccupyList = CalculatePositionList(gridPosition, objectSize);
        Placement placement = new Placement(positionToOccupyList, tid, type, sturctureObject);
        foreach (Vector3Int pos in positionToOccupyList)
        {
            if (_placedObjectDict.ContainsKey(pos))
            {
                throw new Exception($"이미 이 위치에 오브젝트가 있습니다. {pos}");
            }

            _placedObjectDict[pos] = placement;
        }
    }

    /// <summary>
    /// 주어진 위치와 크기에 해당하는 모든 그리드 셀 위치를 계산합니다.
    /// 서버와 클라이언트 모두에서 사용됩니다.
    /// </summary>
    public List<Vector3Int> CalculatePositionList(Vector3Int gridPosition, Vector2Int objectSize)
    {
        List<Vector3Int> returnValList = new();
        for (int x = 0; x < objectSize.x; x++)
        {
            for (int y = 0; y < objectSize.y; y++)
            {
                returnValList.Add(gridPosition + new Vector3Int(x, 0, y));
            }
        }
        return returnValList;
    }

    public bool CanPlaceObjectAt(Vector3Int gridPosition, EAreaType StructureType)
    {
        if (_placedObjectDict.ContainsKey(gridPosition))
        {
            return false;
        }

        // 배치하려는 첫 번째 위치의 AreaType을 확인합니다.
        if (_availableAreaDict.ContainsKey(gridPosition) && (_availableAreaDict[gridPosition] != StructureType && _availableAreaDict[gridPosition] != EAreaType.FrontYard))
        {
            return false;
        }
        return true;
    }

    public Placement GetPlacement(Vector3Int gridPosition)
    {
        if (_placedObjectDict.TryGetValue(gridPosition, out Placement placement))
        {
            return placement;
        }
        return null;
    }

    public void RemoveObjectAt(Vector3Int gridPosition)
    {
        if (_placedObjectDict.ContainsKey(gridPosition))
        {
            foreach (Vector3Int pos in _placedObjectDict[gridPosition].OccupiedPositionList)
            {
                _placedObjectDict.Remove(pos);
            }
        }
    }

    public bool CheckLRUDHasArea(Vector3Int gridPosition, EAreaType areaType)
    {
        for(int i = 0; i < LRUDPositionList.Count; i++)
        {
            if(_availableAreaDict.TryGetValue(gridPosition + LRUDPositionList[i], out EAreaType type))
            {
                if(type == areaType)
                {
                    return true;
                }
            }
        }
        return false;
    }
}
