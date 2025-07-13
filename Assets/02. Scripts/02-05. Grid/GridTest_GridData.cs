using NUnit.Framework;
using System;
using System.Collections.Generic;
using UnityEngine;

public class GridTest_GridData
{
    private Dictionary<Vector3Int, PlacementData> _placedObjectDict = new();

    public void AddObjectAt(Vector3Int gridPosition, Vector2Int objectSize, int id, int placedObjectIndex)
    {
        List<Vector3Int> positionToOccupyList = CalculatePositions(gridPosition, objectSize);
        PlacementData data = new PlacementData(positionToOccupyList, id, placedObjectIndex);
        foreach(var pos in positionToOccupyList)
        {
            if (_placedObjectDict.ContainsKey(pos))
            {
                throw new Exception($"Dictionary already contains this cell position {pos}");
            }
            _placedObjectDict[pos] = data;
        }
    }

    private List<Vector3Int> CalculatePositions(Vector3Int gridPosition, Vector2Int objectSize)
    {
        List<Vector3Int> returnVal = new();
        for (int x = 0; x < objectSize.x; x++)
        {
            for(int y = 0; y < objectSize.y; y++)
            {
                returnVal.Add(gridPosition + new Vector3Int(x, 0, y));
            }
        }
        return returnVal;
    }

    public bool CanPlaceObjectAt(Vector3Int gridPosition, Vector2Int objectSize)
    {
        List<Vector3Int> positionToOccupy = CalculatePositions(gridPosition, objectSize);
        foreach(var pos in positionToOccupy)
        {
            if (_placedObjectDict.ContainsKey(pos))
            {
                return false;
            }
        }
        return true;
    }

    public int getRepresentationIndex(Vector3Int gridPosition)
    {
        if(_placedObjectDict.ContainsKey(gridPosition) == false)
        {
            return -1;
        }
        return _placedObjectDict[gridPosition].PlacedOjectIndex;
    }

    public void RemoveObjectAt(Vector3Int gridPosition)
    {
        foreach(var pos in _placedObjectDict[gridPosition].occupiedPositionList)
        {
            _placedObjectDict.Remove(pos);
        }
    }
}

public class PlacementData
{
    public List<Vector3Int> occupiedPositionList;
    public int ID { get; private set; }
    public int PlacedOjectIndex { get; private set; }

    public PlacementData(List<Vector3Int> occupiedPositionList, int id, int placedOjectIndex)
    {
        this.occupiedPositionList = occupiedPositionList;
        ID = id;
        PlacedOjectIndex = placedOjectIndex;
    }
}