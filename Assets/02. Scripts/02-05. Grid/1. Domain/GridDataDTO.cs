using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class GridDataDTO
{
    public readonly Dictionary<Vector3Int, PlacementDTO> PlacedObjectDict;
    public readonly Dictionary<Vector3Int, EAreaType> AvailableAreaDict;
    public GridDataDTO(Dictionary<Vector3Int, Placement> placedObjectDict, Dictionary<Vector3Int, EAreaType> availableAreaDict)
    {
        Dictionary<Vector3Int, PlacementDTO> temporaryPlacedDict = new();
        foreach (var entry in placedObjectDict)
        {
            temporaryPlacedDict.Add(entry.Key, entry.Value.ToDTO());
        }
        PlacedObjectDict = temporaryPlacedDict;
        AvailableAreaDict = availableAreaDict;
    }

    public GridDataDTO(GridData gridData)
    {
        PlacedObjectDict = gridData.PlacedObjectDict;
        AvailableAreaDict = gridData.AvailableAreaDict;
    }
}
