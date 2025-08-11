using System.Collections.Generic;
using UnityEngine;

public class Placement
{
    public List<Vector3Int> OccupiedPositionList;
    public int TID { get; private set; }
    public EStructureType StructureType { get; private set; }
    public GameObject StructureObject { get; private set; }
    public int IngredientTID { get; private set; }
    public Placement(List<Vector3Int> occupiedPositionList, int tid, EStructureType type, GameObject structureObject, int ingredientTID = 0)
    {
        if (tid < 10000)
        {
            throw new System.Exception("TID가 올바르지 않습니다.");
        }

        OccupiedPositionList = occupiedPositionList;
        TID = tid;
        StructureType = type;
        StructureObject = structureObject;
        IngredientTID = ingredientTID;
    }

    public Placement(PlacementDTO placemenmtDto)
    {
        OccupiedPositionList = placemenmtDto.OccupiedPositions;
        TID = placemenmtDto.TID;
        StructureType = placemenmtDto.StructureType;
        IngredientTID = placemenmtDto.IngredientTID;
    }

    public PlacementDTO ToDTO()
    {
        return new PlacementDTO(OccupiedPositionList, TID, StructureType, IngredientTID);
    }
}
