using UnityEngine;

public class GridSaveData
{
    public Vector3Int GridPosition;
    public int StructureTID;
    public int IngredientTID;

    public GridSaveData(Vector3Int gridPosition, int strucutreTID, int ingredientTID)
    {
        GridPosition = gridPosition;
        StructureTID = strucutreTID;
        IngredientTID = ingredientTID;
    }

    public GridSaveData(GridSaveDataDTO gridSaveDataDto)
    {
        GridPosition = gridSaveDataDto.GridPosition;
        StructureTID = gridSaveDataDto.StructureTID;
        IngredientTID = gridSaveDataDto.IngredientTID;
    }

    public GridSaveDataDTO ToDTO()
    {
        return new GridSaveDataDTO(this);
    }
}
