using UnityEngine;

public class PlacementState : IBuildingState
{
    private int _tid;
    private Grid _grid;
    private PreviewSystem _previewSystem;
    private GridData _gridData;
    private PlaceSystem _objectPlacer;

    public PlacementState(int tid,
                          Grid grid,
                          PreviewSystem previewSystem,
                          GridData gridData,
                          PlaceSystem objectPlacer)
    {
        _tid = tid;
        _grid = grid;
        _previewSystem = previewSystem;
        _gridData = gridData;
        _objectPlacer = objectPlacer;

        // 사이즈 고정됨 수정 필요
        Debug.Log("수정 필요");
        _previewSystem.StartShowingPlacementPreview(
            MachineManager.Instance.GetMachinePrefab(tid),
            Vector2Int.one);
    }

    public void EndState()
    {
        _previewSystem.StopShowingPreview();
    }

    public void OnAction(Vector3Int gridPosition)
    {
        bool placementValidity = CheckPlacementValidity(gridPosition, _tid);
        if (placementValidity == false)
        {
            return;
        }

        int index = _objectPlacer.PlaceObject(
            MachineManager.Instance.GetMachinePrefab(_tid),
            _grid.CellToWorld(gridPosition));

        // 사이즈 고정됨 수정 필요
        Debug.Log("수정 필요");
        _gridData.AddObjectAt(gridPosition,
                                 Vector2Int.one,
                                 _tid,
                                 index);
        _previewSystem.UpdatePosition(_grid.CellToWorld(gridPosition), false);
    }

    private bool CheckPlacementValidity(Vector3Int gridPosition, int TID)
    {
        // 사이즈 + 데이터테이블 고정됨 수정 필요
        Debug.Log("수정 필요");

        EAreaType type = DataTable.Instance.GetMachineData(TID).AreaType;
        return _gridData.CanPlaceObjectAt(gridPosition, Vector2Int.one, type);
    }

    public void UpdateState(Vector3Int gridPosition)
    {
        bool placementValidity = CheckPlacementValidity(gridPosition, _tid);

        _previewSystem.UpdatePosition(_grid.CellToWorld(gridPosition), placementValidity);
    }
}
