using Mirror;
using System;
using System.Collections.Generic;
using UnityEngine;
using VInspector;

/// <summary>
/// 플레이어가 상호작용할 수 있는 가구 클래스입니다.
/// Storage와 마찬가지로 SyncVar를 사용하여 네트워크 동기화를 처리합니다.
/// </summary>
public class Furniture : NetworkBehaviour, IGridItemHandler
{
    [SyncVar(hook = nameof(OnDataTIDChanged))]
    private int _dataTID;
    public int DataTID { get => _dataTID; private set => _dataTID = value; }

    [SyncVar(hook = nameof(OnCurrentRotationChanged))]
    private float _currentRotation;
    public float CurrentRotation { get => _currentRotation; private set => _currentRotation = value; }

    [SyncVar(hook = nameof(OnInputObjectChanged))]
    private GameObject _inputObject;
    public GameObject InputObject { get => _inputObject; private set => _inputObject = value; }

    private FurnitureData _data;
    public FurnitureData Data => _data;

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

    public Action OnDataChanged;

    [Foldout("Project")]
    [SerializeField]
    private List<ModelOnTID> _modelObjectList = new List<ModelOnTID>();
    private Dictionary<int, GameObject> _modelObjectDic;

    private void Awake()
    {
        _modelObjectDic = new Dictionary<int, GameObject>();
        foreach (var modelInfo in _modelObjectList)
        {
            modelInfo.Model.SetActive(false);
            _modelObjectDic.Add(modelInfo.TID, modelInfo.Model);
        }
    }

    public override void OnStartClient()
    {
        base.OnStartClient();

        if (isServer)
        {
            PhaseManager.Instance.PhaseDictionary[EPhaseType.ServingPhase].OnPhaseExited += ResetData;
            PhaseManager.Instance.PhaseDictionary[EPhaseType.PracticingPhase].OnPhaseExited += ResetData;
        }

        OnDataTIDChanged(0, _dataTID);
        OnCurrentRotationChanged(0, _currentRotation);
        OnInputObjectChanged(null, _inputObject);
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
    #endregion

    #region Server-Only Methods
    [Server]
    public void ServerInitFurniture(int furnitureTID)
    {
        DataTID = furnitureTID;
        _data = DataTable.Instance.GetFurnitureData(furnitureTID);
        CurrentRotation = 0f;

        // 서버에서만 인터페이스 컴포넌트들을 할당
        // 테스트용
        if (_data.SpecialStructureType == ESpecialStructureType.Casher)
        {
            _interactComponent = new CasherInteract();
        }
        if (_data.SpecialStructureType == ESpecialStructureType.PickUpTable)
        {
            _inputComponent = new PickUpTableInputContainer();
            _outputComponent = new PickUpTableOutputContainer();
        }
        if (_data.SpecialStructureType == ESpecialStructureType.OldChair || _data.SpecialStructureType == ESpecialStructureType.LuxuryChair)
        {
            _effectComponent = new ChairEffect();
        }

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
            pickedUpItem = GridManager.Instance.StartPlacement(transform.position);
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

        TargetRpcOnPickUp(sender, pickedUpItem);
    }

    [Command(requiresAuthority = false)]
    private void CmdTryDrop(NetworkConnectionToClient sender, Vector3 targetPosition, uint dropItemNetId, int tid, EInputType inputType)
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
                if (GridManager.Instance.TryPlaceStructure(targetPosition))
                {
                    success = true;
                }
            }
            else
            {
                if (ReferenceEquals(_inputComponent, null) == false)
                {
                    success = _inputComponent.ServerTryInput(this, tid, inputType, inputObject);
                    if (success)
                    {
                        InputObject = inputObject;
                    }
                }
            }

            dropItemIdentity.RemoveClientAuthority();
        }

        TargetRpcOnDrop(sender, success);
    }

    [Command(requiresAuthority = false)]
    private void CmdTryEffect(uint customerNetId)
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
    #endregion

    #region TargetRpc
    [TargetRpc]
    private void TargetRpcOnInteract(NetworkConnectionToClient target, bool success)
    {
        if (target.identity.TryGetComponent<Player>(out Player player))
        {
            player.GetAbility<PlayerInteractAbility>().ReceiveInteractResult(success);
        }
    }

    [TargetRpc]
    private void TargetRpcOnPickUp(NetworkConnectionToClient target, GameObject item)
    {
        if (target.identity.TryGetComponent<Player>(out Player player))
        {
            player.GetAbility<PlayerPickupAbility>().ReceivePickedUpItem(item);
        }
    }

    [TargetRpc]
    private void TargetRpcOnDrop(NetworkConnectionToClient target, bool success)
    {
        if (target.identity.TryGetComponent<Player>(out Player player))
        {
            player.GetAbility<PlayerPickupAbility>().ReceiveDroppedItem(success);
        }
    }
    #endregion

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

    public void TryDrop(NetworkConnectionToClient conn, Vector3 targetPosition, GameObject inputObject, int tid = 10000, EInputType inputType = EInputType.None)
    {
        if (inputObject != null && inputObject.TryGetComponent<NetworkIdentity>(out NetworkIdentity netIdentity))
        {
            CmdTryDrop(conn, targetPosition, netIdentity.netId, tid, inputType);
        }
    }

    public void TryEffect(uint customerNetId)
    {
        CmdTryEffect(customerNetId);
    }

    [Server]
    public void ResetData()
    {
        if (!isServer) return;

        if (!ReferenceEquals(InputObject, null))
        {
            CraftItemFactory.Instance.CmdReturn(InputObject);
            InputObject = null;
        }
    }
    #endregion
}
