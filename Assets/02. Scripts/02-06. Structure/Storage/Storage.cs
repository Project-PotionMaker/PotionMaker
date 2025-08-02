using Mirror;
using System;
using System.Collections.Generic;
using UnityEngine;
using VInspector;

/// <summary>
/// 아이템을 보관하고 플레이어가 아이템을 가져갈 수 있는 기능을 담당하는 클래스입니다.
/// Machine과 동일하게 SyncVar를 사용하여 네트워크 동기화를 처리합니다.
/// </summary>
public class Storage : NetworkBehaviour, IGridItemHandler
{
    [SyncVar(hook = nameof(OnDataTIDChanged))]
    private int _dataTID;
    public int DataTID { get => _dataTID; private set => _dataTID = value; }

    [SyncVar(hook = nameof(OnIngredientTIDChanged))]
    private int _ingredientTID;
    public int IngredientTID { get => _ingredientTID; private set => _ingredientTID = value; }

    [SyncVar(hook = nameof(OnCurrentRotationChanged))]
    private float _currentRotation;
    public float CurrentRotation { get => _currentRotation; private set => _currentRotation = value; }

    private StorageData _data;
    public StorageData Data => _data;

    [SerializeField]
    private Transform _model;

    private IOutputContainer<Storage> _outputComponent;

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

        OnIngredientTIDChanged(0, _ingredientTID);
        OnCurrentRotationChanged(0, _currentRotation);
        OnDataTIDChanged(0, _dataTID);
    }

    #region SyncVar Hook Functions

    private void OnDataTIDChanged(int oldTID, int newTID)
    {
        _data = DataTable.Instance.GetStorageData(newTID);
        ActivateModelForTID(newTID);
        OnDataChanged?.Invoke();
        Debug.Log($"Client: Storage Data (TID: {newTID}) loaded.");
    }

    private void OnIngredientTIDChanged(int oldTID, int newTID)
    {
        // UI 업데이트 등 추가 로직 구현
        OnDataChanged?.Invoke();
        Debug.Log($"Client: Ingredient (TID: {newTID}) changed.");
    }

    private void OnCurrentRotationChanged(float oldVal, float newVal)
    {
        _model.rotation = Quaternion.Euler(0, newVal, 0);
        OnDataChanged?.Invoke();
    }

    #endregion

    #region Server-Only Methods
    [Server]
    public void ServerInitStorage(int storageTID, int ingredientTID)
    {
        DataTID = storageTID;
        _data = DataTable.Instance.GetStorageData(storageTID);
        IngredientTID = ingredientTID;
        CurrentRotation = 0f;

        _outputComponent = new StorageOutputContainer();

        OnDataChanged?.Invoke();
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

    [Server]
    public void ResetData()
    {
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

        bool result = false;
        EPhaseType currentPhase = PhaseManager.Instance.CurrentPhase.PhaseType;

        if (currentPhase == EPhaseType.PreparingPhase)
        {
            ServerRotateModel();
            result = true;
        }

        TargetRpcOnInteract(sender, result);
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

            dropItemIdentity.RemoveClientAuthority();
        }

        TargetRpcOnDrop(sender, success);
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
}
