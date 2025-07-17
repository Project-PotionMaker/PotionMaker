using System;
using System.Collections.Generic;
using UnityEngine;
using VInspector;

public class GridManager : MonoBehaviourSingleton<GridManager>
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
        if (ReferenceEquals(_buildingState, null))
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

    public bool CanInteract(Vector3 targetPosition)
    {
        Vector3Int gridPosition = _grid.WorldToCell(targetPosition);
        int index = _gridData.GetRepresentationIndex(gridPosition);
        return index == -1 ? false : true;
    }

    public GameObject TryPickup(Vector3 targetPosition)
    {
        //if (CanInteract(targetPosition))
        //{
        //    _buildingState = new PlacementState(data,
        //                                    _grid,
        //                                    _previewSystem,
        //                                    _gridData,
        //                                    _objectPlacer);
        //}


        return null;
    }

    public bool TryDrop(Vector3 targetPosition)
    {
        //Vector3Int gridPosition = _grid.WorldToCell(targetPosition);

        //_buildingState.OnAction(gridPosition);
        return false;
    }

    [ContextMenu("생성 테스트")]
    public void Test()
    {
        Delivery(10000, new Vector3(-2, 0, 2));
    }

    public void StartPlacement(Vector3 targetPosition)
    {
        StopPlacement();
        _gridVisualization.SetActive(true);

        Vector3Int gridPosition = _grid.WorldToCell(targetPosition);
        Placement placement = _gridData.GetPlacement(gridPosition);
        GameObject structure = _objectPlacer.GetGameObject(placement.PlacedObjectIndex);
        _objectPlacer.RemoveObjectAt(placement.PlacedObjectIndex);
        _gridData.RemoveObjectAt(gridPosition);

        StructureData data = DataTable.Instance.GetStructureData(placement.TID);
        _buildingState = new PlacementState(structure,
                                            data,
                                            _grid,
                                            _previewSystem,
                                            _gridData,
                                            _objectPlacer);

        //_inputManager.OnClicked += PlaceStructure;
        //_inputManager.OnExit += StopPlacement;
    }

    public void Delivery(int tid, Vector3 position)
    {
        StopPlacement();
        _gridVisualization.SetActive(true);
        StructureData data = DataTable.Instance.GetStructureData(tid);
        GameObject newObject = StructureManager.Instance.CreateStructure(tid);
        _buildingState = new PlacementState(newObject,
                                            data,
                                            _grid,
                                            _previewSystem,
                                            _gridData,
                                            _objectPlacer);
        _buildingState.OnAction(_grid.WorldToCell(position));
        StopPlacement();
    }

    //public void StartRemoving()
    //{
    //    StopPlacement();
    //    _gridVisualization.SetActive(true);
    //    _buildingState = new RemovingState(_grid,
    //                                       _previewSystem,
    //                                       _gridData,
    //                                       _objectPlacer);
    //    //_inputManager.OnClicked += PlaceStructure;
    //    _inputManager.OnExit += StopPlacement;
    //}

    private void PlaceStructure(Vector3 position)
    {
        Vector3Int gridPosition = _grid.WorldToCell(position);

        _buildingState.OnAction(gridPosition);

        StopPlacement();
    }

    private void StopPlacement()
    {
        if (ReferenceEquals(_buildingState, null))
        {
            return;
        }

        _gridVisualization.SetActive(false);
        _buildingState.EndState();
        //_inputManager.OnClicked -= PlaceStructure;
        _inputManager.OnExit -= StopPlacement;
        _lastDetectedPosition = Vector3Int.zero;
        _buildingState = null;
    }
}
