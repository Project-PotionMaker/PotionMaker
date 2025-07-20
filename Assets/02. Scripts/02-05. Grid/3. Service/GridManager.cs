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

    public GameObject GetObjectOnGrid(Vector3 targetPosition)
    {
        Vector3Int gridPosition = GetGridPosition(targetPosition);
        int index = _gridData.GetRepresentationIndex(gridPosition);
        return _placeSystem.GetGameObject(index);
    }

    public bool TryInteract(Vector3 targetPosition)
    {
        Vector3Int gridPosition = GetGridPosition(targetPosition);
        int index = _gridData.GetRepresentationIndex(gridPosition);
        if(index != -1)
        {
            GameObject structure = _placeSystem.GetGameObject(index);
            Machine machine = structure.GetComponent<Machine>();
            if (ReferenceEquals(machine, null) == false)
            {
                if (machine.TryInteract())
                {
                    return true;
                }
            }
        }
        return false;
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
        DefaultPool defaultPool = PhotonNetwork.PrefabPool as DefaultPool;

        //string playerAddressableKey = "Prefab_Structure_10000";

        //GameObject _playerPrefab = await AssetManager.Instance.LoadAsset<GameObject>(playerAddressableKey);

        //defaultPool.ResourceCache.Add(playerAddressableKey, _playerPrefab);

        string playerAddressableKey = "Prefab_Structure_10018";

        GameObject _playerPrefab = await AssetManager.Instance.LoadAsset<GameObject>(playerAddressableKey);

        defaultPool.ResourceCache.Add(playerAddressableKey, _playerPrefab);

        CreateStructure(10000, new Vector3(-2, 0, 2), EStructureType.Machine);

        CreateStructure(10018, new Vector3(3, 0, 2), EStructureType.Storage);
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
