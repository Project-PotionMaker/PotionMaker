using UnityEngine;

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
        if (ReferenceEquals(item, null) == false)
        {
            GameObject newItem = item.GetComponent<IGridItemHandler>()?.TryPickUp();
            if(newItem != null)
            {
                _heldItem = newItem;
                _heldItem.transform.SetParent(_owner.HeldPosition);
                _heldItem.transform.localPosition = -0.5f * Vector3.one;
                _heldItem.transform.localRotation = Quaternion.Euler(Vector3.zero);
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

            if (itemHandler.TryDrop(targetPosition))
            {
                _heldItem.transform.SetParent(null);
                _heldItem = null;
            }
        }
        else if (phaseType == EPhaseType.ServingPhase || phaseType == EPhaseType.PracticingPhase)
        {
            GameObject gridObject = FindFrontPickupItem();
            if (gridObject != null)
            {
                IItem item = _heldItem.GetComponent<IItem>();
                if (item != null && gridObject.GetComponent<IGridItemHandler>().TryDrop(targetPosition, item.GetTID(), item.GetInputType(), _heldItem))
                {
                    _heldItem.transform.SetParent(null);
                    if (item.GetInputType() != EInputType.Potion)
                    {
                        Destroy(_heldItem);
                    }
                    _heldItem = null;
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
            CraftItemFactory.Instance.Return(_heldItem);
        }
    }
}
