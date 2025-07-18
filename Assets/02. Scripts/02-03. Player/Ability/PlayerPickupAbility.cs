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
        if (item == null)
        {
            return;
        }

        _heldItem = item;
        _heldItem.transform.SetParent(this.transform);
    }

    private void TryPutDown()
    {
        Vector3 targetPosition = _owner.GetFrontPosition();
        if (GridManager.Instance.TryDrop(targetPosition))
        {
            Debug.Log("Put Down");
            _heldItem.transform.SetParent(null);

            _heldItem = null;
        }
    }

    private GameObject FindFrontPickupItem()
    {
        if (!_owner.CheckObjectInFront())
        {
            return null;
        }

        Vector3 targetPosition = _owner.GetFrontPosition();
        GameObject item = GridManager.Instance.TryPickup(targetPosition);
        return item;
    }
}
