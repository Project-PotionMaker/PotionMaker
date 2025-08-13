using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using VInspector;

/// <summary>
/// 플레이어가 상호작용할 수 있는 가구 클래스입니다.
/// Storage와 마찬가지로 SyncVar를 사용하여 네트워크 동기화를 처리합니다.
/// </summary>

public class Furniture : NetworkBehaviour, IGridItemHandler, IRefundable, ICustomerInteractable
{
    [SyncVar(hook = nameof(OnDataTIDChanged))]
    private int _dataTID;
    public int DataTID { get => _dataTID; private set => _dataTID = value; }

    [SyncVar(hook = nameof(OnCurrentRotationChanged))]
    private float _currentRotation;
    public float CurrentRotation { get => _currentRotation; private set => _currentRotation = value; }

    [SyncVar(hook = nameof(OnInputObjectChanged))]
    private GameObject _inputObject;
    public GameObject InputObject { get => _inputObject; set => _inputObject = value; }

    [SyncVar(hook = nameof(OnRefundProgressChanged))]
    private float _refundProgress;
    public float RefundProgress
    {
        get
        {
            return _refundProgress;
        }
        set
        {
            _refundProgress = value;
        }
    }

    public GameObject RefundObject { get => gameObject; }

    private FurnitureData _data;
    public FurnitureData Data => _data;

    [SerializeField]
    private Collider _collider;
    [SerializeField]
    private Transform _model;

    [SerializeField]
    private Transform _inputPosition;
    public Transform InputPosition => _inputPosition;

    // 서버 전용 컴포넌트들
    private IInteractable<Furniture> _interactComponent;
    private IInputContainer<Furniture> _inputComponent;
    private IOutputContainer<Furniture> _outputComponent;
    private ICustomerEffectable<Furniture> _effectComponent;

    private RefundSystem _refundSystem;
    public Action OnDataChanged;

    [Foldout("Project")]
    [SerializeField]
    private List<ModelOnTID> _modelObjectList = new List<ModelOnTID>();
    private Dictionary<int, GameObject> _modelObjectDic;

    private Coroutine _visibleRoutine;

    private void Awake()
    {
        _modelObjectDic = new Dictionary<int, GameObject>();
        foreach (var modelInfo in _modelObjectList)
        {
            modelInfo.Model.SetActive(false);
            _modelObjectDic.Add(modelInfo.TID, modelInfo.Model);
        }
        _refundSystem = new RefundSystem();
    }

    
    public override void OnStartClient()
    {
        base.OnStartClient();
        _model.gameObject.SetActive(false);
        
        if(!ReferenceEquals(_visibleRoutine, null))
        {
            StopCoroutine(VisibleRoutine());
        }
        _visibleRoutine = StartCoroutine(VisibleRoutine());
        if (isServer)
        {
            PhaseManager.Instance.PhaseDictionary[EPhaseType.ServingPhase].OnPhaseExited += ResetData;
            PhaseManager.Instance.PhaseDictionary[EPhaseType.PracticingPhase].OnPhaseExited += ResetData;
        }
    }

    public override void OnStopClient()
    {
        base.OnStopClient();

        if (isServer)
        {
            PhaseManager.Instance.PhaseDictionary[EPhaseType.ServingPhase].OnPhaseExited -= ResetData;
            PhaseManager.Instance.PhaseDictionary[EPhaseType.PracticingPhase].OnPhaseExited -= ResetData;
        }
    }

    #region SyncVar Hook Functions
    private void OnDataTIDChanged(int oldTID, int newTID)
    {
        _data = DataTable.Instance.GetFurnitureData(newTID);
        ActivateModelForTID(newTID);
        _refundSystem.InitRefundSyStem(DataTable.Instance.GetFurnitureData(newTID).StructureTID, this);
        OnDataChanged?.Invoke();
        Debug.Log($"Client: Furniture Data (TID: {newTID}) loaded.");
    }

    private void OnCurrentRotationChanged(float oldVal, float newVal)
    {
        _model.rotation = Quaternion.Euler(0, newVal, 0);
        OnDataChanged?.Invoke();
    }

    private void OnInputObjectChanged(GameObject oldObj, GameObject newObj)
    {
        if (newObj != null)
        {
            newObj.transform.position = _inputPosition.position;
        }
        OnDataChanged?.Invoke();
    }


    public void OnRefundProgressChanged(float oldValue, float newValue)
    {
        OnDataChanged?.Invoke();
    }
    #endregion

