using Photon.Pun;
using System;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using VInspector;

public class GridManager : MonoBehaviourSingleton<GridManager>
{
    [Foldout("Hierarchy")]
    [SerializeField]
    private Grid _grid;
    [SerializeField]
    private GameObject _gridVisualization;

    [SerializeField]
    private PreviewSystem _previewSystem;

    [SerializeField]
    private PlaceSystem _placeSystem = new();

    private Layout _layout;

    private GridData _gridData;
    private Vector3Int _lastDetectedPosition = Vector3Int.zero;
    private IBuildingState _buildingState;

    private GameObject _cahser;
    public GameObject Casher => _cahser;
    private GameObject _door;
    public GameObject Door => _door;
    private List<GameObject> _pickUpTableList;
    public List<GameObject> PickUpTableList => _pickUpTableList;

    // private GridRepository _repository;

    private void Start()
    {
        StopPlacement();
        _layout = GameObject.FindGameObjectWithTag("Layout").GetComponent<Layout>();
        _gridData = new GridData(_layout.GetAvailableAreaDict());
        _pickUpTableList = new List<GameObject>();
    }

    public void UpdatePlacementPosition(Vector3 targetPosition)
    {
        if (ReferenceEquals(_buildingState, null))
        {
            return;
        }

        Vector3Int gridPosition = GetGridPosition(targetPosition);
        if (_lastDetectedPosition != gridPosition)
        {
            _buildingState.UpdateState(gridPosition);
            _lastDetectedPosition = gridPosition;
        }
    }

    public GameObject GetObjectOnGrid(Vector3 targetPosition)
    {
        Vector3Int gridPosition = GetGridPosition(targetPosition);
        int index = _gridData.GetRepresentationIndex(gridPosition);
        return _placeSystem.GetGameObject(index);
    }

    public GameObject StartPlacement(Vector3 targetPosition)
    {
        StopPlacement();
        _gridVisualization.SetActive(true);

        Vector3Int gridPosition = GetGridPosition(targetPosition);
        Placement placement = _gridData.GetPlacement(gridPosition);
        GameObject structure = _placeSystem.GetGameObject(placement.PlacedObjectIndex);
        _placeSystem.RemoveObjectAt(placement.PlacedObjectIndex);
        _gridData.RemoveObjectAt(gridPosition);

        StructureData data = DataTable.Instance.GetStructureData(placement.TID);
        _buildingState = new PlacementState(structure,
                                            data,
                                            _grid,
                                            _previewSystem,
                                            _gridData,
                                            _placeSystem);

        return structure;
    }

    public void CreateStructure(int tid, Vector3 position, EStructureType structureType, int ingredientTID = 0)
    {
        StopPlacement();
        StructureData data = DataTable.Instance.GetStructureData(tid);
        GameObject newObject = StructureManager.Instance.CreateStructure(tid, ingredientTID);

        switch (data.SpecialStructureType)
        {
            case ESpecialStructureType.PickUpTable:
                _pickUpTableList.Add(newObject);
                break;
            case ESpecialStructureType.TrashCan:
                break;
            case ESpecialStructureType.Casher:
                _cahser = newObject;
                break;
            case ESpecialStructureType.None:
                break;
        }

        _buildingState = new PlacementState(newObject,
                                            data,
                                            _grid,
                                            _previewSystem,
                                            _gridData,
                                            _placeSystem);
        TryPlaceStructure(position);
    }

    public bool TryPlaceStructure(Vector3 position)
    {
        Vector3Int gridPosition = GetGridPosition(position);
        if(ReferenceEquals(_buildingState, null))
        {
            return false;
        }

        if (_buildingState.TryAction(gridPosition))
        {
            StopPlacement();
            return true;
        }

        return false;
    }

    [Button("생성 테스트")]
    public async void Test()
    {
        CreateStructure(10000, new Vector3(-5, 0, 4), EStructureType.Machine);
        CreateStructure(10002, new Vector3(-3, 0, 4), EStructureType.Machine);
        CreateStructure(10014, new Vector3(0, 0, 0), EStructureType.Furniture);
        CreateStructure(10016, new Vector3(-5, 0, 0), EStructureType.Furniture);
        CreateStructure(10005, new Vector3(0, 0, 2), EStructureType.Furniture);
        CreateStructure(10019, new Vector3(4, 0, 2), EStructureType.Storage, 10000);
        CreateStructure(10019, new Vector3(4, 0, 4), EStructureType.Storage, 10001);
    }

    public Vector3Int GetGridPosition(Vector3 targetPosition)
    {
        targetPosition = new Vector3(targetPosition.x, 0, targetPosition.z);
        return _grid.WorldToCell(targetPosition);
    }

    private void StopPlacement()
    {
        if (ReferenceEquals(_buildingState, null))
        {
            return;
        }

        _gridVisualization.SetActive(false);
        _buildingState.EndState();
        _lastDetectedPosition = Vector3Int.zero;
        _buildingState = null;
    }
}
