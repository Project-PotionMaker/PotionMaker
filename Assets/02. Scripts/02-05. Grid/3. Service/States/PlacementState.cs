using UnityEngine;

public class PlacementState : IBuildingState
{
    private StructureData _data;
    private Grid _grid;
    private PreviewSystem _previewSystem;
    private GridData _gridData;
    private PlaceSystem _objectPlacer;
    private GameObject _structure;
    private Vector2Int _size;

    public PlacementState(GameObject structure,
                          StructureData data,
                          Grid grid,
                          PreviewSystem previewSystem,
                          GridData gridData,
                          PlaceSystem objectPlacer)
    {
        _structure = structure;
        _data = data;
        _grid = grid;
        _previewSystem = previewSystem;
        _gridData = gridData;
        _objectPlacer = objectPlacer;

        _size = new Vector2Int(data.Width, data.Length);

        _previewSystem.StartShowingPlacementPreview(
            _data.TID,
            _size);
    }

    public void EndState()
    {
        _previewSystem.StopShowingPreview();
    }

    public bool TryAction(Vector3Int gridPosition)
    {
        bool placementValidity = CheckPlacementValidity(gridPosition);
        if (placementValidity == false)
        {
            return false;
        }

        int index = _objectPlacer.PlaceObject(
            _structure,
            _data.TID,
            _grid.CellToWorld(gridPosition));

        _gridData.AddObjectAt(gridPosition,
                                 _size,
                                 _data.TID,
                                 _data.StructureType,
                                 index);
        _previewSystem.UpdatePosition(_grid.CellToWorld(gridPosition), false);
        return true;
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