    #region Server-Only Methods
    [Server]
    public void ServerInitFurniture(int furnitureTID)
    {
        DataTID = furnitureTID;
        _data = DataTable.Instance.GetFurnitureData(furnitureTID);
        CurrentRotation = 0f;

        switch (_data.SpecialStructureType)
        {
            case ESpecialStructureType.Casher:
                _interactComponent = new CasherInteract();
                break;
            case ESpecialStructureType.PickUpTable:
                _inputComponent = new PickUpTableInputContainer();
                _outputComponent = new PickUpTableOutputContainer();
                break;
            case ESpecialStructureType.OldChair:
            case ESpecialStructureType.LuxuryChair:
                _effectComponent = new ChairEffect();
                break;
            case ESpecialStructureType.TrashCan:
                _inputComponent = new TrashCanInputContainer();
                break;

        }

        _refundSystem.InitRefundSyStem(DataTable.Instance.GetFurnitureData(furnitureTID).StructureTID, this);
        OnDataChanged?.Invoke();
        Debug.Log($"Server: Furniture (TID: {furnitureTID}) initialized.");
    }

    [Server]
    public void ServerRotateModel()
    {
        CurrentRotation += 90f;
        if (CurrentRotation >= 360f)
        {
            CurrentRotation = 0;
        }
        _model.rotation = Quaternion.Euler(0, _currentRotation, 0);
    }
    #endregion

    #region Commands
    [Command(requiresAuthority = false)]
    private void CmdTryInteract(NetworkConnectionToClient sender = null)
    {
        if (isServer == false)
        {
            return;
        }

        bool success = false;
        EPhaseType currentPhase = PhaseManager.Instance.CurrentPhase.PhaseType;

        if (currentPhase == EPhaseType.PreparingPhase)
        {
            // 임시 코드
            Debug.Log("임시 코드");
            if (_data.SpecialStructureType == ESpecialStructureType.Casher)
            {
                Test_MarketSingleton.Instance.ShowHideMarket();
                TargetRpcOnInteract(sender, true);
                return;
            }

            ServerRotateModel();
            success = true;
        }
        else
        {
            if (ReferenceEquals(_interactComponent, null) == false)
            {
                success = _interactComponent.ServerTryInteract(this);
            }
        }

        TargetRpcOnInteract(sender, success);
    }

    [Command(requiresAuthority = false)]
    private void CmdTryPickUp(NetworkConnectionToClient sender = null)
    {
        if (isServer == false)
        {
            return;
        }

        GameObject pickedUpItem = null;
        EPhaseType currentPhase = PhaseManager.Instance.CurrentPhase.PhaseType;

        if (currentPhase == EPhaseType.PreparingPhase)
        {
            // 임시 코드
            Debug.Log("임시 코드");
            if (_data.SpecialStructureType == ESpecialStructureType.Casher ||
                _data.SpecialStructureType == ESpecialStructureType.Practice)
            {
                return;
            }
            // GridManager의 서버 전용 메서드를 호출하여 서버에서 그리드 정보만 제거하고 오브젝트를 반환받음
            pickedUpItem = GridManager.Instance.ServerRemovePlacementDataOnly(transform.position);

            // 반환받은 오브젝트가 있다면, 픽업한 플레이어에게 권한을 할당함
            if (pickedUpItem != null && sender != null)
            {
                NetworkServer.spawned[pickedUpItem.GetComponent<NetworkIdentity>().netId].AssignClientAuthority(sender);
                //GetComponent<NetworkTransformReliable>().syncMode = SyncMode.Owner;

                // GridManager의 TargetRpc를 호출하여 클라이언트에서 배치 상태를 시작하도록 지시
                GridManager.Instance.TargetRpcStartPlacement(sender, pickedUpItem.GetComponent<NetworkIdentity>().netId);
            }
        }
        else
        {
            if (ReferenceEquals(_outputComponent, null) == false)
            {
                if (_outputComponent.ServerCanTake(this))
                {
                    pickedUpItem = _outputComponent.ServerTakeItem(this);
                }
            }
        }

        if (pickedUpItem != null && sender != null)
        {
            NetworkServer.spawned[pickedUpItem.GetComponent<NetworkIdentity>().netId].AssignClientAuthority(sender);
        }

        if (pickedUpItem != null)
        {
            TargetRpcOnPickUp(sender, pickedUpItem.GetComponent<NetworkIdentity>());
        }
    }

    [Command(requiresAuthority = false)]
    private void CmdTryDrop(Vector3 targetPosition, uint dropItemNetId, int tid, EInputType inputType, NetworkConnectionToClient sender = null)
    {
        if (isServer == false)
        {
            return;
        }

        bool success = false;
        EPhaseType currentPhase = PhaseManager.Instance.CurrentPhase.PhaseType;

        if (NetworkServer.spawned.TryGetValue(dropItemNetId, out NetworkIdentity dropItemIdentity))
        {
            GameObject inputObject = dropItemIdentity.gameObject;

            if (currentPhase == EPhaseType.PreparingPhase)
            {
                // 임시 코드
                Debug.Log("임시 코드");
                if (_data.SpecialStructureType == ESpecialStructureType.Casher ||
                    _data.SpecialStructureType == ESpecialStructureType.Practice)
                {
                    TargetRpcOnDrop(sender, false, transform.position);
                    return;
                }
                if (GridManager.Instance.ServerCanPlaceObjectAt(targetPosition, _data.AreaType))
                {
                    transform.position = targetPosition;
                    GridManager.Instance.ServerPlaceStructure2(targetPosition, dropItemNetId, sender);
                    dropItemIdentity.RemoveClientAuthority();
                    RpcOnDrop();
                    success = true;
                }
                else
                {
                    success = false;
                }
            }
            else
            {
                if (ReferenceEquals(_inputComponent, null) == false)
                {
                    success = _inputComponent.ServerTryInput(this, tid, inputType, inputObject);
                }
            }
        }

        if (success)
        {
            TargetRpcOnDrop(sender, success, transform.position);
        }
    }

