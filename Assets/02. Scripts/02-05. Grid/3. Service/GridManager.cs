using Mirror;
using Steamworks;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using VInspector;

/// <summary>
/// 그리드 및 가구 배치를 관리하는 싱글턴 클래스입니다.
/// 모든 배치 정보의 진실의 근원(Source of Truth)은 서버이며,
/// 클라이언트는 서버의 데이터를 동기화 받아 시각화만 처리합니다.
/// </summary>
public class GridManager : NetworkBehaviourSingleton<GridManager>, IShopInfoSaveable
{
    [Foldout("Hierarchy")]
    [SerializeField]
    private Grid _grid;
    [SerializeField]
    private GameObject _gridVisualization;

    [SerializeField]
    private PreviewSystem _previewSystem;

    // Layout은 클라이언트/서버 모두에서 초기화 시 사용됩니다.
    private Layout _layout;

    // 서버 전용: 배치 데이터를 관리합니다.
    private GridData _serverGridData;
    public GridData ServerGridData => _serverGridData;

    private HallAreaPathFinder _hallAreaPathFinder = new();

    // 클라이언트 전용: 현재 배치 중인 상태를 관리합니다.
    private IBuildingState _buildingState;
    private Vector3Int _lastDetectedPosition = Vector3Int.zero;

    // 모든 클라이언트에 동기화되는 배치 정보
    private readonly SyncDictionary<Vector3Int, PlacementData> _placedObjectInGridSyncDict = new();

    // 서버 전용
    private Dictionary<int, List<NetworkIdentity>> _managedStructureDict = new();
    public Dictionary<int, List<NetworkIdentity>> ManagedStructureDict => _managedStructureDict;

    private List<NetworkIdentity> _pickupTableForCustomerList = new();
    public List<NetworkIdentity> PickupTableForCustomerList => _pickupTableForCustomerList;

    private List<GridSaveData> _gridSaveDataList = new();
    public List<GridSaveDataDTO> GridSaveDataList => _gridSaveDataList.Select(gridSaveData => gridSaveData.ToDTO()).ToList();

    public override void OnStartClient()
    {

        base.OnStartClient();
        // 클라이언트가 시작될 때 씬 로드 이벤트를 구독합니다.
        SceneManager.sceneLoaded += OnSceneLoaded;
        // 현재 씬에 PotionHouse가 이미 존재하는 경우를 대비하여 초기화 시도를 합니다.
        TryInitializeWithPotionHouse();
        InitGridManager();
    }

    public override void OnStopClient()
    {
        base.OnStopClient();
        // 클라이언트가 정지될 때 이벤트를 반드시 구독 해제합니다.
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // 씬이 로드되면 PotionHouse를 찾아 초기화를 시도합니다.
        TryInitializeWithPotionHouse();
    }

    private void TryInitializeWithPotionHouse()
    {
        // PotionHouse 인스턴스가 존재하고 초기화가 완료되었을 때만 GridManager를 초기화합니다.
        if (PotionHouse.Instance != null)
        {
            // PotionHouse의 초기화 이벤트에 구독하여 PotionHouse의 데이터가 로드된 후 GridManager를 초기화합니다.
            PotionHouse.Instance.OnInitialized -= InitGridManager; // 중복 구독 방지
            PotionHouse.Instance.OnInitialized += InitGridManager;
        }
    }

    [Server]
    public override void OnStartServer()
    {
        base.OnStartServer();
        // 서버에서만 GridData와 고객 관련 리스트를 초기화합니다.
    }

    // 서버와 클라이언트 모두에서 호출되는 초기화 로직
    // 이제 이 메소드는 PotionHouse.OnInitialized 이벤트에 의해 호출됩니다.
    private void InitGridManager()
    {
        _grid = GameObject.FindGameObjectWithTag(nameof(ETags.Grid))?.GetComponent<Grid>();
        _previewSystem = GameObject.FindGameObjectWithTag(nameof(ETags.PreviewSystem))?.GetComponent<PreviewSystem>();
        _gridVisualization = GameObject.FindGameObjectWithTag(nameof(ETags.GridVisualization));
        _gridVisualization.SetActive(false);
        if (_grid == null)
        {
            return;
        }

        _layout = PotionHouse.Instance.Layout;
        _serverGridData = new GridData(_layout.GetAvailableAreaDict());
        _managedStructureDict = new();
        _pickupTableForCustomerList = new();

        _gridSaveDataList = ShopInfoManager.Instance.ShopInfo.GridSaveDataList;
        LoadGridSaveData();

        StopPlacement();
    }

