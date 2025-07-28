using System;
using System.Collections.Generic;
using UnityEngine;

public class Delivery
{
    public void DeliverStructure(int structureTID, EAreaType areaType, int startIndex, out int newStartIndex)
    {
        ReadOnlyList<Vector3Int> locatePosition = GridManager.Instance.GetPositionByAreaType(areaType);
        if (locatePosition == null)
        {
            throw new Exception($"{nameof(areaType)} is not set in the Layout");
        }
        for (int i = startIndex; i < locatePosition.Count; i++)
        {
            if (GridManager.Instance.GetObjectOnGrid(locatePosition[i]) == null)
            {
                GridManager.Instance.CreateStructure(structureTID, locatePosition[i]);
                newStartIndex = i + 1;
                return;
            }
        }
        throw new Exception("There is No Available area");
    }

    public void DeliverStorages(List<int> ingredientTIDList, EAreaType areaType)
    {
        ReadOnlyList<Vector3Int> positionList = GridManager.Instance.GetPositionByAreaType(areaType);

        int index = 0;

        foreach (int ingredientTID in ingredientTIDList)
        {
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
            Debug.Log($"{storageStructureTID} 생성 : {positionList[index]}");
            GridManager.Instance.CreateStructure(storageStructureTID, positionList[index++], ingredientTID);
        }
    }
}
