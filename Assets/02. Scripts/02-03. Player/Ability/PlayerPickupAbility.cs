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
        Debug.Log("Put Down");
        _heldItem.transform.SetParent(null);
        Vector3 targetPosition = transform.position + transform.forward * 0.5f;
        GridManager.Instance.TryDrop(targetPosition);

        _heldItem = null;
    }

    private GameObject FindFrontPickupItem()
    {
        Vector3 targetPosition = transform.position + transform.forward * 0.5f;
        GameObject item = GridManager.Instance.TryPickup(targetPosition);
        return item;
    }
}
