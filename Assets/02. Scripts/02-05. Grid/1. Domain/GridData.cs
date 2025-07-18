using System;
using System.Collections.Generic;
using UnityEngine;

public class GridData
{
    private Dictionary<Vector3Int, Placement> _placedObjectDict = new();
    private Dictionary<Vector3Int, EAreaType> _availableAreaDict;

    public GridData(Dictionary<Vector3Int, EAreaType> availableAreaDict)
    {
        if(ReferenceEquals(availableAreaDict, null) ||
            (ReferenceEquals(availableAreaDict, null) == false && availableAreaDict.Count == 0))
        {
            throw new Exception("가능한 구역이 존재해야 합니다.");
        }

        _availableAreaDict = availableAreaDict;
    }

    public void AddObjectAt(Vector3Int gridPosition, Vector2Int objectSize, int tid, EStructureType type, int placedObjectIndex)
    {
        List<Vector3Int> positionToOccupyList = CalculatePositions(gridPosition, objectSize);
        Placement placement = new Placement(positionToOccupyList, tid, type, placedObjectIndex);
        foreach(Vector3Int pos in positionToOccupyList)
        {
            if (_placedObjectDict.ContainsKey(pos))
            {
                throw new Exception($"이미 이 위치에 오브젝트가 있습니다. {pos}");
            }

            _placedObjectDict[pos] = placement;
        }
    }

    private List<Vector3Int> CalculatePositions(Vector3Int gridPosition, Vector2Int objectSize)
    {
        List<Vector3Int> returnValList = new();
        for(int x = 0; x < objectSize.x; x++)
        {
            for(int y = 0; y < objectSize.y; y++)
            {
                returnValList.Add(gridPosition + new Vector3Int(x, 0, y));
            }
        }
        return returnValList;
    }

    public bool CanPlaceObjectAt(Vector3Int gridPosition, Vector2Int objectSize, EAreaType StructureType)
    {
        List<Vector3Int> positionToOccupyList = CalculatePositions(gridPosition, objectSize);
        foreach(Vector3Int pos in positionToOccupyList)
        {
            if (_placedObjectDict.ContainsKey(pos))
            {
                return false;
            }
        }
        if (_availableAreaDict.ContainsKey(gridPosition) && _availableAreaDict[gridPosition] != StructureType)
        {
            return false;
        }

        return true;
    }

    public Placement GetPlacement(Vector3Int gridPosition)
    {
        if(_placedObjectDict.TryGetValue(gridPosition, out Placement placement))
        {
            return placement;
        }
        return null;
    }

    public int GetRepresentationIndex(Vector3Int gridPosition)
    {
        if(_placedObjectDict.TryGetValue(gridPosition, out Placement value))
        {
            return value.PlacedObjectIndex;
        }
        return -1;
    }

    public void RemoveObjectAt(Vector3Int gridPosition)
    {
        foreach(Vector3Int pos in _placedObjectDict[gridPosition].OccupiedPositionList)
        {
            _placedObjectDict.Remove(pos);
        }
    }

    public GridDataDTO ToDTO()
    {
        return new GridDataDTO(_placedObjectDict, _availableAreaDict);
    }
}