    [Command(requiresAuthority = false)]
    private void CmdCustomerEffect(uint customerNetId)
    {
        if (isServer == false)
        {
            return;
        }

        if (NetworkServer.spawned.TryGetValue(customerNetId, out NetworkIdentity customerIdentity))
        {
            if (ReferenceEquals(_effectComponent, null) == false)
            {
                _effectComponent.ServerEffect(this, customerIdentity);
            }
        }
    }

    [Command(requiresAuthority = false)]
    private void CmdStartRefund()
    {
        _refundSystem.ServerStartRefund(connectionToClient);
    }

    [Command(requiresAuthority = false)]
    private void CmdCancelRefund()
    {
        _refundSystem.ServerCancelRefund();
    }

    private void CmdCustomerPickup()
    {
        if(isServer == false)
        {
            return;
        }

        if (ReferenceEquals(_outputComponent, null) == false)
        {
            _outputComponent.ServerTakeItem(this) ;
        }
        
    }
    #endregion

    #region TargetRpc
    [TargetRpc]
    private void TargetRpcOnInteract(NetworkConnectionToClient target, bool success)
    {
        if (NetworkClient.connection.identity.TryGetComponent<Player>(out Player player))
        {
            player.GetAbility<PlayerInteractAbility>().ReceiveInteractResult(success);
        }
    }

    [TargetRpc]
    private void TargetRpcOnPickUp(NetworkConnectionToClient target, NetworkIdentity itemNetId)
    {
        if (NetworkClient.connection.identity.TryGetComponent<Player>(out Player player))
        {
            player.GetAbility<PlayerPickupAbility>().ReceivePickedUpItem(itemNetId);
        }
    }

    [TargetRpc]
    private void TargetRpcOnDrop(NetworkConnectionToClient target, bool success, Vector3 position)
    {
        //transform.position = _grid.CellToWorld(gridPosition) + new Vector3(0.5f, 0, 0.5f);
        if (NetworkClient.connection.identity.TryGetComponent<Player>(out Player player))
        {
            player.GetAbility<PlayerPickupAbility>().ReceiveDroppedItem(success);
        }
    }
    #endregion

    [ClientRpc]
    private void RpcOnDrop()
    {
        gameObject.SetActive(false);
    }

    #region Public Interface (IGridItemHandler)
    private void ActivateModelForTID(int tid)
    {
        if (_modelObjectDic == null) return;
        foreach (var modelInfo in _modelObjectList)
        {
            modelInfo.Model.SetActive(false);
        }
        if (_modelObjectDic.TryGetValue(tid, out GameObject modelToActivate))
        {
            modelToActivate.SetActive(true);
        }
    }

    public void TryInteract(NetworkConnectionToClient conn)
    {
        CmdTryInteract(conn);
    }

    public void TryPickUp(NetworkConnectionToClient conn)
    {
        CmdTryPickUp(conn);
    }

    public void TryDrop(NetworkConnectionToClient conn, Vector3 targetPosition, NetworkIdentity inputNetId, int tid = 10000, EInputType inputType = EInputType.None)
    {
        if (inputNetId != null)
        {
            CmdTryDrop(targetPosition, inputNetId.netId, tid, inputType);
        }
    }

    public void TryCustomerEffect(uint customerNetId)
    {
        CmdCustomerEffect(customerNetId);
    }

    public void TryCustomerPickup()
    {
        CmdCustomerPickup();
    }

    [Server]
    public void ResetData()
    {
        if (!isServer) return;

        if (!ReferenceEquals(InputObject, null))
        {
            CraftItemFactory.Instance.ReturnObject(InputObject);
            InputObject = null;
        }
    }

    public int GetStructureTID()
    {
        return _data.StructureTID;
    }

    public void SetCollider(bool active)
    {
        _collider.enabled = active;
    }

    public void StartRefund()
    {
        CmdStartRefund();
    }

    public void CancelRefund()
    {
        CmdCancelRefund();
    }
    #endregion

    private IEnumerator VisibleRoutine()
    {
        yield return new WaitForSeconds(0.05f);
        _model.gameObject.SetActive(true);
    }
}
