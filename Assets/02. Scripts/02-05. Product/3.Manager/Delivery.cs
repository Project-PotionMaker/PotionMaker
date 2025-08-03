using Mirror;
using System;
using System.Collections.Generic;
using UnityEngine;

public class Delivery
{
    /// <summary>
    /// 지정된 영역에 구조물 하나를 배치합니다.
    /// 이 함수는 서버에서만 실행되어야 합니다.
    /// </summary>
    /// <param name="structureTID">배치할 구조물의 TID</param>
    /// <param name="areaType">구조물을 배치할 영역의 타입</param>
    public void DeliverStructure(int structureTID, EAreaType areaType)
    {
        ReadOnlyList<Vector3Int> positionList = GridManager.Instance.GetPositionByAreaType(areaType);
        if (positionList == null)
        {
            throw new Exception($"{nameof(areaType)} is not set in the Layout");
        }
        for (int i = 0; i < positionList.Count; i++)
        {
            if (GridManager.Instance.ServerGetObjectOnGrid(positionList[i]) == null)
            {
                GridManager.Instance.ServerCreateStructure(structureTID, positionList[i]);
                return;
            }
        }
        throw new Exception("There is No Available area");
    }

    /// <summary>
    /// 지정된 영역의 특정 인덱스부터 시작하여 구조물 하나를 배치합니다.
    /// 이 함수는 서버에서만 실행되어야 합니다.
    /// </summary>
    /// <param name="structureTID">배치할 구조물의 TID</param>
    /// <param name="areaType">구조물을 배치할 영역의 타입</param>
    /// <param name="startIndex">탐색을 시작할 인덱스</param>
    /// <param name="newStartIndex">다음 탐색을 시작할 인덱스 (out)</param>
    private void DeliverStructure(int structureTID, EAreaType areaType, int startIndex, out int newStartIndex)
    {
        ReadOnlyList<Vector3Int> positionList = GridManager.Instance.GetPositionByAreaType(areaType);
        if (positionList == null)
        {
            throw new Exception($"{nameof(areaType)} is not set in the Layout");
        }
        for (int i = startIndex; i < positionList.Count; i++)
        {
            if (GridManager.Instance.ServerGetObjectOnGrid(positionList[i]) == null)
            {
                GridManager.Instance.ServerCreateStructure(structureTID, positionList[i]);
                newStartIndex = i + 1;
                return;
            }
        }
        throw new Exception("There is No Available area");
    }

    /// <summary>
    /// 여러 개의 구조물을 지정된 영역에 순서대로 배치합니다.
    /// 이 함수는 서버에서만 실행되어야 합니다.
    /// </summary>
    /// <param name="structureTIDList">배치할 구조물 TID 목록</param>
    /// <param name="areaType">구조물을 배치할 영역의 타입</param>
    public void DeliverStructures(List<int> structureTIDList, EAreaType areaType)
    {
        int index = 0;
        foreach (int structureTID in structureTIDList)
        {
            DeliverStructure(structureTID, areaType, index, out index);
        }
    }

    /// <summary>
    /// 지정된 영역에 저장소(Storage)를 하나 배치합니다.
    /// 이 함수는 서버에서만 실행되어야 합니다.
    /// </summary>
    /// <param name="ingredientTID">저장소에 담을 재료의 TID</param>
    /// <param name="areaType">저장소를 배치할 영역의 타입</param>
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
            if (GridManager.Instance.ServerGetObjectOnGrid(positionList[i]) == null)
            {
                GridManager.Instance.ServerCreateStructure(storageStructureTID, positionList[i], ingredientTID);
                return;
            }
        }
        throw new Exception("There is No Available area");
    }

    /// <summary>
    /// 지정된 영역의 특정 인덱스부터 시작하여 저장소(Storage)를 하나 배치합니다.
    /// 이 함수는 서버에서만 실행되어야 합니다.
    /// </summary>
    /// <param name="ingredientTID">저장소에 담을 재료의 TID</param>
    /// <param name="areaType">저장소를 배치할 영역의 타입</param>
    /// <param name="startIndex">탐색을 시작할 인덱스</param>
    /// <param name="newStartIndex">다음 탐색을 시작할 인덱스 (out)</param>
    private void DeliverStorage(int ingredientTID, EAreaType areaType, int startIndex, out int newStartIndex)
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
            if (GridManager.Instance.ServerGetObjectOnGrid(positionList[i]) == null)
            {
                GridManager.Instance.ServerCreateStructure(storageStructureTID, positionList[i], ingredientTID);
                newStartIndex = i + 1;
                return;
            }
        }
        throw new Exception("There is No Available area");
    }

    /// <summary>
    /// 여러 개의 저장소를 지정된 영역에 순서대로 배치합니다.
    /// 이 함수는 서버에서만 실행되어야 합니다.
    /// </summary>
    /// <param name="ingredientTIDList">저장소에 담을 재료 TID 목록</param>
    /// <param name="areaType">저장소를 배치할 영역의 타입</param>
    public void DeliverStorages(List<int> ingredientTIDList, EAreaType areaType)
    {
        int index = 0;
        foreach (int ingredientTID in ingredientTIDList)
        {
            DeliverStorage(ingredientTID, areaType, index, out index);
        }
    }
}
