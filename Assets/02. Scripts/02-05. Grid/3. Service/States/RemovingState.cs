using System;
using UnityEngine;

public class RemovingState : IBuildingState
{
    private int _gameObjectIndex = -1;
    private Grid _grid;
    private PreviewSystem _previewSystem;
    private GridData _gridData;
    private PlaceSystem _objectPlacer;

    public RemovingState(Grid grid,
                         PreviewSystem previewSystem,
                         GridData gridData,
                         PlaceSystem objectPlacer)
    {
        _grid = grid;
        _previewSystem = previewSystem;
        _gridData = gridData;
        _objectPlacer = objectPlacer;

        _previewSystem.StartShowingRemovePreview();
    }

    public void EndState()
    {
        _previewSystem.StopShowingPreview();
    }

    public void OnAction(Vector3Int gridPosition)
    {
        GridData selectedData = null;
        //테스트용
        Debug.Log("테스트");
        if(_gridData.CanPlaceObjectAt(gridPosition, Vector2Int.one, EAreaType.Kitchen) == false)
        {
            selectedData = _gridData;
        }

        if(ReferenceEquals(selectedData, null))
        {
            return;
        }
        else
        {
            _gameObjectIndex = selectedData.GetRepresentationIndex(gridPosition);
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

        //테스트용
        Debug.Log("테스트");
        return !_gridData.CanPlaceObjectAt(gridPosition, Vector2Int.one, EAreaType.Kitchen);
    }
}
