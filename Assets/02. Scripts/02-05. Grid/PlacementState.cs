using UnityEngine;

public class PlacementState : IBuildingState
{
    private int _selectedObjectIndex = -1;
    private int _id;
    private Grid _grid;
    private PreviewSystem _previewSystem;
    private GridTest_ObjectDatabaseSO _database;
    private GridTest_GridData _furnitureData;
    private ObjectPlacer _objectPlacer;

    public PlacementState(int id,
                          Grid grid,
                          PreviewSystem previewSystem,
                          GridTest_ObjectDatabaseSO database,
                          GridTest_GridData furnitureData,
                          ObjectPlacer objectPlacer)
    {
        _id = id;
        _grid = grid;
        _previewSystem = previewSystem;
        _database = database;
        _furnitureData = furnitureData;
        _objectPlacer = objectPlacer;

        _selectedObjectIndex = _database.ObjectsDataList.FindIndex(data => data.ID == id);
        if (_selectedObjectIndex > -1)
        {
            _previewSystem.StartShowingPlacementPreview(
                _database.ObjectsDataList[_selectedObjectIndex].Prefab,
                _database.ObjectsDataList[_selectedObjectIndex].Size);
        }
        else
        {
            throw new System.Exception($"No object with ID {id}");
        }
    }

    public void EndState()
    {
        _previewSystem.StopShowingPreview();
    }

    public void OnAction(Vector3Int gridPosition)
    {
        bool placementValidity = CheckPlacementValidity(gridPosition, _selectedObjectIndex);
        if (placementValidity == false)
        {
            return;
        }

        int index = _objectPlacer.PlaceObject(
            _database.ObjectsDataList[_selectedObjectIndex].Prefab,
            _grid.CellToWorld(gridPosition));
        GridTest_GridData selectedData = _database.ObjectsDataList[_selectedObjectIndex].ID == 0 ? _furnitureData : _furnitureData;
        selectedData.AddObjectAt(gridPosition,
                                 _database.ObjectsDataList[_selectedObjectIndex].Size,
                                 _database.ObjectsDataList[_selectedObjectIndex].ID,
                                 index);
        _previewSystem.UpdatePosition(_grid.CellToWorld(gridPosition), false);
    }

    private bool CheckPlacementValidity(Vector3Int gridPosition, int selectedObjectDBIndex)
    {
        GridTest_GridData selectedData = _database.ObjectsDataList[selectedObjectDBIndex].ID == 0 ? _furnitureData : _furnitureData;

        return selectedData.CanPlaceObjectAt(gridPosition, _database.ObjectsDataList[selectedObjectDBIndex].Size);
    }

    public void UpdateState(Vector3Int gridPosition)
    {
        bool placementValidity = CheckPlacementValidity(gridPosition, _selectedObjectIndex);

        _previewSystem.UpdatePosition(_grid.CellToWorld(gridPosition), placementValidity);
    }
}