    public List<NetworkIdentity> GetCustomerFurnitureList(ESpecialStructureType type)
    {
        int tid = StructureManager.Instance.SpecialStructureTIDDict[type];
        if(type == ESpecialStructureType.PickUpTable)
        {
            // customer과 상호작용 가능한 픽업테이블만 전달
            return _pickupTableForCustomerList;
        }
        else
        {
            if (!_managedStructureDict.TryGetValue(tid, out List<NetworkIdentity> value))
            {
                value = new List<NetworkIdentity>();
                _managedStructureDict.Add(tid, value);
            }
            return value;
        }
    }

    // --- 클라이언트 전용 로직: 미리보기 업데이트 ---
    [Client]
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

    // --- 클라이언트 전용 로직: 그리드 위의 오브젝트 정보를 가져옴 ---
    [Client]
    public GameObject GetObjectOnGrid(Vector3 targetPosition)
    {
        Vector3Int gridPosition = GetGridPosition(targetPosition);

        if (_placedObjectInGridSyncDict.TryGetValue(gridPosition, out PlacementData placementData))
        {
            if (NetworkClient.spawned.TryGetValue(placementData.netId, out NetworkIdentity identity))
            {
                return identity.gameObject;
            }
        }
        return null;
    }

    // --- 서버 전용: 그리드에 있는 오브젝트를 반환 ---
    [Server]
    public GameObject ServerGetObjectOnGrid(Vector3 targetPosition)
    {
        Vector3Int gridPosition = GetGridPosition(targetPosition);
        Placement placement = _serverGridData.GetPlacement(gridPosition);

        if (!ReferenceEquals(placement, null))
        {
            return placement.StructureObject;
        }
        return null;
    }


    // --- 클라이언트 전용 로직: 배치 상태 시작 ---
    [Client]
    public void StartPlacement(GameObject structure, StructureData data)
    {
        _gridVisualization.SetActive(true);
        StopPlacement();
        _buildingState = new PlacementState(structure, data, _grid, _previewSystem);
        _buildingState.StartState();
    }

    [Server]
    public bool ServerCanPlaceObjectAt(Vector3 targetPosition, EAreaType areaType)
    {
        Vector3Int gridPosition = GetGridPosition(targetPosition);
        if (_serverGridData != null)
        {
            return _serverGridData.CanPlaceObjectAt(gridPosition, areaType);
        }
        return false;
    }

    // --- 서버로 배치를 요청하는 Command ---
    [Server]
    public void ServerPlaceStructure(Vector3 targetPosition, uint structureNetId, NetworkConnectionToClient sender = null)
    {
        if (!isServer) return;

        // targetPosition을 gridPosition으로 변환
        Vector3Int gridPosition = GetGridPosition(targetPosition);

        if (NetworkServer.spawned.TryGetValue(structureNetId, out NetworkIdentity structureIdentity))
        {
            GameObject structure = structureIdentity.gameObject;
            StructureData data = DataTable.Instance.GetStructureData(structure.GetComponent<IGridItemHandler>().GetStructureTID());

            structure.transform.rotation = Quaternion.identity;
            structure.transform.position = _grid.CellToWorld(gridPosition) + new Vector3(0.5f, 0, 0.5f);
            _serverGridData.AddObjectAt(gridPosition, new Vector2Int(data.Width, data.Length), data.TID, data.StructureType, structure);

            PlacementData newPlacementData = new PlacementData(structureNetId, data.TID, structure.transform.rotation);
            foreach (var pos in _serverGridData.CalculatePositionList(gridPosition, new Vector2Int(data.Width, data.Length)))
            {
                _placedObjectInGridSyncDict[pos] = newPlacementData;
            }

            if (data.SpecialStructureType == ESpecialStructureType.PickUpTable)
            {
                HandlePickupTablePlacement(structureIdentity, gridPosition, data);
            }

            if (data.SpecialStructureType == ESpecialStructureType.PickUpTable ||
                data.SpecialStructureType == ESpecialStructureType.OldChair ||
                data.SpecialStructureType == ESpecialStructureType.LuxuryChair)
            {
                _hallAreaPathFinder.UpdateGridPathFinder(_serverGridData.PlacedPositionHashSet,
                    ToPickupTablePositionHashSet(_pickupTableForCustomerList));
                if (!_hallAreaPathFinder.HasPath())
                {
                    Debug.LogWarning("경로 없음!!!");
                    // 경로가 없을 때 로직
                }
            }

            TargetRpcOnPlaceStructure(sender, true, structureNetId);
        }
    }

