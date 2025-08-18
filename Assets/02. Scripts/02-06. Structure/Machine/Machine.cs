using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VInspector;

[Serializable]
public class ModelOnTID
{
    public int TID;
    public GameObject Model;
}

public class Machine : NetworkBehaviour, IGridItemHandler, IRefundable
{
    [SerializeField]
    private Collider _collider;
    [SerializeField]
    private Transform _model;
    [SerializeField]
    private Transform _putItemPosition;
    public Transform PutItemPosition => _putItemPosition;

    private MachineData _data;
    public MachineData Data => _data;

    #region SyncVar Variables (Public Get, Private Set)

    // Machine Data ID (클라이언트에서 MachineData를 로드하는 데 사용)
    [SyncVar(hook = nameof(OnDataTIDChanged))]
    private int _dataTID;
    public int DataTID { get => _dataTID; private set => _dataTID = value; }

    // 진행도
    [SyncVar(hook = nameof(OnCurrentProgressChanged))]
    private float _currentProgress;
    public float CurrentProgress { get => _currentProgress; private set => _currentProgress = value; }

    // 남은 생산량
    [SyncVar(hook = nameof(OnLeftOutputAmountChanged))]
    private int _leftOutputAmount;
    public int LeftOutputAmount { get => _leftOutputAmount; private set => _leftOutputAmount = value; }

    // 공정 완료 여부
    [SyncVar(hook = nameof(OnIsProcessFinishedChanged))]
    private bool _isProcessFinished;
    public bool IsProcessFinished { get => _isProcessFinished; private set => _isProcessFinished = value; }

    // 공정 시작 여부
    [SyncVar(hook = nameof(OnIsProcessStartedChanged))]
    private bool _isProcessStarted;
    public bool IsProcessStarted { get => _isProcessStarted; private set => _isProcessStarted = value; }

    // 모델 회전 값 (준비 페이즈에서만 변경)
    [SyncVar(hook = nameof(OnCurrentRotationChanged))]
    private float _currentRotation;
    public float CurrentRotation { get => _currentRotation; private set => _currentRotation = value; }

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

    // 투입된 아이템 TID 리스트 (SyncList 사용 권장)
    public readonly SyncList<int> InputTIDList = new SyncList<int>();

    // 투입 타입 (예: 한 번에 여러 개 넣는지 등)
    [SyncVar(hook = nameof(OnInputTypeChanged))]
    private EInputType _inputType;
    public EInputType InputType { get => _inputType; private set => _inputType = value; }


    #endregion

    private IInteractable<Machine> _interactComponent;
    private IInputContainer<Machine> _inputComponent;
    private IOutputContainer<Machine> _outputComponent;

    private Renderer[] _modelRenderers;
    private MaterialPropertyBlock _matPropertyBlock;

    [Foldout("Project")]
    [SerializeField]
    private List<ModelOnTID> _modelObjectList = new List<ModelOnTID>();
    private Dictionary<int, GameObject> _modelObjectDic;

