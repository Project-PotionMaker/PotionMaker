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
            // GridManager의 서버 전용 메서드를 호출하여 서버에서 그리드 정보만 제거하고 오브젝트를 반환받음
            pickedUpItem = GridManager.Instance.ServerRemovePlacementDataOnly(transform.position);

            // 반환받은 오브젝트가 있다면, 픽업한 플레이어에게 권한을 할당함
            if (pickedUpItem != null && sender != null)
            {
                NetworkServer.spawned[pickedUpItem.GetComponent<NetworkIdentity>().netId].AssignClientAuthority(sender);
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

            if (GridManager.Instance.ServerCanPlaceObjectAt(targetPosition, EAreaType.Storage))
            {
                GridManager.Instance.ServerPlaceStructure(targetPosition, dropItemNetId, sender);
                success = true;
            }
            else
            {
                success = false;
            }
        }

        if (success)
        {
            dropItemIdentity.RemoveClientAuthority();
        }


        TargetRpcOnDrop(sender, success);
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
    private void TargetRpcOnDrop(NetworkConnectionToClient target, bool success)
    {
        if (NetworkClient.connection.identity.TryGetComponent<Player>(out Player player))
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

    public void TryDrop(NetworkConnectionToClient conn, Vector3 targetPosition, NetworkIdentity inputNetId, int tid = 10000, EInputType inputType = EInputType.None)
    {
        if (inputNetId != null)
        {
            CmdTryDrop(targetPosition, inputNetId.netId, tid, inputType, conn);
        }
    }

    public int GetStructureTID()
    {
        return _data.StructureTID;
    }
}