    private void HandlePickupTablePlacement(NetworkIdentity structureIdentity, Vector3Int gridPosition, StructureData data)
    {
        if (data.SpecialStructureType == ESpecialStructureType.PickUpTable)
        {
            if (_serverGridData.CheckLRUDHasArea(gridPosition, EAreaType.Hall))
            {
                if (!_pickupTableForCustomerList.Contains(structureIdentity))
                {
                    _pickupTableForCustomerList.Add(structureIdentity);
                }
            }
            else
            {
                _pickupTableForCustomerList.Remove(structureIdentity);
            }
        }
    }


    // --- 클라이언트에게 배치 결과를 전달하는 TargetRpc ---
    [TargetRpc]
    private void TargetRpcOnPlaceStructure(NetworkConnectionToClient target, bool success, uint structureNetId)
    {
        if (NetworkClient.connection.identity == null)
        {
            return;
        }

        if (NetworkClient.connection.identity.isLocalPlayer)
        {
            if (_buildingState != null)
            {
                _buildingState.ReceivePlaceResult(success);
                if (success)
                {
                    StopPlacement();
                }
            }
        }
    }

    [Server]
    public GameObject ServerRemovePlacementDataOnly(Vector3 targetPosition)
    {
        // targetPosition을 gridPosition으로 변환
        Vector3Int gridPosition = GetGridPosition(targetPosition);

        Placement placement = _serverGridData.GetPlacement(gridPosition);
        if (ReferenceEquals(placement, null))
        {
            return null;
        }

        GameObject structureObject = placement.StructureObject;

        // 그리드 데이터와 SyncDictionary에서 객체 정보 삭제
        _serverGridData.RemoveObjectAt(gridPosition);
        foreach (var pos in placement.OccupiedPositionList)
        {
            _placedObjectInGridSyncDict.Remove(pos);
        }

        // GameObject는 파괴하지 않고 반환
        return structureObject;
    }

    // === (새로운 함수) 픽업한 플레이어의 클라이언트에서 PlacementState를 시작하도록 지시 ===
    [TargetRpc]
    public void TargetRpcStartPlacement(NetworkConnectionToClient target, uint structureNetId)
    {
        if (NetworkClient.connection.identity == null)
        {
            return;
        }

        if (NetworkClient.connection.identity.isLocalPlayer)
        {
            if (NetworkClient.spawned.TryGetValue(structureNetId, out NetworkIdentity itemIdentity))
            {
                StructureData data = DataTable.Instance.GetStructureData(itemIdentity.gameObject.GetComponent<IGridItemHandler>().GetStructureTID());
                StartPlacement(itemIdentity.gameObject, data);
            }
        }
    }

    // --- 서버 전용: 구조물 생성 ---
    [Server]
    public bool ServerCreateStructure(int tid, Vector3Int position, int ingredientTID = 0)
    {
        StructureData data = DataTable.Instance.GetStructureData(tid);
        GameObject newObject = StructureManager.Instance.ServerCreateStructure(tid, ingredientTID);

        Vector3Int gridPosition = GetGridPosition(position);
        if (_serverGridData.CanPlaceObjectAt(gridPosition, data.AreaType))
        {
            newObject.transform.rotation = Quaternion.identity;
            newObject.transform.position = _grid.CellToWorld(gridPosition) + new Vector3(0.5f, 0, 0.5f);
            _serverGridData.AddObjectAt(gridPosition, new Vector2Int(data.Width, data.Length), data.TID, data.StructureType, newObject);

            PlacementData newPlacementData = new PlacementData(newObject.GetComponent<NetworkIdentity>().netId, data.TID, newObject.transform.rotation);
            foreach (var pos in _serverGridData.CalculatePositionList(gridPosition, new Vector2Int(data.Width, data.Length)))
            {
                _placedObjectInGridSyncDict[pos] = newPlacementData;
            }
            if (newObject.TryGetComponent<NetworkIdentity>(out NetworkIdentity netId))
            {
                if (_managedStructureDict.TryGetValue(tid, out List<NetworkIdentity> structureNetIdList))
                {
                    structureNetIdList.Add(netId);
                }
                else
                {
                    _managedStructureDict.Add(tid, new List<NetworkIdentity>{ netId });
                }

                if (data.SpecialStructureType == ESpecialStructureType.PickUpTable)
                {
                    if (_serverGridData.CheckLRUDHasArea(gridPosition, EAreaType.Hall))
                    {
                        if (_pickupTableForCustomerList.Contains(netId) == false)
                        {
                            _pickupTableForCustomerList.Add(netId);
                        }
                    }
                }
            }


            return true;
        }

        StructureFactory.Instance.ReturnObject(newObject);
        return false;
    }

