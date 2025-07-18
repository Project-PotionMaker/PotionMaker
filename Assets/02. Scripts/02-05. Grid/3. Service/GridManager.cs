using System;
using System.Collections.Generic;
using TMPro;
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

    // private GridRepository _repository;

    private void Start()
    {
        StopPlacement();
        _layout = GameObject.FindGameObjectWithTag("Layout").GetComponent<Layout>();
        _gridData = new GridData(_layout.GetAvailableAreaDict());
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

    public bool CheckObjectOnGrid(Vector3 targetPosition)
    {
        Vector3Int gridPosition = GetGridPosition(targetPosition);
        int index = _gridData.GetRepresentationIndex(gridPosition);
        return index == -1 ? false : true;

        //if (PhaseManager.Instance.CurrentPhase.PhaseType == EPhaseType.PreparingPhase)
        //{
        //}
        //else if (PhaseManager.Instance.CurrentPhase.PhaseType == EPhaseType.ServingPhase)
        //{
        //    Placement placement = _gridData.GetPlacement(gridPosition);
        //    GameObject structure = _placeSystem.GetGameObject(placement.PlacedObjectIndex);
        //    if (placement.structureType == EStructureType.Machine)
        //    {
        //        GameObject pickupItem = structure.GetComponent<Machine>().TakeOutput();

        //    }
        //}
        //return false;

    }

    public GameObject TryPickup(Vector3 targetPosition)
    {
        if (PhaseManager.Instance.CurrentPhase.PhaseType == EPhaseType.PreparingPhase)
        {
            if (CheckObjectOnGrid(targetPosition))
            {
                return StartPlacement(targetPosition);
            }
        }
        else if (PhaseManager.Instance.CurrentPhase.PhaseType == EPhaseType.ServingPhase)
        {
            Vector3Int gridPosition = GetGridPosition(targetPosition);
            Placement placement = _gridData.GetPlacement(gridPosition);
            if(ReferenceEquals(placement, null))
            {
                return null;
            }
            GameObject structure = _placeSystem.GetGameObject(placement.PlacedObjectIndex);
            if (placement.structureType == EStructureType.Machine)
            {
                GameObject pickupItem = structure.GetComponent<Machine>().TakeOutput();
                return pickupItem;
            }
        }
        return null;
    }

    public bool TryDrop(Vector3 targetPosition, )
    {
        Vector3Int gridPosition = GetGridPosition(targetPosition);
        if (PhaseManager.Instance.CurrentPhase.PhaseType == EPhaseType.PreparingPhase)
        {
            if (_buildingState.TryAction(gridPosition))
            {
                StopPlacement();
                return true;
            }
        }
        else if (PhaseManager.Instance.CurrentPhase.PhaseType == EPhaseType.ServingPhase)
        {
            Placement placement = _gridData.GetPlacement(gridPosition);
            if (ReferenceEquals(placement, null))
            {
                return false;
            }
            GameObject structure = _placeSystem.GetGameObject(placement.PlacedObjectIndex);
            if (placement.structureType == EStructureType.Machine)
            {
                GameObject pickupItem = structure.GetComponent<Machine>().TryInput()
                return pickupItem;
            }
        }


        return false;
    }
    private GameObject StartPlacement(Vector3 targetPosition)
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

    public void CreateStructure(int tid, Vector3 position, EStructureType structureType)
    {
        StopPlacement();
        StructureData data = DataTable.Instance.GetStructureData(tid);
        GameObject newObject = StructureManager.Instance.CreateStructure(tid);
        _buildingState = new PlacementState(newObject,
                                            data,
                                            _grid,
                                            _previewSystem,
                                            _gridData,
                                            _placeSystem);
        PlaceStructure(position);
    }

    private void PlaceStructure(Vector3 position)
    {
        Vector3Int gridPosition = GetGridPosition(position);

        if (_buildingState.TryAction(gridPosition))
        {
            StopPlacement();
        }
    }

    [ContextMenu("생성 테스트")]
    public void Test()
    {
        CreateStructure(10000, new Vector3(-2, 0, 2), EStructureType.Machine);
    }

    private Vector3Int GetGridPosition(Vector3 targetPosition)
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
