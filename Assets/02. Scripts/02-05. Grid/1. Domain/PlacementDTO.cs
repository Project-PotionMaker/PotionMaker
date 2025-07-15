using System.Collections.Generic;
using UnityEngine; 

[System.Serializable]
public class PlacementDTO
{
    public ReadOnlyList<Vector3Int> OccupiedPositions;
    public readonly int TID;
    public readonly int PlacedObjectIndex;

    public PlacementDTO(List<Vector3Int> occupiedPositions, int tid, int placedObjectIndex)
    {
        OccupiedPositions = new ReadOnlyList<Vector3Int>(occupiedPositions);
        TID = tid;
        PlacedObjectIndex = placedObjectIndex;
    }
}
