using System;
using System.Collections.Generic;
using UnityEngine;

public class Delivery
{
    public void DeliverStructure(int structureTID, EAreaType areaType)
    {
        ReadOnlyList<Vector3Int> positionList = GridManager.Instance.GetPositionByAreaType(areaType);
        if (positionList == null)
        {
            throw new Exception($"{nameof(areaType)} is not set in the Layout");
        }
        for (int i = 0; i < positionList.Count; i++)
        {
            if (GridManager.Instance.GetObjectOnGrid(positionList[i]) == null)
            {
                GridManager.Instance.CreateStructure(structureTID, positionList[i]);
                return;
            }
        }
        throw new Exception("There is No Available area");
    }
    public void DeliverStructure(int structureTID, EAreaType areaType, int startIndex, out int newStartIndex)
    {
        ReadOnlyList<Vector3Int> positionList = GridManager.Instance.GetPositionByAreaType(areaType);
        if (positionList == null)
        {
            throw new Exception($"{nameof(areaType)} is not set in the Layout");
        }
        for (int i = startIndex; i < positionList.Count; i++)
        {
            if (GridManager.Instance.GetObjectOnGrid(positionList[i]) == null)
            {
                GridManager.Instance.CreateStructure(structureTID, positionList[i]);
                newStartIndex = i + 1;
                return;
            }
        }
        throw new Exception("There is No Available area");
    }

    public void DeliverStructures(List<int> structureTIDList, EAreaType areaType)
    {
        int index = 0;
        foreach (int structureTID in structureTIDList)
        {
            DeliverStructure(structureTID, areaType, index, out index);
        }
    }

    public void DeliverStorage(int ingredientTID, EAreaType areaType)
    {
        ReadOnlyList<Vector3Int> positionList = GridManager.Instance.GetPositionByAreaType(areaType);
        if (positionList == null)
        {
            throw new Exception($"{nameof(areaType)} is not set in the Layout");
        }

        int storageStructureTID;
        switch (DataTable.Instance.GetIngredientData(ingredientTID).IngredientType)
        {
            case EIngredientType.Plants:
            {
                storageStructureTID = 10018;
                break;
            }
            case EIngredientType.Animals:
            {
                storageStructureTID = 10019;
                break;
            }
            case EIngredientType.Crystals:
            {
                storageStructureTID = 10020;
                break;
            }
            default:
            {
                storageStructureTID = -1;
                break;
            }
        }

        for (int i = 0; i < positionList.Count; i++)
        {
            if (GridManager.Instance.GetObjectOnGrid(positionList[i]) == null)
            {
                GridManager.Instance.CreateStructure(storageStructureTID, positionList[i], ingredientTID);
                return;
            }
        }
        throw new Exception("There is No Available area");
    }
    public void DeliverStorage(int ingredientTID, EAreaType areaType, int startIndex, out int newStartIndex)
    {
        ReadOnlyList<Vector3Int> positionList = GridManager.Instance.GetPositionByAreaType(areaType);
        if (positionList == null)
        {
            throw new Exception($"{nameof(areaType)} is not set in the Layout");
        }

        int storageStructureTID;
        switch (DataTable.Instance.GetIngredientData(ingredientTID).IngredientType)
        {
            case EIngredientType.Plants:
            {
                storageStructureTID = 10018;
                break;
            }
            case EIngredientType.Animals:
            {
                storageStructureTID = 10019;
                break;
            }
            case EIngredientType.Crystals:
            {
                storageStructureTID = 10020;
                break;
            }
            default:
            {
                storageStructureTID = -1;
                break;
            }
        }
        
        for (int i = startIndex; i < positionList.Count; i++)
        {
            if (GridManager.Instance.GetObjectOnGrid(positionList[i]) == null)
            {
                GridManager.Instance.CreateStructure(storageStructureTID, positionList[i], ingredientTID);
                newStartIndex = i + 1;
                return;
            }
        }
        throw new Exception("There is No Available area");
    }

    public void DeliverStorages(List<int> ingredientTIDList, EAreaType areaType)
    {
        int index = 0;
        foreach (int ingredientTID in ingredientTIDList)
        {
            DeliverStructure(ingredientTID, areaType, index, out index);
        }
    }
}
