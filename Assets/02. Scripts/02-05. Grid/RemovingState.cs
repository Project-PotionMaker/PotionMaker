using System;
using UnityEngine;

public class RemovingState : IBuildingState
{
    private int _gameObjectIndex = -1;
    private Grid _grid;
    private PreviewSystem _previewSystem;
    private GridTest_GridData _furnitureData;
    private ObjectPlacer _objectPlacer;

    public RemovingState(Grid grid,
                         PreviewSystem previewSystem,
                         GridTest_GridData furnitureData,
                         ObjectPlacer objectPlacer)
    {
        _grid = grid;
        _previewSystem = previewSystem;
        _furnitureData = furnitureData;
        _objectPlacer = objectPlacer;

        _previewSystem.StartShowingRemovePreview();
    }

    public void EndState()
    {
        _previewSystem.StopShowingPreview();
    }

    public void OnAction(Vector3Int gridPosition)
    {
        GridTest_GridData selectedData = null;
        if(_furnitureData.CanPlaceObjectAt(gridPosition, Vector2Int.one) == false)
        {
            selectedData = _furnitureData;
        }

        if(ReferenceEquals(selectedData, null))
        {
            return;
        }
        else
        {
            _gameObjectIndex = selectedData.getRepresentationIndex(gridPosition);
            if(_gameObjectIndex == -1)
            {
                return;
            }
            selectedData.RemoveObjectAt(gridPosition);
            _objectPlacer.RemoveObjectAt(_gameObjectIndex);
        }

        Vector3 cellPosition = _grid.CellToWorld(gridPosition);
        _previewSystem.UpdatePosition(cellPosition, CheckIfSelectionIsValid(gridPosition));
    }

    public void UpdateState(Vector3Int gridPosition)
    {
        bool validity = CheckIfSelectionIsValid(gridPosition);
        _previewSystem.UpdatePosition(_grid.CellToWorld(gridPosition), validity);
    }

    private bool CheckIfSelectionIsValid(Vector3Int gridPosition)
    {
        // 공간이 없어야 삭제
        return !_furnitureData.CanPlaceObjectAt(gridPosition, Vector2Int.one);
    }
}
