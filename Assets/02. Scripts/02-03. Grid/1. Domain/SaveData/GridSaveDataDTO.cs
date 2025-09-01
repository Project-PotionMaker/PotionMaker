using System;
using UnityEngine;

[Serializable]
public class GridSaveDataDTO
{
    public readonly Vector3Int GridPosition;
    public readonly int StructureTID;
    public readonly int IngredientTID;

    public GridSaveDataDTO(GridSaveData gridSaveData)
    {
        GridPosition = gridSaveData.GridPosition;
        StructureTID = gridSaveData.StructureTID;
        IngredientTID = gridSaveData.IngredientTID;
    }
}