    private RefundSystem _refundSystem;
    public Action OnDataChanged;

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
        _matPropertyBlock = new MaterialPropertyBlock();
    }

    public override void OnStartServer()
    {
        base.OnStartServer();
    }

    public override void OnStartClient()
    {
        base.OnStartClient();
        _model.gameObject.SetActive(false);
        if (!ReferenceEquals(_visibleRoutine, null))
        {
            StopCoroutine(_visibleRoutine);
        }
        _visibleRoutine = StartCoroutine(VisibleRoutine());
        if (!isServer)
        {
            InputTIDList.Callback += OnInputTIDListChanged;
        }
        else
        {
            InputTIDList.Callback += OnInputTIDListChanged;
            PhaseManager.Instance.PhaseDictionary[EPhaseType.ServingPhase].OnPhaseExited += ResetData; // 서버에서만 호출되도록
            PhaseManager.Instance.PhaseDictionary[EPhaseType.PracticingPhase].OnPhaseExited += ResetData; // 서버에서만 호출되도록

        }
    }

    public override void OnStopClient()
    {
        base.OnStopClient();
        if (!isServer)
        {
            InputTIDList.Callback -= OnInputTIDListChanged;
        }
        else
        {
            PhaseManager.Instance.PhaseDictionary[EPhaseType.ServingPhase].OnPhaseExited -= ResetData;
            PhaseManager.Instance.PhaseDictionary[EPhaseType.PracticingPhase].OnPhaseExited -= ResetData;
        }
    }

    #region SyncVar Hook Functions (클라이언트에서 SyncVar 변경 시 호출됨)
    private void OnDataTIDChanged(int oldTID, int newTID)
    {
        // 클라이언트에서 _data 초기화.
        _data = DataTable.Instance.GetMachineData(newTID);
        ActivateModelForTID(newTID); // 모델 활성화
        _refundSystem.InitRefundSyStem(DataTable.Instance.GetMachineData(newTID).StructureTID, this);

        // 인터페이스 컴포넌트 초기화 (클라이언트에서도 Data를 기반으로)
        _interactComponent = GetInteractableComponent(_data.InteractType);
        _inputComponent = new MachineInputContainer();
        _outputComponent = new MachineOutputContainer();

        OnDataChanged?.Invoke(); // UI 업데이트를 위한 액션 호출
        Debug.Log($"Client: Machine Data (TID: {newTID}) loaded.");
    }

    private void OnCurrentProgressChanged(float oldVal, float newVal)
    {
        OnDataChanged?.Invoke(); // UI 게이지 업데이트
        Debug.Log($"Client: Progress updated to {newVal:F2}");
    }

    private void OnLeftOutputAmountChanged(int oldVal, int newVal)
    {
        OnDataChanged?.Invoke(); // UI 숫자 업데이트
        Debug.Log($"Client: LeftOutputAmount updated to {newVal}");
    }

    private void OnIsProcessFinishedChanged(bool oldVal, bool newVal)
    {
        OnDataChanged?.Invoke(); // UI 상태 업데이트
        Debug.Log($"Client: IsProcessFinished updated to {newVal}");
    }

    private void OnIsProcessStartedChanged(bool oldVal, bool newVal)
    {
        OnDataChanged?.Invoke(); // UI 상태 업데이트 (애니메이션 시작/중지 등)
        Debug.Log($"Client: IsProcessStarted updated to {newVal}");
    }

    private void OnCurrentRotationChanged(float oldVal, float newVal)
    {
        _model.rotation = Quaternion.Euler(0, newVal, 0); // 클라이언트 모델 회전 업데이트
        OnDataChanged?.Invoke();
    }

    private void OnInputTIDListChanged(SyncList<int>.Operation op, int itemIndex, int oldItem, int newItem)
    {
        OnDataChanged?.Invoke(); // UI (인풋 슬롯) 업데이트
        Debug.Log($"InputTIDList changed: {op}, Index: {itemIndex}, Old: {oldItem}, New: {newItem}");
    }

    private void OnInputTypeChanged(EInputType oldVal, EInputType newVal)
    {
        OnDataChanged?.Invoke();
        Debug.Log($"Client: InputType updated to {newVal}");
    }

    public void OnRefundProgressChanged(float oldValue, float newValue)
    {
        OnDataChanged?.Invoke();
    }
    #endregion

    #region Server-Only Methods (Called by Commands or other Server logic)

    // Machine 초기화 (서버에서만 호출)
    [Server]
    public void ServerInitMachine(int machineTID)
    {
        DataTID = machineTID; // SyncVar 설정 (클라이언트에 동기화됨)
        _data = DataTable.Instance.GetMachineData(machineTID); // 서버에서 _data 로드

        // 나머지 SyncVar 초기값 설정 (서버에서만)
        CurrentProgress = 0f;
        LeftOutputAmount = 0;
        IsProcessFinished = false;
        IsProcessStarted = false;
        CurrentRotation = 0f;
        InputTIDList.Clear(); // SyncList 초기화 (서버에서만)
        InputType = EInputType.None;

        // 인터페이스 컴포넌트 초기화 (서버에서도 Data를 기반으로)
        _interactComponent = GetInteractableComponent(_data.InteractType);
        _inputComponent = new MachineInputContainer();
        _outputComponent = new MachineOutputContainer();
        _refundSystem.InitRefundSyStem(DataTable.Instance.GetMachineData(machineTID).StructureTID, this);

        ActivateModelForTID(_data.TID); // 모델 활성화
        OnDataChanged?.Invoke(); // 초기화 후 UI 업데이트를 위한 액션 호출
    }

    // Process Started/Finished 상태를 변경하는 서버 메서드
    [Server]
    public void ServerSetIsProcessStarted(bool value)
    {
        IsProcessStarted = value;
    }

    [Server]
    public void ServerSetIsProcessFinished(bool value)
    {
        IsProcessFinished = value;
    }

    // CurrentProgress를 증가/변경하는 서버 메서드
    [Server]
    public void ServerIncreaseProgress(float amount)
    {
        // _currentProgress의 private set 접근은 이 클래스 내부에서만 허용
        CurrentProgress += amount;
        CurrentProgress = Mathf.Clamp(CurrentProgress, 0, Data.MaxProgress);

        // 진행도에 따른 상태 변경 (서버에서)
        if (CurrentProgress >= Data.MaxProgress)
        {
            ServerSetIsProcessFinished(true);
            ServerSetIsProcessStarted(false);
            ServerSetLeftOutputAmount(Data.OutputAmount);
            StopAllCoroutines(); // 서버의 코루틴만 중지
        }
    }

    // LeftOutputAmount를 변경하는 서버 메서드
    [Server]
    public void ServerSetLeftOutputAmount(int value)
    {
        LeftOutputAmount = value;
    }

    // LeftOutputAmount를 감소시키는 서버 메서드
    [Server]
    public void ServerDecreaseOutputAmount(int amount)
    {
        LeftOutputAmount -= amount;
        if (LeftOutputAmount < 0) LeftOutputAmount = 0;
    }

    // CurrentRotation을 변경하는 서버 메서드 (준비 페이즈 로직)
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

    // InputTIDList에 아이템을 추가하는 서버 메서드
    [Server]
    public void ServerAddInputTID(int tid)
    {
        InputTIDList.Add(tid); // SyncList는 서버에서 변경하면 자동으로 클라이언트에 동기화
        // 추가 유효성 검사 및 로직
    }

    // InputTIDList에서 아이템을 제거하는 서버 메서드 (인덱스로)
    [Server]
    public void ServerRemoveInputTIDAt(int index)
    {
        if (index >= 0 && index < InputTIDList.Count)
        {
            InputTIDList.RemoveAt(index);
        }
    }

    // InputType을 변경하는 서버 메서드
    [Server]
    public void ServerSetInputType(EInputType type)
    {
        InputType = type;
    }

    // 머신 상태 리셋 (서버에서만 호출)
    [Server]
    public void ResetData()
    {
        StopAllCoroutines(); // 서버의 코루틴만 중지
        LeftOutputAmount = 0; // SyncVar 초기화
        IsProcessFinished = false; // SyncVar 초기화
        IsProcessStarted = false; // SyncVar 초기화
        CurrentProgress = 0f; // SyncVar 초기화
        InputTIDList.Clear(); // SyncList 초기화
        InputType = EInputType.None; // SyncVar 초기화
    }

    #endregion

    #region Commands (클라이언트에서 서버로 요청)
    // 상호작용 요청 커맨드 (클라이언트에서 호출)
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
        else
        {
            if (ReferenceEquals(_interactComponent, null) == false)
            {
                result = _interactComponent.ServerTryInteract(this);
            }
        }

        TargetRpcOnInteract(sender, result);
    }

    [TargetRpc]
    private void TargetRpcOnInteract(NetworkConnectionToClient target, bool success)
    {
        if (NetworkClient.connection.identity.TryGetComponent<Player>(out Player player))
        {
            player.GetAbility<PlayerInteractAbility>().ReceiveInteractResult(success);
        }
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
                //GetComponent<NetworkTransformReliable>().syncMode = SyncMode.Owner;
                // GridManager의 TargetRpc를 호출하여 클라이언트에서 배치 상태를 시작하도록 지시
                GridManager.Instance.TargetRpcStartPlacement(sender, pickedUpItem.GetComponent<NetworkIdentity>().netId);
            }
        }
        else
        {
            if (ReferenceEquals(_outputComponent, null) == false)
            {
                pickedUpItem = _outputComponent.ServerTakeItem(this);
            }
        }

        if (pickedUpItem != null && sender != null)
        {
            NetworkServer.spawned[pickedUpItem.GetComponent<NetworkIdentity>().netId].AssignClientAuthority(sender);
            TargetRpcOnPickUp(sender, pickedUpItem.GetComponent<NetworkIdentity>());
            pickedUpItem.GetComponent<OutputItem>()?.TargetRpcSetFocus(sender, true);
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

    [Command(requiresAuthority = false)]
    private void CmdTryDrop(Vector3 targetPosition, int tid, EInputType inputType, uint dropItemNetId, NetworkConnectionToClient sender = null)
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
                if (GridManager.Instance.ServerCanPlaceObjectAt(targetPosition, _data.AreaType))
                {
                    transform.position = targetPosition;
                    GridManager.Instance.ServerPlaceStructure(targetPosition, dropItemNetId, sender);
                    dropItemIdentity.RemoveClientAuthority();
                    success = true;
                    RpcOnDrop();
                }
                else
                {
                    success = false;
                }
            }
            else
            {
                success = _inputComponent.ServerTryInput(this, tid, inputType, inputObject);
                if (success)
                {
                    CraftItemFactory.Instance.ReturnObject(inputObject);
                }
            }

        }

        if (success)
        {
            TargetRpcOnDrop(sender, success);
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

    [ClientRpc]
    private void RpcOnDrop()
    {
        gameObject.SetActive(false);
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
    #endregion

    // 모델 활성화/비활성화
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
            _modelRenderers = GetComponentsInChildren<Renderer>();
        }
    }

    private IInteractable<Machine> GetInteractableComponent(EInteractType interactType)
    {
        switch (interactType)
        {
            case EInteractType.KeepPressing:
                // 수정 필요
                return new AutoProgressInteract();
            case EInteractType.AutoProgress:
                return new AutoProgressInteract();
            case EInteractType.ClickRepeatly:
                return new ClickRepeatlyInteract();
            case EInteractType.ClickOnce:
                return new ClickOnceInteract();
        }

        return null;
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
            CmdTryDrop(targetPosition, tid, inputType, inputNetId.netId, conn);
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

    private IEnumerator VisibleRoutine()
    {
        yield return new WaitForSeconds(0.05f);
        _model.gameObject.SetActive(true);
    }

    private Color emissionColor = new Color(0.25f, 0.25f, 0.25f);
    public void SetHighlight(bool active)
    {
        if (active)
        {
            _matPropertyBlock.SetColor("_EmissionColor", emissionColor);
        }
        else
        {
            _matPropertyBlock.SetColor("_EmissionColor", Color.black);
        }

        foreach (var renderer in _modelRenderers)
        {
            renderer?.SetPropertyBlock(_matPropertyBlock);
        }
    }
}