    [Server]
    public void ServerRefundStructure(int structureTID, GameObject refundObject)
    {
        _managedStructureDict[structureTID].Remove(refundObject.GetComponent<NetworkIdentity>());
        StopPlacement();
        StructureFactory.Instance.ReturnObject(refundObject);
    }


    // --- 클라이언트 전용: 현재 배치 상태 종료 ---
    [Client]
    public void StopPlacement()
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

    [Command(requiresAuthority = false)]
    public void LoadGridSaveData()
    {
        if (!isServer)
        {
            return;
        }
        foreach (GridSaveData data in ShopInfoManager.Instance.ShopInfo.GridSaveDataList)
        {
            ServerCreateStructure(data.StructureTID, data.GridPosition, data.IngredientTID);
        }

        ServerCreateStructure(StructureManager.Instance.SpecialStructureTIDDict[ESpecialStructureType.Casher], PotionHouse.Instance.Layout.CashierSpawnPosition); // 계산기 스폰
        ServerCreateStructure(StructureManager.Instance.SpecialStructureTIDDict[ESpecialStructureType.Practice], PotionHouse.Instance.Layout.PracticeSpawnPosition); // 연습모드 스폰

        _hallAreaPathFinder.InitGridPathFinder
            (GetPositionByAreaType(EAreaType.Hall).ToHashSet(),
            _serverGridData.PlacedPositionHashSet,
            PotionHouse.Instance.Layout.CashierSpawnPosition,
            PotionHouse.Instance.Layout.EnterDoorPosition,
            PotionHouse.Instance.Layout.ExitDoorPosition,
            ToPickupTablePositionHashSet(_pickupTableForCustomerList));
    }

    // --- 클라이언트/서버 공용 메서드 ---
    public Vector3Int GetGridPosition(Vector3 targetPosition)
    {
        if(_grid == null)
        {
            return Vector3Int.zero;
        }
        targetPosition = new Vector3(targetPosition.x, 0, targetPosition.z);
        return _grid.WorldToCell(targetPosition);
    }

    public ReadOnlyList<Vector3Int> GetPositionByAreaType(EAreaType areaType)
    {
        foreach (AreaDefinition areaDefinition in _layout.AllAreaDefinitionList)
        {
            if (areaDefinition.AreaType == areaType)
            {
                return new ReadOnlyList<Vector3Int>(areaDefinition.GridPositionList);
            }
        }

        return null;
    }

    public ReadOnlyList<int> GetPlacedStructureTIDList()
    {
        if (isClient)
        {
            return new ReadOnlyList<int>(_placedObjectInGridSyncDict.Values.Select(p => p.TID).ToList());
        }

        return _serverGridData.PlacedObjectList;
    }

    private HashSet<Vector3Int> ToPickupTablePositionHashSet(List<NetworkIdentity> pickupTableList)
    {
        return pickupTableList.Select
            (networkIdentity => GetGridPosition(networkIdentity.transform.position)).ToHashSet();
    }

    public void MakeGridSaveDataList()
    {
        _gridSaveDataList.Clear();
        foreach (var placedInfo in _serverGridData.PlacedObjectDict)
        {
            if (StructureManager.Instance.SpecialStructureTIDDict[ESpecialStructureType.Casher] == placedInfo.Value.TID ||
                StructureManager.Instance.SpecialStructureTIDDict[ESpecialStructureType.Practice] == placedInfo.Value.TID)
            {
                continue;
            }
            Vector3Int gridPosition = placedInfo.Key;
            int structureTID = placedInfo.Value.TID;
            int ingredientTID = placedInfo.Value.IngredientTID;
            _gridSaveDataList.Add(new GridSaveData(gridPosition, structureTID, ingredientTID));
        }
    }

    public void ApplyLoadedData(ShopInfo shopInfo)
    {
        _gridSaveDataList = shopInfo.GridSaveDataList;
    }

    public void ProvideSaveData(ShopInfo shopInfo)
    {
        MakeGridSaveDataList();
        shopInfo.GridSaveDataList = _gridSaveDataList;
    }
}


[Serializable]
public struct PlacementData
{
    public uint netId;
    public int TID;
    public Quaternion rotation;

    public PlacementData(uint netId, int tid, Quaternion rotation)
    {
        this.netId = netId;
        this.TID = tid;
        this.rotation = rotation;
    }
}