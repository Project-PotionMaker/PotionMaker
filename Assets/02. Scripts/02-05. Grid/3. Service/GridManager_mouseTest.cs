using System;
using System.Collections.Generic;
using UnityEngine;
using VInspector;

public class GridManager_mouseTest : MonoBehaviourSingleton<GridManager_mouseTest>
{
    [Foldout("Hierarchy")]
    [SerializeField]
    private Grid _grid;
    // 추후 삭제 (인풋매니저)
    [SerializeField]
    private GridTest_InputManager _inputManager;
    [SerializeField]
    private GameObject _gridVisualization;

    [SerializeField]
    private PreviewSystem _previewSystem;

    [SerializeField]
    private PlaceSystem _objectPlacer = new();

    private Layout _layout;

    private GridData _gridData;
    private Vector3Int _lastDetectedPosition = Vector3Int.zero;
    private IBuildingState _buildingState;
    // private GridRepository _repository;

    private void Start()
    {
        StopPlacement();
        _layout = GameObject.FindGameObjectWithTag("Layout").GetComponent<Layout>();
        _gridData = new GridData(_layout.GetAvailableAreaDict());
    }

    private void Update()
    {
        if(ReferenceEquals(_buildingState, null))
        {
            return;
        }

        // inputManager 테스트 코드
        Vector3 mousePosition = _inputManager.GetSelectedMapPosition();
        Vector3Int gridPosition = _grid.WorldToCell(mousePosition);
        if (_lastDetectedPosition != gridPosition)
        {
            _buildingState.UpdateState(gridPosition);
            _lastDetectedPosition = gridPosition;
        }
    }
    public void StartPlacement(int tid)
    {
        StopPlacement();
        _gridVisualization.SetActive(true);
        StructureData data = DataTable.Instance.GetStructureData(tid);
        _buildingState = new PlacementState(data,
                                            _grid,
                                            _previewSystem,
                                            _gridData,
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
                                           _gridData,
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
        _lastDetectedPosition = Vector3Int.zero;
        _buildingState = null;
    }
}
