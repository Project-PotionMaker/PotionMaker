//using Photon.Pun;
using System;
using System.Collections.Generic;
using System.Linq;
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

    private Layout _layout;
    
    private GridData _gridData;
    private Vector3Int _lastDetectedPosition = Vector3Int.zero;
    private IBuildingState _buildingState;

    private GameObject _cahser;
    public GameObject Casher => _cahser;
    private GameObject _enterDoor;
    public GameObject EnterDoor => _enterDoor;
    private GameObject _exitDoor;
    public GameObject ExitDoor => _exitDoor;
    private List<GameObject> _pickUpTableList;
    public List<GameObject> PickUpTableList => _pickUpTableList;
    private List<GameObject> _oldChairList;
    public List<GameObject> OldChairList => _oldChairList;
    private List<GameObject> _luxuryChairList;
    public List<GameObject> LuxuryChairList => _luxuryChairList;


    // private GridRepository _repository;

    protected override void Awake()
    {
        base.Awake();
        StopPlacement();

        _layout = GameObject.FindGameObjectWithTag(nameof(ETags.Layout)).GetComponent<Layout>();
        _gridData = new GridData(_layout.GetAvailableAreaDict());
        _pickUpTableList = new List<GameObject>();
        _oldChairList = new List<GameObject>();
        _luxuryChairList = new List<GameObject>();
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
        Placement placement = _gridData.GetPlacement(gridPosition);
        if(ReferenceEquals(placement, null))
        {
            return null;
        }
        return placement.StructureObject;
    }

    public GameObject StartPlacement(Vector3 targetPosition)
    {
        StopPlacement();
        _gridVisualization.SetActive(true);
        Debug.Log(targetPosition);
        Vector3Int gridPosition = GetGridPosition(targetPosition);
        Placement placement = _gridData.GetPlacement(gridPosition);
        GameObject structure = placement.StructureObject;
        _gridData.RemoveObjectAt(gridPosition);

        StructureData data = DataTable.Instance.GetStructureData(placement.TID);
        _buildingState = new PlacementState(structure,
                                            data,
                                            _grid,
                                            _previewSystem,
                                            _gridData);

        return structure;
    }

    public bool CreateStructure(int tid, Vector3 position, int ingredientTID = 0)
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
            case ESpecialStructureType.OldChair:
                _oldChairList.Add(newObject);
                break;
            case ESpecialStructureType.LuxuryChair:
                _luxuryChairList.Add(newObject);
                break;
            case ESpecialStructureType.None:
                break;
        }

        _buildingState = new PlacementState(newObject,
                                            data,
                                            _grid,
                                            _previewSystem,
                                            _gridData);
        return TryPlaceStructure(position);
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
        CreateStructure(10000, new Vector3(-5, 0, 4)); //절구
        CreateStructure(10003, new Vector3(-3, 0, 4)); //가열냄비
        CreateStructure(10013, new Vector3(-1, 0, 0)); // 픽업테이블
        CreateStructure(10013, new Vector3(0, 0, 0)); // 픽업테이블
        CreateStructure(10014, new Vector3(0, 0, 4)); // 쓰레기통
        CreateStructure(10015, new Vector3(-5, 0, 0)); // 계산기
        CreateStructure(10016, new Vector3(-1, 0, -5)); // 허름한 의자
        CreateStructure(10017, new Vector3(0, 0, -5)); // 푹신한 의자
        CreateStructure(10006, new Vector3(0, 0, 2)); // 병입기
        CreateStructure(10018, new Vector3(4, 0, 2), 10006); // 동물상자
        CreateStructure(10018, new Vector3(4, 0, 4), 10007); // 동물상자
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

    public ReadOnlyList<Vector3Int> GetPositionByAreaType(EAreaType areaType)
    {
        foreach(AreaDefinition areaDefinition in _layout.AllAreaDefinitionList)
        {
            if(areaDefinition.AreaType == areaType)
            {
                return new ReadOnlyList<Vector3Int>(areaDefinition.GridPositionList);
            }
        }

        return null;
    }
    
    public ReadOnlyList<int> GetPlacedStructureTIDList()
    {
        return _gridData.PlacedObjectList;
    }
}
