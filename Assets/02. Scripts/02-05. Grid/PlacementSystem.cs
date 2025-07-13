using Google.Apis.Sheets.v4.Data;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using UnityEditor.Rendering;
using UnityEngine;
using Color = UnityEngine.Color;

public class PlacementSystem : MonoBehaviour
{
    [SerializeField]
    private Grid _grid;
    [SerializeField]
    private GridTest_InputManager _inputManager;

    [SerializeField]
    private GridTest_ObjectDatabaseSO _database;

    [SerializeField]
    private GameObject _gridVisualization;

    // floor가 database의 0번째일때 사용하기 위한 변수
    private GridTest_GridData _floorData;
    private GridTest_GridData _furnitureData;

    [SerializeField]
    private PreviewSystem _previewSystem;

    private Vector3Int lastDetectedPosition = Vector3Int.zero;

    [SerializeField]
    private ObjectPlacer _objectPlacer;

    private IBuildingState _buildingState;

    private void Start()
    {
        StopPlacement();
        _floorData = new();
        _furnitureData = new();
    }

    private void Update()
    {
        if (ReferenceEquals(_buildingState, null))
        {
            return;
        }

        Vector3 mousePosition = _inputManager.GetSelectedMapPosition();
        Vector3Int gridPosition = _grid.WorldToCell(mousePosition);
        if(lastDetectedPosition != gridPosition)
        {
            _buildingState.UpdateState(gridPosition);
            lastDetectedPosition = gridPosition;
        }
    }

    public void StartPlacement(int id)
    {
        StopPlacement();
        _gridVisualization.SetActive(true);
        _buildingState = new PlacementState(id,
                                            _grid,
                                            _previewSystem,
                                            _database,
                                            _furnitureData,
                                            _objectPlacer);
        _inputManager.OnClicked += PlaceStructure;
        _inputManager.OnExit += StopPlacement;
    }

    public void StartRemoving()
    {
        StopPlacement();
        _gridVisualization.SetActive(true);
        _buildingState = new RemovingState(_grid,
                                           _previewSystem,
                                           _furnitureData,
                                           _objectPlacer);
        _inputManager.OnClicked += PlaceStructure;
        _inputManager.OnExit += StopPlacement;
    }
    private void PlaceStructure()
    {
        if (_inputManager.IsPointerOverUI())
        {
            return;
        }
        Vector3 mousePosition = _inputManager.GetSelectedMapPosition();
        Vector3Int gridPosition = _grid.WorldToCell(mousePosition);

        _buildingState.OnAction(gridPosition);
    }

    //private bool CheckPlacementValidity(Vector3Int gridPosition, int selectedObjectDBIndex)
    //{
    //    GridTest_GridData selectedData = _database.ObjectsDataList[selectedObjectDBIndex].ID == 0 ? _floorData : _furnitureData;

    //    return selectedData.CanPlaceObjectAt(gridPosition, _database.ObjectsDataList[selectedObjectDBIndex].Size);
    //}

    private void StopPlacement()
    {
        if(ReferenceEquals(_buildingState, null))
        {
            return;
        }
        _gridVisualization.SetActive(false);
        _buildingState.EndState();
        _inputManager.OnClicked -= PlaceStructure;
        _inputManager.OnExit -= StopPlacement;
        lastDetectedPosition = Vector3Int.zero;
        _buildingState = null;
    }
}
