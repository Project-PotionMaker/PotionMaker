using Mirror;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 가구 배치 상태를 관리하는 클래스입니다.
/// 이 클래스는 오직 로컬 클라이언트에서만 동작합니다.
/// </summary>
public class PlacementState : IBuildingState
{
    private StructureData _data;
    private Grid _grid;
    private PreviewSystem _previewSystem;
    private GameObject _structure;
    private NetworkIdentity _structureIdentity;

    private Vector2Int _size;
    private Quaternion _rotation;

    public PlacementState(GameObject structure,
                          StructureData data,
                          Grid grid,
                          PreviewSystem previewSystem)
    {
        _structure = structure;
        _structureIdentity = structure.GetComponent<NetworkIdentity>();
        _data = data;
        _grid = grid;
        _previewSystem = previewSystem;

        _size = new Vector2Int(data.Width, data.Length);
        _rotation = structure.transform.rotation;
    }

    /// <summary>
    /// 배치 상태를 시작합니다.
    /// </summary>
    public void StartState()
    {
        _previewSystem.StartShowingPlacementPreview(_data.TID);
    }

    public void EndState()
    {
        _previewSystem.StopShowingPreview();
    }

    /// <summary>
    /// 클라이언트에서 서버로 배치 요청을 보냅니다.
    /// </summary>
    public bool TryAction(Vector3Int gridPosition)
    {
        // 클라이언트에서 먼저 유효성을 확인합니다.
        bool placementValidity = CheckPlacementValidity(gridPosition);
        if (placementValidity == false)
        {
            // 유효하지 않으면 서버에 요청을 보내지 않고 즉시 반환
            return false;
        }

        GridManager.Instance.ServerPlaceStructure(
            gridPosition,
            _structureIdentity.netId,
            NetworkClient.localPlayer.connectionToClient);

        return true;
    }

    /// <summary>
    /// 서버로부터 받은 배치 결과를 처리합니다.
    /// </summary>
    public void ReceivePlaceResult(bool success)
    {
        if (success)
        {
            Debug.Log("Placement successful!");
            // 클라이언트 상태를 종료합니다. 실제 오브젝트 위치는 SyncDictionary를 통해 업데이트됩니다.
            EndState();
        }
        else
        {
            Debug.Log("Placement failed. Item returned.");
            // 실패 시 아이템을 다시 손에 쥐는 로직이 필요할 수 있습니다.
            EndState();
        }
    }

    private bool CheckPlacementValidity(Vector3Int gridPosition)
    {
        // 1. 구조물이 차지할 그리드 포지션 목록을 계산합니다.
        List<Vector3Int> positionToOccupyList = CalculatePositionList(gridPosition, _size);

        // 2. 차지할 그리드 셀이 이미 다른 오브젝트로 채워져 있는지 확인합니다.
        //    _placedObjectSyncDict는 GridManager에 있으므로 싱글톤 인스턴스를 사용합니다.
        foreach (Vector3Int pos in positionToOccupyList)
        {
            if (GridManager.Instance.GetObjectOnGrid(pos) != null)
            {
                return false;
            }
        }

        // 3. 구조물이 올바른 AreaType에 배치되는지 확인합니다.
        //    GridManager가 가지고 있는 영역 정보(EAreaType)를 사용합니다.
        EAreaType areaType = _data.AreaType;
        if (GridManager.Instance.GetPositionByAreaType(areaType) != null &&
            !GridManager.Instance.GetPositionByAreaType(areaType).Contains(gridPosition))
        {
            // 만약 현재 영역에 배치할 수 없는데, FrontYard 영역에는 배치할 수 있는 경우를 처리합니다.
            if (GridManager.Instance.GetPositionByAreaType(EAreaType.FrontYard) == null ||
                !GridManager.Instance.GetPositionByAreaType(EAreaType.FrontYard).Contains(gridPosition))
            {
                return false;
            }
        }

        return true;
    }

    /// <summary>
    /// 주어진 시작 위치와 크기로 구조물이 차지할 모든 그리드 좌표를 계산합니다.
    /// 이 메서드는 _grid가 가지고 있는 그리드 정보를 바탕으로 로직을 구현해야 합니다.
    /// </summary>
    private List<Vector3Int> CalculatePositionList(Vector3Int startPosition, Vector2Int size)
    {
        List<Vector3Int> positions = new List<Vector3Int>();
        for (int x = 0; x < size.x; x++)
        {
            for (int y = 0; y < size.y; y++)
            {
                positions.Add(startPosition + new Vector3Int(x, 0, y));
            }
        }
        return positions;
    }

    public void UpdateState(Vector3Int gridPosition)
    {
        if (PotionHouse.Instance.Layout.GetAvailableAreaDict().ContainsKey(gridPosition) == false)
        {
            return;
        }

        bool placementValidity = CheckPlacementValidity(gridPosition);
        _previewSystem.UpdatePosition(_grid.CellToWorld(gridPosition), placementValidity);
    }
}
