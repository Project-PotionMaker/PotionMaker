using UnityEngine;

public class PlacementState : IBuildingState
{
    private StructureData _data;
    private Grid _grid;
    private PreviewSystem _previewSystem;
    private GridData _gridData;
    private PlaceSystem _objectPlacer;
    private Vector2Int _size;

    public PlacementState(StructureData data,
                          Grid grid,
                          PreviewSystem previewSystem,
                          GridData gridData,
                          PlaceSystem objectPlacer)
    {
        _data = data;
        _grid = grid;
        _previewSystem = previewSystem;
        _gridData = gridData;
        _objectPlacer = objectPlacer;

        _size = new Vector2Int(data.Width, data.Length);

        _previewSystem.StartShowingPlacementPreview(
            StructureManager.Instance.GetMachinePrefab(data.TID),
            _size);
    }

    public void EndState()
    {
        _previewSystem.StopShowingPreview();
    }

    public void OnAction(Vector3Int gridPosition)
    {
        bool placementValidity = CheckPlacementValidity(gridPosition);
        if (placementValidity == false)
        {
            return;
        }

        // MachineManager 말고 StructureManager에서 가져와야됨
        Debug.Log("수정필요ㅕ");
        int index = _objectPlacer.PlaceObject(
            StructureManager.Instance.GetMachinePrefab(_data.TID),
            _grid.CellToWorld(gridPosition));

        _gridData.AddObjectAt(gridPosition,
                                 _size,
                                 _data.TID,
                                 index);
        _previewSystem.UpdatePosition(_grid.CellToWorld(gridPosition), false);
    }

    private bool CheckPlacementValidity(Vector3Int gridPosition)
    {
        return _gridData.CanPlaceObjectAt(gridPosition, _size, _data.AreaType);
    }

    public void UpdateState(Vector3Int gridPosition)
    {
        bool placementValidity = CheckPlacementValidity(gridPosition);

        _previewSystem.UpdatePosition(_grid.CellToWorld(gridPosition), placementValidity);
    }
}
