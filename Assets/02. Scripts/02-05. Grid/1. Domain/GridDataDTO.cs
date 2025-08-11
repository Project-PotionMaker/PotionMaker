using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class GridDataDTO
{
    public readonly Dictionary<Vector3Int, PlacementDTO> _placedObjectDict;
    public readonly Dictionary<Vector3Int, EAreaType> _availableAreaDict;
    public GridDataDTO(Dictionary<Vector3Int, Placement> placedObjectDict, Dictionary<Vector3Int, EAreaType> availableAreaDict)
    {
        Dictionary<Vector3Int, PlacementDTO> temporaryPlacedDict = new();
        foreach (var entry in placedObjectDict)
        {
            temporaryPlacedDict.Add(entry.Key, entry.Value.ToDTO());
        }
        _placedObjectDict = temporaryPlacedDict;
        _availableAreaDict = availableAreaDict;
    }

    public GridDataDTO(GridData gridData)
    {
        _placedObjectDict = gridData.PlacedObjectDict;
        _availableAreaDict = gridData.AvailableAreaDict;
    }
}
