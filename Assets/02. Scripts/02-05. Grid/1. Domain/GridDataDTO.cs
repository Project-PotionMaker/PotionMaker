using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GridDataDTO
{
    public ReadOnlyDictionary<Vector3Int, PlacementDTO> _placedObjectDict;
    public ReadOnlyDictionary<Vector3Int, EAreaType> _availableAreaDict;
    public GridDataDTO(Dictionary<Vector3Int, Placement> placedObjectDict, Dictionary<Vector3Int, EAreaType> availableAreaDict)
    {
        Dictionary<Vector3Int, PlacementDTO> temporaryPlacedDict = new();
        foreach (var entry in placedObjectDict)
        {
            temporaryPlacedDict.Add(entry.Key, entry.Value.ToDTO());
        }
        _placedObjectDict = new ReadOnlyDictionary<Vector3Int, PlacementDTO>(temporaryPlacedDict);
        _availableAreaDict = new ReadOnlyDictionary<Vector3Int, EAreaType>(availableAreaDict);
    }
}
