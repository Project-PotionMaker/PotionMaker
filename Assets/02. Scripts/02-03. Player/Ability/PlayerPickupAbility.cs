using Mirror;
using UnityEditorInternal.Profiling.Memory.Experimental;
using UnityEngine;
using static UnityEditor.Progress;

public class PlayerPickupAbility : PlayerAbility
{
    // 나중에 IPickable로 변경 가능
    private GameObject _heldItem = null;
    private PlayerAnimationAbility _animationAbility;

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

        if( _heldItem != null)
        {
            Vector3 targetPosition = _owner.GetFrontPosition();
            GridManager.Instance.UpdatePlacementPosition(targetPosition);
        }
    }

    private void OnPickupInput()
    {
        if (_heldItem == null)
        {
            TryPickup();
        }
        else
        {
            TryPutDown();
        }

        bool hasHeldItem = _heldItem != null;
        _animationAbility.SetBool(EPlayerAnimationParameter.HasHeldItem,hasHeldItem);
    }

    private void TryPickup()
    {
        GameObject item = FindFrontPickupItem();
        if(item != null)
        {
            IGridItemHandler itemHandler = item.GetComponent<IGridItemHandler>();
            if(ReferenceEquals(itemHandler, null) == false)
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
            IGridItemHandler itemHandler = _heldItem.GetComponent<IGridItemHandler>();
            if (ReferenceEquals(itemHandler, null) == true)
            {
                return;
            }

            itemHandler.TryDrop(_owner.connectionToClient, targetPosition, _heldItem);
        }
        else if (phaseType == EPhaseType.ServingPhase || phaseType == EPhaseType.PracticingPhase)
        {
            GameObject gridObject = FindFrontPickupItem();
            if (gridObject != null)
            {
                IItem item = _heldItem.GetComponent<IItem>();
                if(ReferenceEquals(item, null) == false)
                {
                    gridObject.GetComponent<IGridItemHandler>().TryDrop(
                        _owner.connectionToClient,
                        targetPosition,
                        _heldItem,
                        item.GetTID(),
                        item.GetInputType()
                        );
                }
            }
        }
    }

    private GameObject FindFrontPickupItem()
    {
        if (!_owner.CheckObjectInFront())
        {
            return null;
        }

        Vector3 targetPosition = _owner.GetFrontPosition();
        GameObject item = GridManager.Instance.GetObjectOnGrid(targetPosition);
        return item;
    }

    private void ResetItem()
    {
        if(!ReferenceEquals(_heldItem, null))
        {
            _heldItem.transform.SetParent(null);
            CraftItemFactory.Instance.CmdReturn(_heldItem);
        }
    }

    [Client]
    public void ReceivePickedUpItem(GameObject item)
    {
        if(item == null)
        {
            return;
        }

        _heldItem = item;
        _heldItem.transform.SetParent(_owner.HeldPosition);
        _heldItem.transform.localPosition = -0.5f * Vector3.one;
        _heldItem.transform.localRotation = Quaternion.Euler(Vector3.zero);
    }

    [Client]
    public void ReceiveDroppedItem(bool success)
    {
        if(success == false || _heldItem == null)
        {
            return;
        }

        _heldItem.transform.SetParent(null);
        _heldItem = null;
    }
}
