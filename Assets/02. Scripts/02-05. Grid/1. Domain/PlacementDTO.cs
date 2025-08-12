using System;
using System.Collections.Generic;
using UnityEngine; 

[Serializable]
public class PlacementDTO
{
    public readonly List<Vector3Int> OccupiedPositions;
    public readonly int TID;
    public readonly EStructureType StructureType;
    public readonly int IngredientTID;

    public PlacementDTO(List<Vector3Int> occupiedPositions, int tid, EStructureType structureType,
        int ingredientTID)
    {
        OccupiedPositions = occupiedPositions;
        TID = tid;
        StructureType = structureType;
        IngredientTID = ingredientTID;
    }
}
