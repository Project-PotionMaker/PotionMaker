using Mirror;
using UnityEngine;

public class PlayerPickupAbility : PlayerAbility
{
    [SyncVar(hook = nameof(OnHeldItemChanged))]
    private NetworkIdentity _heldItemIdentity;
    public NetworkIdentity HeldItemIdentity => _heldItemIdentity;
    private PlayerAnimationAbility _animationAbility;

    // 영상 임시
    private CanvasGroup _lastHighlightedStructureCanvas;

    private void Start()
    {
        if (!_owner.isLocalPlayer)
        {
            return;
        }

        InputManager.Instance.OnPickupEvent += OnPickupInput;
        _animationAbility = _owner.GetAbility<PlayerAnimationAbility>();
        PhaseManager.Instance.PhaseDictionary[EPhaseType.ServingPhase].OnPhaseExited += ResetItem;
        PhaseManager.Instance.PhaseDictionary[EPhaseType.PracticingPhase].OnPhaseExited += ResetItem;
    }

    private void Update()
    {
        if (!_owner.isLocalPlayer)
        {
            return;
        }

        if (_heldItemIdentity != null)
        {
            Vector3 targetPosition = _owner.GetFrontPosition();
            GridManager.Instance.UpdatePlacementPosition(targetPosition);
        }
    }

    private void OnPickupInput()
    {
        if (_heldItemIdentity == null)
        {
            TryPickup();
        }
        else
        {
            TryPutDown();
        }

        bool hasHeldItem = _heldItemIdentity != null;
        _animationAbility.SetBool(EPlayerAnimationParameter.HasHeldItem, hasHeldItem);
    }

    private void TryPickup()
    {
        GameObject item = FindFrontPickupItem();
        if (item != null)
        {
            IGridItemHandler itemHandler = item.GetComponent<IGridItemHandler>();
            if (ReferenceEquals(itemHandler, null) == false)
            {
                itemHandler.TryPickUp(_owner.connectionToClient);
            }
        }
    }

    private void TryPutDown()
    {
        Vector3 targetPosition = _owner.GetFrontPosition();

        EPhaseType phaseType = PhaseManager.Instance.CurrentPhase.PhaseType;

        if (phaseType == EPhaseType.PreparingPhase)
        {
            IGridItemHandler itemHandler = _heldItemIdentity.gameObject.GetComponent<IGridItemHandler>();
            if (ReferenceEquals(itemHandler, null) == true)
            {
                return;
            }

            itemHandler.TryDrop(_owner.connectionToClient, targetPosition, _heldItemIdentity);
        }
        else if (phaseType == EPhaseType.ServingPhase || phaseType == EPhaseType.PracticingPhase)
        {
            GameObject gridObject = FindFrontPickupItem();
            if (gridObject != null)
            {
                IItem item = _heldItemIdentity.gameObject.GetComponent<IItem>();
                if (ReferenceEquals(item, null) == false)
                {
                    gridObject.GetComponent<IGridItemHandler>().TryDrop(
                        _owner.connectionToClient,
                        targetPosition,
                        _heldItemIdentity,
                        item.GetTID(),
                        item.GetInputType()
                        );
                }
            }
        }
    }

    private GameObject FindFrontPickupItem()
    {
        if (_owner.GetObjectInFront() == null)
        {
            return null;
        }

        Vector3 targetPosition = _owner.GetFrontPosition();
        GameObject item = GridManager.Instance.GetObjectOnGrid(targetPosition);
        return item;
    }

    private void ResetItem()
    {
        if (!ReferenceEquals(_heldItemIdentity, null))
        {
            _heldItemIdentity = null;
        }
    }

    [Client]
    public void ReceiveRefundCompleted()
    {
        ResetItem();
    }

    [Client]
    public void ReceivePickedUpItem(NetworkIdentity itemNetId)
    {
        if (itemNetId == null)
        {
            return;
        }

        CmdPickUpItem(itemNetId);
    }

    [Client]
    public void ReceiveDroppedItem(bool success)
    {
        if (success == false || _heldItemIdentity == null)
        {
            return;
        }
        CmdDropItem();
    }

    private void OnHeldItemChanged(NetworkIdentity oldIdentity, NetworkIdentity newIdentity)
    {
        // 1. 이전에 들고 있던 가구가 있었다면 부모-자식 관계를 해제합니다.
        if (oldIdentity != null)
        {
            oldIdentity.transform.SetParent(null);
            Collider collider = oldIdentity.GetComponentInChildren<Collider>();
            if(!ReferenceEquals(collider, null))
            {
                collider.enabled = true;
            }
        }

        // 2. 새롭게 들게 된 가구가 있다면 부모-자식 관계를 설정합니다.
        if (newIdentity != null)
        {
            // 부모를 _heldPosition으로 설정
            newIdentity.transform.SetParent(_owner.HeldPosition, true);
            newIdentity.transform.localPosition = Vector3.zero;
            newIdentity.transform.localRotation = Quaternion.identity;
            Collider collider = newIdentity.GetComponentInChildren<Collider>();
            if(!ReferenceEquals(collider, null))
            {
                collider.enabled = false;
            }
        }
    }

    [Command]
    public void CmdPickUpItem(NetworkIdentity itemToHoldIdentity)
    {
        if (!_owner.isServer) return;

        _heldItemIdentity = itemToHoldIdentity;
    }

    [Command]
    public void CmdDropItem()
    {
        if (!_owner.isServer) return;

        GameObject heldItemObject = _heldItemIdentity.gameObject;
        if(heldItemObject.TryGetComponent<IGridItemHandler>(out IGridItemHandler structure))
        {
            StructureData data = DataTable.Instance.GetStructureData(structure.GetStructureTID());
            GridManager.Instance.CmdRemoveStructure(data.TID, heldItemObject);
        }
        else
        {
            CraftItemFactory.Instance.ReturnObject(_heldItemIdentity.gameObject);
        }

        _heldItemIdentity = null;
    }

    private void OnDestroy()
    {
        if(InputManager.Instance != null)
        {
            InputManager.Instance.OnPickupEvent -= OnPickupInput;
        }
        if(PhaseManager.Instance != null)
        {
            PhaseManager.Instance.PhaseDictionary[EPhaseType.ServingPhase].OnPhaseExited -= ResetItem;
            PhaseManager.Instance.PhaseDictionary[EPhaseType.PracticingPhase].OnPhaseExited -= ResetItem;
        }
    }
}