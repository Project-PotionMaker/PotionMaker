using System;
using System.Collections.Generic;
using UnityEngine; 

[Serializable]
public class PlacementDTO
{
    public readonly List<Vector3Int> OccupiedPositions;
    public readonly int TID;
    public readonly int PlacedObjectIndex;

    public PlacementDTO(List<Vector3Int> occupiedPositions, int tid, int placedObjectIndex)
    {
        OccupiedPositions = occupiedPositions;
        TID = tid;
        PlacedObjectIndex = placedObjectIndex;
    }
}
