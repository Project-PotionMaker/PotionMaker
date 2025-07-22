using UnityEngine;

public class PlayerPickupAbility : PlayerAbility
{
    // 나중에 IPickable로 변경 가능
    private GameObject _heldItem = null;

    private void Start()
    {
        if (!_photonView.IsMine)
        {
            return;
        }

        InputManager.Instance.OnPickupEvent += OnPickupInput;
    }

    private void Update()
    {
        if (!_photonView.IsMine)
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
    }

    private void TryPickup()
    {
        Debug.Log("Pickup");
        GameObject item = FindFrontPickupItem();
        if (ReferenceEquals(item, null) == false)
        {
            GameObject newItem = item.GetComponent<IGridItemHandler>()?.TryPickUp();
            if(newItem != null)
            {
                _heldItem = newItem;
                _heldItem.transform.SetParent(_owner.HeldPosition);
                _heldItem.transform.localPosition = Vector3.zero;
                _heldItem.transform.localRotation = Quaternion.Euler(Vector3.zero);
            }
        }
    }

    private void TryPutDown()
    {
        Vector3 targetPosition = _owner.GetFrontPosition();
        IGridItemHandler itemHandler = _heldItem.GetComponent<IGridItemHandler>();
        if(ReferenceEquals(itemHandler, null) == false)
        {
            if (itemHandler.TryDrop(targetPosition))
            {
                _heldItem.transform.SetParent(null);

                _heldItem = null;
            }
        }
        else
        {
            GameObject gridObject = FindFrontPickupItem();
            if(gridObject != null)
            {
                IItem item = _heldItem.GetComponent<IItem>();
                if (item != null && gridObject.GetComponent<IGridItemHandler>().TryDrop(targetPosition, item.GetTID(), item.GetInputType(), _heldItem))
                {
                    _heldItem.transform.SetParent(null);
                    Destroy(_heldItem);
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
}
