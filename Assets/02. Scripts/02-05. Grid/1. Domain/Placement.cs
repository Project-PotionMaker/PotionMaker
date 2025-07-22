using NUnit.Framework;
using NUnit.Framework.Constraints;
using System.Collections.Generic;
using UnityEngine;

public class Placement
{
    public List<Vector3Int> OccupiedPositionList;
    public int TID { get; private set; }
    public EStructureType structureType { get; private set; }
    public GameObject StructureObject { get; private set; }
    public Placement(List<Vector3Int> occupiedPositionList, int tid, EStructureType type, GameObject structureObject)
    {
        if (tid < 10000)
        {
            throw new System.Exception("TID가 올바르지 않습니다.");
        }

        OccupiedPositionList = occupiedPositionList;
        TID = tid;
        structureType = type;
        StructureObject = structureObject;
    }

    public PlacementDTO ToDTO()
    {
        //return new PlacementDTO(OccupiedPositionList, TID, PlacedObjectIndex);
        return null;
    }
}
